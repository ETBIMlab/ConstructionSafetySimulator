using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera2DIrisBlend : CameraBlendType
{
    [SerializeField]
    [Min(0)]
    [Tooltip("Transparency of blend. 0 is transparent, 1 is opaque")]
    private float i_EdgeBlend = 0.0f;

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

    public override Material CreatePostProcessMaterial()
    {
        var shader = Shader.Find("Hidden/CustomVRTextureIris");
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

        return mat;
    }

    public override void UpdatePostProcessMaterial()
    {
        leftMaterial.SetFloat("_Strength", Strength);
        leftMaterial.SetFloat("_EdgeWidth", EdgeBlend);

        rightMaterial.SetFloat("_Strength", Strength);
        rightMaterial.SetFloat("_EdgeWidth", EdgeBlend);
    }

    public override bool UsesDepthTextures()
    {
        return false;
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        if (Application.isPlaying)
        {
            EdgeBlend = i_EdgeBlend;
        }
        else
        {
            m_edgeBlend = i_EdgeBlend;
        }
    }
#endif
}
