using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;
using UnityEngine.XR;

[DisallowMultipleComponent]
public class CameraLayerBlend : ScreenSpaceRenderer
{
    private static Vector2Int DefaultDimension = Vector2Int.zero;

    #region Inspector_Fields
#if UNITY_EDITOR


    [Header("CameraLayerBlend Inspector Fields")]
    [SerializeField]
    private LayerMask i_BlendMask;
    [SerializeField]
    private Material i_BlendSkybox;

#endif
#endregion
    private List<CameraBlendType> blendEffects;

    [Header("CameraLayerBlend Editor")]
    public bool ShowBlendCamera = true;

    [SerializeField]
    [HideInInspector]
    private LayerMask m_blendMask;
    public LayerMask BlendMask
    {
        get => m_blendMask;
        set
        {
            if (m_blendMask != value)
            {
                m_blendMask = value;
                ResetBlendCamera();
            }
#if UNITY_EDITOR
            i_BlendMask = value;
#endif
        }
    }

    [SerializeField]
    [HideInInspector]
    private Material m_blendSkybox;
    public Material BlendSkybox
    {
        get => m_blendSkybox;
        set
        {
            if (m_blendSkybox != value)
            {
                m_blendSkybox = value;
                ResetSkyBox();
            }
#if UNITY_EDITOR
            i_BlendSkybox = value;
#endif
        }
    }


    #region Private_Variables
    private Camera blendLayerCamera;
    private RenderTexture leftRenderTexture;
    private RenderTexture rightRenderTexture;
    #endregion

    #region Initializing_and_Enabling

    protected override void Awake()
    {
        base.Awake();

        OnDrawDisabled += () => { RemoveRPMCall(); if (blendLayerCamera != null) blendLayerCamera.gameObject.SetActive(false); };
        OnDrawEnabled += () => { AddRPMCall(); if (blendLayerCamera != null) blendLayerCamera.gameObject.SetActive(true); };
        OnTargetCameraChanged += () => { ResetBlendCamera(); };

        blendEffects = new List<CameraBlendType>();

        ResetBlendCamera(true);
    }

    public void AddBlendType(CameraBlendType effect)
    {
        if (blendEffects.Count == 0)
            QueueUpdate();

        blendEffects.Add(effect);

        if (StateChangeDebuging)
            Debug.Log("<b>CameraLayerBlend:</b> Found Blend Component: " + effect.GetType());

        leftMaterials.Add(effect.leftMaterial);
        rightMaterials.Add(effect.rightMaterial);

        if (effect.UsesDepthTextures())
            UseDepthTexture = true;

        if (HasOverlayTexturesInit())
        {
            effect.SetMaterialTexture(leftRenderTexture, rightRenderTexture);
        }
    }

    public void RemoveBlendType(CameraBlendType effect)
    {
        leftMaterials.Remove(effect.leftMaterial);
        rightMaterials.Remove(effect.rightMaterial);
        blendEffects.Remove(effect);
    }

    public void SetAllStrengths(float strength)
    {
        Debug.Log("Setting Strengths: " + strength);
        foreach(CameraBlendType effect in blendEffects)
        {
            effect.Strength = strength;
        }
    }

    public void Initialize(Camera BaseCamera, LayerMask BlendMask, int DrawOrder, Material BlendSkybox)
    {
        if (StateChangeDebuging)
            Debug.Log("<b>CameraLayerBlend:</b> manual Initailize()");
        this.BlendMask = BlendMask;
        this.DrawOrder = DrawOrder;
        this.BlendSkybox = BlendSkybox;

        this.TargetCamera = BaseCamera;
    }

    private void OnDestroy()
    {
        if (blendLayerCamera != null)
            DestroyImmediate(blendLayerCamera.gameObject);
    }

    protected override void OnEnable()
    {
        ResetBlendCamera();

        AlwaysDraw = false;

        if (XRSettings.enabled)
        {
            if (StateChangeDebuging)
                Debug.Log("<b>CameraLayerBlend:</b> Enabled -> XR enabled, using VR");
            // In order to create render textures, we need to know the size of the render, which is device specific. To aviod this we use the first frame to get the output render texture dimensions. Unfortunatly, Unity won't allow us to render on the frame we get the dimensions, thus we will always miss one frame of rendering. Therefore, we define a defult size after the first blend texture is created. This component uses the default on the first frame, but also resets the textures to match the camera just in case.

            // If the component has textures or a default, use this until new render textures are set up.
            if (!HasOverlayTexturesInit())
            {
                if (DefaultDimension != Vector2Int.zero)
                {
                    CreateRenderTextures(DefaultDimension.x, DefaultDimension.y);
                }
                else
                {
                    AlwaysDraw = true;
                    RenderPipelineManager.endCameraRendering += TryToSetupRenderTextures;
                }
            }
        }
        else
        {
            if (StateChangeDebuging)
                Debug.Log("<b>CameraLayerBlend:</b> Enabled -> XR settings not enabled, using screen");
            CreateRenderTextures(Screen.width, Screen.height);
        }

        // Do the base AFTER we add the texture setup callback. This way, the textures are created right before we need to copy the render over
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        RenderPipelineManager.endCameraRendering -= TryToSetupRenderTextures;

        leftRenderTexture.Release();
        Destroy(leftRenderTexture);

        rightRenderTexture.Release();
        Destroy(rightRenderTexture);
    }

    #endregion

    #region RenderTextures_and_Materials

    private void CreateRenderTextures(int sceneWidth, int sceneHeight, int aaLevel = 8)
    {
        int depth = 32;
        // Might want to add a scale, not too sure about this specific implementation of render textures.
        int w = (int)(sceneWidth);
        int h = (int)(sceneHeight);

        leftRenderTexture = new RenderTexture(w, h, depth);
        leftRenderTexture.antiAliasing = aaLevel;

        rightRenderTexture = new RenderTexture(w, h, depth);
        rightRenderTexture.antiAliasing = aaLevel;



        foreach (CameraBlendType effect in blendEffects)
        {
            effect.SetMaterialTexture(leftRenderTexture, rightRenderTexture);
        }

        if (!XRSettings.enabled && blendLayerCamera != null)
        {
            if (TargetCamera == null)
                blendLayerCamera.targetTexture = null;
            else
                blendLayerCamera.targetTexture = leftRenderTexture;
        }

        if (DefaultDimension == Vector2Int.zero)
            DefaultDimension = new Vector2Int(w, h);
    }

    private void ResetBlendCamera(bool forceNew = false)
    {
        if (StateChangeDebuging)
            Debug.Log("<b>CameraLayerBlend:</b> Reseting blend camera..");

        if (blendLayerCamera == null || forceNew)
        {
            if (blendLayerCamera != null)
                Destroy(blendLayerCamera);

            GameObject go = new GameObject("Blend Camera [" + ((int) BlendMask) + "]");
            go.transform.parent = transform;
            blendLayerCamera = go.AddComponent<Camera>();
            go.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;

            if (!ShowBlendCamera)
                go.hideFlags |= HideFlags.HideInHierarchy;
        }

        // As long as the camera was made before, it'll keep using properties it already has.
        // If not, use the default properties. (this can be bad but is allowed because OnEnable runs before initail values can be set)
        if (TargetCamera != null)
        {
            blendLayerCamera.CopyFrom(TargetCamera);
            blendLayerCamera.depth -= 0.1f;
        }
        
        blendLayerCamera.cullingMask = BlendMask;

        if (!XRSettings.enabled)
        {
            if (TargetCamera == null)
                blendLayerCamera.targetTexture = null;
            else
                blendLayerCamera.targetTexture = leftRenderTexture;
        }

        blendLayerCamera.transform.localPosition = Vector3.zero;
        blendLayerCamera.transform.localRotation = Quaternion.identity;

        ResetSkyBox();
    }

    private void ResetSkyBox()
    {
        if (BlendSkybox != null)
        {
            if (blendLayerCamera == null)
                return;

            Skybox skybox = blendLayerCamera.GetComponent<Skybox>();
            if (skybox == null)
                skybox = blendLayerCamera.gameObject.AddComponent<Skybox>();

            skybox.material = BlendSkybox;
        }
    }

    /// <summary>
    /// Waits for a render so it can get the render width and height for the VR without finding and setting it manually. This prevents a overrlay render from occuring on the first frame unless default sizes are set. Used for XR only.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="camera"></param>
    private void TryToSetupRenderTextures(ScriptableRenderContext context, Camera camera)
    {
        if (camera == blendLayerCamera || camera == TargetCamera)
        {
            if (UpdateDebuging)
                Debug.Log("<b>CameraLayerBlend:</b> TryToSetup:: Active Render Texture: " + RenderTexture.active);

            if (RenderTexture.active != null)
            {
                if (StateChangeDebuging)
                    Debug.Log("<b>CameraLayerBlend:</b> TryToSetup:: Render textures are set up!");

                CreateRenderTextures(RenderTexture.active.width, RenderTexture.active.height);

                RenderPipelineManager.endCameraRendering -= TryToSetupRenderTextures;

                AlwaysDraw = false;
            }
        }
    }

    public bool HasOverlayTexturesInit()
    {
        return leftRenderTexture != null && rightRenderTexture != null;
    }

    #endregion

    

    #region Blend_Camera_Render_Passes

    private void AddRPMCall()
    {
        if (XRSettings.enabled)
        {
            switch (XRSettings.stereoRenderingMode)
            {
                case XRSettings.StereoRenderingMode.MultiPass:
                    RenderPipelineManager.endCameraRendering -= MultiPass_RPM_endCameraRendering;
                    RenderPipelineManager.endCameraRendering += MultiPass_RPM_endCameraRendering;
                    break;
                case XRSettings.StereoRenderingMode.SinglePass:
                    throw new NotImplementedException("Standard Single Pass has not been supported with camera layer blending yet.");
                case XRSettings.StereoRenderingMode.SinglePassInstanced:
                    RenderPipelineManager.endCameraRendering -= SinglePassInstanced_RPM_endCameraRendering;
                    RenderPipelineManager.endCameraRendering += SinglePassInstanced_RPM_endCameraRendering;
                    break;
                case XRSettings.StereoRenderingMode.SinglePassMultiview:
                    throw new NotImplementedException("Multiview has not been supported with camera layer blending yet.");
            }

        }
        // If XR settings are disabled, we will just set the output render texture of the camera.
        // This cannot be done for XR because we specifically need to seporate the depth slices of the camera.
        // Note that if the BaseCamera is null, we remove the target render texture.
        /*else
        {
            RenderPipelineManager.endCameraRendering -= NoXR_RPM_endCameraRendering;
            RenderPipelineManager.endCameraRendering += NoXR_RPM_endCameraRendering;
        }*/
    }
    private void RemoveRPMCall()
    {
        if (XRSettings.enabled)
        {
            switch (XRSettings.stereoRenderingMode)
            {
                case XRSettings.StereoRenderingMode.MultiPass:
                    RenderPipelineManager.endCameraRendering -= MultiPass_RPM_endCameraRendering;
                    break;
                case XRSettings.StereoRenderingMode.SinglePass:
                    throw new NotImplementedException("Standard Single Pass has not been supported with camera layer blending yet.");
                case XRSettings.StereoRenderingMode.SinglePassInstanced:
                    RenderPipelineManager.endCameraRendering -= SinglePassInstanced_RPM_endCameraRendering;
                    break;
                case XRSettings.StereoRenderingMode.SinglePassMultiview:
                    throw new NotImplementedException("Multiview has not been supported with camera layer blending yet.");
            }

        }
        // If XR settings are disabled, we will just set the output render texture of the camera.
        // This cannot be done for XR because we specifically need to seporate the depth slices of the camera.
        // Note that if the BaseCamera is null, we remove the target render texture.
        /*else
        {
            RenderPipelineManager.endCameraRendering -= NoXR_RPM_endCameraRendering;
        }*/

    }

    private void SinglePassInstanced_RPM_endCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (camera == blendLayerCamera)
        {
            if (UpdateDebuging)
                Debug.Log("<b>CameraLayerBlend:</b> Rendering Blend Texture with Single Pass");

            RenderTexture active = RenderTexture.active;
            
            Graphics.Blit(
                source: active,
                dest: leftRenderTexture,
                scale: new Vector2(1.0f, -1.0f),
                offset: new Vector2(0.0f, 1.0f),
                sourceDepthSlice: 0,
                destDepthSlice: 0);

            Graphics.Blit(
                source: active,
                dest: rightRenderTexture,
                scale: new Vector2(1.0f, -1.0f),
                offset: new Vector2(0.0f, 1.0f),
                sourceDepthSlice: 1,
                destDepthSlice: 0);
        }
    }

    private void MultiPass_RPM_endCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (camera == blendLayerCamera)
        {
            if (UpdateDebuging)
                Debug.Log("<b>CameraLayerBlend:</b> Rendering Blend Texture with MultiPass. Pass -> " + effect_pass);

            Graphics.Blit(
                source: RenderTexture.active,
                dest: (effect_pass == 0) ? leftRenderTexture : rightRenderTexture,
                sourceDepthSlice: effect_pass,
                destDepthSlice: 0);

            effect_pass++;
            /*Graphics.Blit(
                source: active,
                dest: rightRenderTexture,
                sourceDepthSlice: 1,
                destDepthSlice: 0);*/
        }
    }

    protected override int DrawCoverage()
    {
        if (blendEffects.Count == 0)
            return 0;

        bool allZero = true;
        foreach(CameraBlendType effect in blendEffects)
        {
            if (effect.Strength >= 1)
                return 2;

            if (effect.Strength > 0)
                allZero = false;
        }

        return allZero ? 0 : 1;
    }

    #endregion

    #region Editor_Only
#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        if (Application.isPlaying)
        {
            BlendMask = i_BlendMask;
            BlendSkybox = i_BlendSkybox;
        }
        else
        {
            m_blendMask = i_BlendMask;
            m_blendSkybox = i_BlendSkybox;
        }
    }
#endif
    #endregion

    //private void CreateBlendMaterials()
    //{
    //    Debug.Log("Created Mat. ");
    //    // Unity has a built-in shader that is useful for drawing
    //    // simple colored things. In this case, we just want to use
    //    // a blend mode that inverts destination colors.
    //    var shader = Shader.Find("Hidden/TestingScreenSpace");

    //    leftBlendMat = new Material(shader);
    //    leftBlendMat.hideFlags = HideFlags.HideAndDontSave;
    //    leftBlendMat.SetColor("_Color", new Color(1, 1, 1, blend));
    //    leftBlendMat.SetTexture("_MainTex", leftRenderTexture);

    //    rightBlendMat = new Material(shader);
    //    rightBlendMat.hideFlags = HideFlags.HideAndDontSave;
    //    rightBlendMat.SetColor("_Color", new Color(1, 1, 1, blend));
    //    rightBlendMat.SetTexture("_MainTex", rightRenderTexture);

    //}

    /*
    void OnEnable()
    {
        RenderPipelineManager.endCameraRendering += RenderPipelineManager_endCameraRendering;
    }
    void OnDisable()
    {
        RenderPipelineManager.endCameraRendering -= RenderPipelineManager_endCameraRendering;
    }
    private void RenderPipelineManager_endCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        OnPostRender();
    }

    public Material mat;

    // Will be called from camera after regular rendering is done.
    public void OnPostRender()
    {

        if (!mat)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things. In this case, we just want to use
            // a blend mode that inverts destination colors.
            var shader = Shader.Find("Hidden/Internal-Colored");
            mat = new Material(shader);
            mat.hideFlags = HideFlags.HideAndDontSave;
            // Set blend mode to invert destination colors.
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusDstColor);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            // Turn off backface culling, depth writes, depth test.
            mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            mat.SetInt("_ZWrite", 0);
            mat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
        }

        GL.PushMatrix();
        GL.LoadOrtho();

        // activate the first shader pass (in this case we know it is the only pass)
        mat.SetPass(0);
        // draw a quad over whole screen
        GL.Begin(GL.QUADS);
        GL.Vertex3(0, 0, 0);
        GL.Vertex3(1, 0, 0);
        GL.Vertex3(1, 1, 0);
        GL.Vertex3(0, 1, 0);
        GL.End();
        GL.PopMatrix();

        // Repeat for the right eye
        //mat.SetPass(0);
        //GL.Begin(GL.QUADS);
        //GL.Vertex3(1, 0, 0);
        //GL.Vertex3(2, 0, 0);
        //GL.Vertex3(2, 1, 0);
        //GL.Vertex3(1, 1, 0);
        //GL.End();
        //GL.PopMatrix();
    }
    */
}

