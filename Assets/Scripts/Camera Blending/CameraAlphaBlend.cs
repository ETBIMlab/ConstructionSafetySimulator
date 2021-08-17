using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAlphaBlend : CameraBlendType
{
    public override Material CreatePostProcessMaterial()
    {
        var shader = Shader.Find("Hidden/CustomVRTextureBlend");
        Material mat = new Material(shader);
        mat.hideFlags = HideFlags.HideAndDontSave;
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        // Turn off backface culling, depth writes, depth test.
        mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        mat.SetInt("_ZWrite", 0);
        mat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);

        //leftBlendMat.SetTexture("_MainTex", leftRenderTexture);
        mat.SetColor("_Color", new Color(1, 1, 1, Strength));

        return mat;
    }

    public override void UpdatePostProcessMaterial()
    {
        leftMaterial.SetColor("_Color", new Color(1, 1, 1, Strength));
        rightMaterial.SetColor("_Color", new Color(1, 1, 1, Strength));
    }

    public override bool UsesDepthTextures()
    {
        return false;
    }
}
