using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationOffset : MonoBehaviour
{
    [Range(0,1)]
    [Tooltip("This is a percent offset")]
    public float offset = 0f;
    public string animationName = "Place Holder";
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Animator>().Play(animationName, 0, offset);
    }
}
