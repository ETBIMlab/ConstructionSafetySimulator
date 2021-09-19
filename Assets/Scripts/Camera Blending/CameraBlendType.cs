using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraLayerBlend))]
public abstract class CameraBlendType : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField]
    [Range(0, 1)]
    [Tooltip("Transparency of blend. 0 is transparent, 1 is opaque")]
    private float i_Strength = 0;
#endif

    [HideInInspector]
    [SerializeField]
    private float m_strength;
    public float Strength
    {
        get => m_strength;

        set
        {
            if (m_strength != value)
            {
                if (value >= 1)
                {
                    m_strength = 1;
                    ScreenSpaceRenderer.QueueUpdate();
                }
                else if (value <= 0)
                {
                    m_strength = 0;
                    ScreenSpaceRenderer.QueueUpdate();
                }
                else
                {
                    if ((m_strength == 0 || m_strength == 1))
                    {
                        m_strength = value;
                    ScreenSpaceRenderer.QueueUpdate();
                    }
                    else m_strength = value;
                }
                UpdatePostProcessMaterial();
            }
#if UNITY_EDITOR
            i_Strength = value;
#endif
        }
    }

    internal Material leftMaterial;
    internal Material rightMaterial;

    public abstract bool UsesDepthTextures();

    public abstract Material CreatePostProcessMaterial();
    public abstract void UpdatePostProcessMaterial();

    public void SetMaterialTexture(Texture left, Texture right)
    {
        leftMaterial.mainTexture = left;
        rightMaterial.mainTexture = right;
    }

    protected virtual void OnDestroy()
    {
        Destroy(leftMaterial);
        Destroy(rightMaterial);
        GetComponent<CameraLayerBlend>().RemoveBlendType(this);
    }

    protected virtual void Start()
    {
        leftMaterial = CreatePostProcessMaterial();
        rightMaterial = CreatePostProcessMaterial();
        GetComponent<CameraLayerBlend>().AddBlendType(this);
    }

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        if (Application.isPlaying)
        {
            Strength = i_Strength;
        }
        else
        {
            m_strength = i_Strength;
        }
    }
#endif
}
