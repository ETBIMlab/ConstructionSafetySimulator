using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;
using UnityEngine.XR;
using UnityEngine.Rendering.Universal;
using static UnityEngine.Rendering.Universal.ScriptableRenderer;

public abstract class ScreenSpaceRenderer : MonoBehaviour
{
    /// <summary>
    /// Contains a list of the active components that are currently drawing to a rendering camera.
    /// </summary>
    [NonSerialized] private static List<ScreenSpaceRenderer> instances;
    [NonSerialized] private static bool queuedUpdate = false;
    [NonSerialized] private static bool wasSet;
    [NonSerialized] private static Vector3 topLeft, topRight, bottomLeft, bottomRight;

    protected Action OnTargetCameraChanged;
    protected Action OnDrawOrderChanged;
    protected Action OnDrawEnabled;
    protected Action OnDrawDisabled;


    #region Inspector_Fields

    [Header("Renderer Fields")]
    [SerializeField]
    [Tooltip("The camera which you want to draw on top of. CameraLayerBlend will always run a camera in priority order.")]
    private Camera i_TargetCamera;
    [SerializeField]
    [Range(0, 100)]
    private int i_DrawOrder = 0;

    [Header("Rendering Debuging")]
    public bool StateChangeDebuging = false;
    public bool UpdateDebuging = false;

    #endregion

    #region Encapsulated_Properties
    [HideInInspector]
    [SerializeField]
    private Camera m_targetCamera;
    public Camera TargetCamera
    {
        get => m_targetCamera;
        set
        {
            if (m_targetCamera != value)
            {
                m_targetCamera = value;
                OnTargetCameraChanged.Invoke();
            }
#if UNITY_EDITOR
            i_TargetCamera = value;
#endif
        }
    }

    [HideInInspector]
    [SerializeField]
    private int m_drawOrder;
    public int DrawOrder
    {
        get => m_drawOrder;
        set
        {
            if (m_drawOrder != value)
            {
                m_drawOrder = value;
                OnDrawOrderChanged.Invoke();
            }
#if UNITY_EDITOR
            i_DrawOrder = value;
#endif
        }
    }
    
    private bool m_alwaysDraw;
    protected bool AlwaysDraw
    {
        get => m_alwaysDraw;
        set
        {
            if (m_alwaysDraw != value)
            {
                m_alwaysDraw = value;
                QueueUpdate();
            }
        }
    }

    private bool m_useDepthTexture;
    public bool UseDepthTexture
    {
        get => m_useDepthTexture;
        protected set
        {
            if (m_useDepthTexture != value)
            {
                m_useDepthTexture = value;
                UpdateDepthSettings();
            }
        }
    }

    


    #endregion

    #region Private_Variables
    protected List<Material> leftMaterials;
    protected List<Material> rightMaterials;
    private bool sudoEnabled;

    // Multipass only
    protected int effect_pass;
    private int base_pass;
    #endregion

    #region Initializing_and_Enabling

    protected virtual void Reset()
    {
        i_TargetCamera = GetComponent<Camera>();
    }

    protected virtual void Awake()
    {
        sudoEnabled = false;
        UseDepthTexture = false;
        AlwaysDraw = false;
        leftMaterials = new List<Material>();
        rightMaterials = new List<Material>();

        

        OnTargetCameraChanged += () => { QueueUpdate(); UpdateDepthSettings(); };
        //OnStrengthChanged += null;
        OnDrawOrderChanged += () => { SortInstances(); QueueUpdate(); };

        if (TargetCamera == null)
            TargetCamera = GetComponent<Camera>();
    }

    protected virtual void OnEnable()
    {
        if (StateChangeDebuging)
            Debug.Log("<b>ScreenSpaceRenderer:</b> ScreenSpaceRenderer '" + gameObject.name + "' Enabled");

        AddToInstances();

        sudoEnabled = false;
        OnDrawDisabled.Invoke();
    }

    protected virtual void OnDisable()
    {
        if (StateChangeDebuging)
            Debug.Log("<b>CameraLayerBlend:</b> " + gameObject.name + " Disabled");

        SudoDisable();

        RemoveFromInstances();
    }

    #endregion

    #region Instances_management

    protected abstract int DrawCoverage();
    
    // TODO:: Depth texture mode does not automatically unset.
    private void UpdateDepthSettings()
    {
        if (TargetCamera != null)
        {
            if (UseDepthTexture)
                TargetCamera.depthTextureMode = DepthTextureMode.Depth;
        }
    }

    private void AddToInstances()
    {
        if (instances == null)
            instances = new List<ScreenSpaceRenderer>();

        if (!instances.Contains(this))
        {
            if (instances.Count <= 0)
            {
                if (StateChangeDebuging)
                    Debug.Log("<b>ScreenSpaceRenderer:</b> Adding Static RPCall");

                AddStaticRPCall();
            }

            instances.Add(this);
        }

        SortInstances();
        QueueUpdate();
    }

    private void RemoveFromInstances()
    {
        if (instances != null && instances.Contains(this))
        {
            instances.Remove(this);
            QueueUpdate();

            if (instances.Count <= 0)
                RemoveStaticRPCall();
        }
    }

    protected virtual void LateUpdate()
    {
        if (queuedUpdate)
        {
            UpdateRenderingCameras();
            ScreenSpaceRenderer.queuedUpdate = false;
        }
    }

    private static void UpdateRenderingCameras()
    {
        if (instances == null)
            return;

        List<Camera> hidden = new List<Camera>();
        for (int i = instances.Count - 1; i >= 0; i--)
        {
            Debug.Log(instances[i].DrawCoverage());
            if (instances[i].AlwaysDraw == true)
            {
                instances[i].SudoEnable();
            }
            else if (instances[i].TargetCamera != null && hidden.Contains(instances[i].TargetCamera))
            {
                instances[i].SudoDisable();
            }
            else if (instances[i].TargetCamera != null && instances[i].DrawCoverage() == 2)
            {
                instances[i].SudoEnable();
                hidden.Add(instances[i].TargetCamera);
            }
            else
            {
                if (instances[i].DrawCoverage() == 0)
                    instances[i].SudoDisable();
                else
                    instances[i].SudoEnable();
            }
        }
    }

    public static void QueueUpdate()
    {
        queuedUpdate = true;
    }

    private void SudoDisable()
    {
        if (StateChangeDebuging)
            Debug.Log("<b>ScreenSpaceRenderer:</b> Sudo disable (strength is 0 or texture is completely painted over)");
        if (sudoEnabled != false)
        {
            sudoEnabled = false;
            OnDrawDisabled.Invoke();
        }
    }

    private void SudoEnable()
    {
        if (sudoEnabled != true)
        {
            if (StateChangeDebuging)
                Debug.Log("<b>ScreenSpaceRenderer:</b> Sudo enable (added to rendering)");
            // We could check to see if the action has been registered already. Removing the action is to ensure that we don't accidentally register the even twice.
            sudoEnabled = true;
            OnDrawEnabled.Invoke();
        }
    }


    private static void SortInstances()
    {
        instances.Sort((blend1, blend2) => blend1.DrawOrder - blend2.DrawOrder);
    }

    #endregion

    #region Base_Camera_Render_Passes

    private static void RemoveStaticRPCall()
    {
        RenderPipelineManager.endCameraRendering -= Static_SinglePass_RPM_endCameraREndering;
        RenderPipelineManager.endCameraRendering -= Static_NOXR_RPM_endCameraREndering;

        // MultiPass
        if (XRSettings.stereoRenderingMode == XRSettings.StereoRenderingMode.MultiPass)
        {
            RenderPipelineManager.endCameraRendering -= Static_MultiPass_RPM_endCameraREndering;
            RenderPipelineManager.beginFrameRendering -= Static_CountPass_RPM_beginFrameREndering;
        }
    }

    private static void AddStaticRPCall()
    {
        if (UnityEngine.XR.XRSettings.enabled)
        {

            if (XRSettings.stereoRenderingMode == XRSettings.StereoRenderingMode.MultiPass)
            {
                RenderPipelineManager.endCameraRendering += Static_MultiPass_RPM_endCameraREndering;
                RenderPipelineManager.beginFrameRendering += Static_CountPass_RPM_beginFrameREndering;
            }
            else
                RenderPipelineManager.endCameraRendering += Static_SinglePass_RPM_endCameraREndering;
        }
        else
            RenderPipelineManager.endCameraRendering += Static_NOXR_RPM_endCameraREndering;
    }

    private static void Static_SinglePass_RPM_endCameraREndering(ScriptableRenderContext context, Camera camera)
    {
        wasSet = false;
        foreach (ScreenSpaceRenderer effect in instances)
        {
            if (effect.sudoEnabled)
            {
                if (camera == effect.TargetCamera)
                {
                    if (effect.UpdateDebuging)
                        Debug.Log("<b>ScreenSpaceRenderer:</b> Static_XR from ScreenSpaceRenderer named '" + effect.name + "'");

                    if (!effect.UseDepthTexture)
                    {
                        foreach (Material mat in effect.leftMaterials)
                        {
                            Graphics.Blit(null,
                                RenderTexture.active,
                                mat, 0, 0);
                        }

                        foreach (Material mat in effect.rightMaterials)
                        {
                            Graphics.Blit(null,
                                RenderTexture.active,
                                mat, 0, 1);
                        }
                    }
                    else
                    {
                        if (!wasSet)
                        {
                            SetRaycastCorners(camera, out topLeft, out topRight, out bottomLeft, out bottomRight);
                        }

                        RenderTexture active = RenderTexture.active;

                        foreach (Material mat in effect.leftMaterials)
                        {
                            CustomDepthBlit(RenderTexture.active, mat, topLeft, topRight, bottomLeft, bottomRight);
                        }

                        foreach (Material mat in effect.rightMaterials)
                        {
                            CustomDepthBlit(RenderTexture.active, mat, topLeft, topRight, bottomLeft, bottomRight);
                        }
                    }
                }
            }
        }
    }

    private static void Static_CountPass_RPM_beginFrameREndering(ScriptableRenderContext context, Camera[] cameras)
    {
        foreach (ScreenSpaceRenderer effect in instances)
        {
            effect.base_pass = 0;
            effect.effect_pass = 0;
        }
    }

    private static void Static_MultiPass_RPM_endCameraREndering(ScriptableRenderContext context, Camera camera)
    {
        wasSet = false;
        foreach (ScreenSpaceRenderer effect in instances)
        {
            if (effect.sudoEnabled)
            {
                if (camera == effect.TargetCamera)
                {
                    if (effect.UpdateDebuging)
                        Debug.Log("<b>ScreenSpaceRenderer:</b> Static_Multi_XR from ScreenSpaceRenderer named '" + effect.name + "'. Pass -> " + effect.base_pass);

                    if (!effect.UseDepthTexture)
                    {
                        if (effect.base_pass == 0) foreach (Material mat in effect.leftMaterials)
                        {
                            Graphics.Blit(null,
                                RenderTexture.active,
                                mat, 0, 0);
                        }
                        else foreach (Material mat in effect.rightMaterials)
                        {
                            Graphics.Blit(null,
                                RenderTexture.active,
                                mat, 0, 0);
                        }
                    }
                    else
                    {
                        if (!wasSet)
                        {
                            SetRaycastCorners(camera, out topLeft, out topRight, out bottomLeft, out bottomRight);
                        }

                        if (effect.base_pass == 0) foreach (Material mat in effect.leftMaterials)
                        {
                            CustomDepthBlit(RenderTexture.active, mat, topLeft, topRight, bottomLeft, bottomRight);
                        }
                        else foreach (Material mat in effect.rightMaterials)
                        {
                            CustomDepthBlit(RenderTexture.active, mat, topLeft, topRight, bottomLeft, bottomRight);
                        }
                            
                    }

                    effect.base_pass++;
                }
            }
        }
    }

    private static void Static_NOXR_RPM_endCameraREndering(ScriptableRenderContext context, Camera camera)
    {
        wasSet = false;
        foreach (ScreenSpaceRenderer effect in instances)
        {
            if (camera == effect.TargetCamera)
            {
                if (effect.UpdateDebuging)
                    Debug.Log("<b>ScreenSpaceRenderer:</b> Static_Single_NOXR from ScreenSpaceRenderer named '" + effect.name + "' + Use Depth Textures?: " + effect.UseDepthTexture);

                if (!effect.UseDepthTexture)
                {
                    foreach (Material mat in effect.leftMaterials)
                    {
                        Graphics.Blit(null, RenderTexture.active, mat, 0, 0);
                    }
                }
                else
                {
                    if (!wasSet)
                    {
                        SetRaycastCorners(camera, out topLeft, out topRight, out bottomLeft, out bottomRight);
                        wasSet = true;
                    }
                    foreach (Material mat in effect.leftMaterials)
                        CustomDepthBlit(RenderTexture.active, mat, topLeft, topRight, bottomLeft, bottomRight);
                }
            }
        }
    }

    #endregion

    private static void CustomDepthBlit(RenderTexture dest, Material mat, Vector3 topLeft, Vector3 topRight, Vector3 bottomLeft, Vector3 bottomRight)
    {
        GL.PushMatrix();
        GL.LoadOrtho();

        mat.SetPass(0);

        GL.Begin(GL.QUADS);

        GL.MultiTexCoord2(0, 0.0f, 0.0f);
        GL.MultiTexCoord(1, bottomLeft);
        GL.Vertex3(0.0f, 0.0f, 0.0f);

        GL.MultiTexCoord2(0, 1.0f, 0.0f);
        GL.MultiTexCoord(1, bottomRight);
        GL.Vertex3(1.0f, 0.0f, 0.0f);

        GL.MultiTexCoord2(0, 1.0f, 1.0f);
        GL.MultiTexCoord(1, topRight);
        GL.Vertex3(1.0f, 1.0f, 0.0f);

        GL.MultiTexCoord2(0, 0.0f, 1.0f);
        GL.MultiTexCoord(1, topLeft);
        GL.Vertex3(0.0f, 1.0f, 0.0f);

        GL.End();
        GL.PopMatrix();
    }

    private static void SetRaycastCorners(Camera camera, out Vector3 topLeft, out Vector3 topRight, out Vector3 bottomLeft, out Vector3 bottomRight)
    {
        // Compute Frustum Corners
        float camFar = camera.farClipPlane;
        float camFov = camera.fieldOfView;
        float camAspect = camera.aspect;

        float fovWHalf = camFov * 0.5f;


        Vector3 toRight = camera.transform.right * Mathf.Tan(fovWHalf * Mathf.Deg2Rad) * camAspect;
        Vector3 toTop = camera.transform.up * Mathf.Tan(fovWHalf * Mathf.Deg2Rad);

         topLeft = (camera.transform.forward - toRight + toTop);
        float camScale = topLeft.magnitude * camFar;

        topLeft.Normalize();
        topLeft *= camScale;

        topRight = (camera.transform.forward + toRight + toTop);
        topRight.Normalize();
        topRight *= camScale;

        bottomRight = (camera.transform.forward + toRight - toTop);
        bottomRight.Normalize();
        bottomRight *= camScale;

        bottomLeft = (camera.transform.forward - toRight - toTop);
        bottomLeft.Normalize();
        bottomLeft *= camScale;
    }

    #region Editor_Only
#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        if (Application.isPlaying)
        {
            TargetCamera = i_TargetCamera;
            DrawOrder = i_DrawOrder;
        }
        else
        {
            m_targetCamera = i_TargetCamera;
            m_drawOrder = i_DrawOrder;
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
