using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraWorldIrisBlend : CameraBlendType
{
    [SerializeField]
    [Min(0)]
    [Tooltip("Transparency of blend. 0 is transparent, 1 is opaque")]
    private float i_EdgeBlend = 0.0f;

    [SerializeField]
    [Tooltip("Transparency of blend. 0 is transparent, 1 is opaque")]
    private Vector3 i_IrisOrigin = Vector3.zero;

    [HideInInspector]
    [SerializeField]
    private float m_edgeBlend;
    public float EdgeBlend
    {
        get => m_edgeBlend;

        set
        {
            if (m_edgeBlend != value)
            {
                m_edgeBlend = value;
                UpdatePostProcessMaterial();
            }
#if UNITY_EDITOR
            m_edgeBlend = value;
#endif
        }
    }

    [HideInInspector]
    [SerializeField]
    private Vector3 m_irisOrigin;
    public Vector3 IrisOrigin
    {
        get => m_irisOrigin;

        set
        {
            if (m_irisOrigin != value)
            {
                m_irisOrigin = value;
                UpdatePostProcessMaterial();
            }
#if UNITY_EDITOR
            m_irisOrigin = value;
#endif
        }
    }

    public override Material CreatePostProcessMaterial()
    {
        var shader = Shader.Find("Hidden/CustomVRTextureWorldIris");
        Material mat = new Material(shader);
        mat.hideFlags = HideFlags.HideAndDontSave;
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        // Turn off backface culling, depth writes, depth test.
        mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        mat.SetInt("_ZWrite", 0);
        mat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);

        //leftBlendMat.SetTexture("_MainTex", leftRenderTexture);
        mat.SetColor("_Color", new Color(1, 1, 1, 1));

        mat.SetFloat("_Strength", Strength);
        mat.SetFloat("_EdgeWidth", EdgeBlend);
        mat.SetVector("_WorldSpaceOrigin", IrisOrigin);

        return mat;
    }

    public override void UpdatePostProcessMaterial()
    {
        if (leftMaterial == null || rightMaterial == null)
            return;

        leftMaterial.SetFloat("_Strength", Strength);
        leftMaterial.SetFloat("_EdgeWidth", EdgeBlend);
        leftMaterial.SetVector("_WorldSpaceOrigin", IrisOrigin);

        rightMaterial.SetFloat("_Strength", Strength);
        rightMaterial.SetFloat("_EdgeWidth", EdgeBlend);
        rightMaterial.SetVector("_WorldSpaceOrigin", IrisOrigin);
    }

    public override bool UsesDepthTextures()
    {
        return true;
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        if (Application.isPlaying)
        {
            EdgeBlend = i_EdgeBlend;
            IrisOrigin = i_IrisOrigin;
        }
        else
        {
            m_edgeBlend = i_EdgeBlend;
            m_irisOrigin = i_IrisOrigin;
        }
    }
#endif
}
