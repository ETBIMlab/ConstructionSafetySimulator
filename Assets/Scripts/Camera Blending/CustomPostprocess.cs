using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPostprocess : ScreenSpaceRenderer
{
    #region Strength
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
                }
                else if (value <= 0)
                {
                    m_strength = 0;
                    QueueUpdate();
                }
                else
                {
                    if ((m_strength == 0 || m_strength == 1))
                    {
                        m_strength = value;
                        QueueUpdate();
                    }
                    else m_strength = value;
                }

            }
#if UNITY_EDITOR
            i_Strength = value;
#endif
        }
    }
    #endregion

    [SerializeField]
    [Tooltip("The material that should be applied to the camera")]
    private Material PostProcessEffect = null;

    protected override void OnEnable()
    {
        base.OnEnable();
        if (PostProcessEffect != null)
        {
            leftMaterials.Add(PostProcessEffect);
            rightMaterials.Add(PostProcessEffect);
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (PostProcessEffect != null)
        {
            leftMaterials.Remove(PostProcessEffect);
            rightMaterials.Remove(PostProcessEffect);
        }
    }

    protected override int DrawCoverage()
    {
        if (Strength == 0)
            return 0;
        else
            return 1;
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
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
