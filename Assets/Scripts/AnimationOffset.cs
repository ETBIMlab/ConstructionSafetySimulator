using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
* Simple Offset component for animations. Only compateble with animations that should
* play on enable.
*/
[RequireComponent(typeof(Animator))]
public class AnimationOffset : MonoBehaviour
{
    [Range(0,1)]
    [Tooltip("This is a percent offset")]
    public float offset = 0f;
    public string animationName = "Place Holder";

    private Animator animator;
    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        if(!animator.enabled)
            animator.enabled = true;
        animator.Play(animationName, 0, offset);
    }
}
