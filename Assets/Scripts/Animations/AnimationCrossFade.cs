using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationCrossFade : MonoBehaviour
{
    private Animator _animator;

    public FadeData[] fadeDatas;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void CrossFade(int fadeDataIndex)
    {
        Debug.Assert(fadeDataIndex >= 0 && fadeDataIndex < fadeDatas.Length, "AnimationCrossFade: Index is out of bounds.", this);

        fadeDatas[fadeDataIndex].BeginCrossFade(_animator);
    }

    [System.Serializable]
    public class FadeData
    {
        [Tooltip("If fade is set to false, then the TransitionTime will be normalized to a percentage of the played clip.")]
        public bool InFixedTime = false;
        public string StateHashName = "null";
        public float TransitionTime = 0.25f;
        public int Layer = -1;
        public float NormalizedTimeOffset = 0.0f;

        internal void BeginCrossFade(Animator animator)
        {
            if (InFixedTime)
                animator.CrossFadeInFixedTime(StateHashName, TransitionTime, Layer, NormalizedTimeOffset);
            else
                animator.CrossFade(StateHashName, TransitionTime, Layer, NormalizedTimeOffset);
        }
    }

}
