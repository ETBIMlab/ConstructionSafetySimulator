using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardAnimationController : MonoBehaviour
{
    [Tooltip("The object with an animator attached to it")]
    [SerializeField] public Animator animationController;
    [Tooltip("The keyboard key that will start the animation.")]
    [SerializeField] public KeyCode keycode;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(keycode)) {
            animationController.SetBool("KeyPressed", true);
        }
    }
}
