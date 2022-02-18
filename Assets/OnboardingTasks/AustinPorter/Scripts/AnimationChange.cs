using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationChange : MonoBehaviour
{
    static Animator anim;
    public bool movingForward = true;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        SetDirection(movingForward); //start the timeline
    }

    /// <summary>
    /// Change the direction to be the opposite of the current direction
    /// </summary>
    public void ChangeDirection()
    {
        SetDirection(!movingForward);
    }

    /// <summary>
    /// Set the direction of the timeline regardless of current
    /// </summary>
    public void SetDirection(bool goForward)
    {
        movingForward = goForward; //set to current direction

        if (movingForward)
        {
            //Debug.Log("Forward.");
            //anim.SetBool("NervousState", false);
            anim.SetFloat("Speed", 1.0f);
        }
        else
        {
            Debug.Log("Reverse.");
            //anim.SetBool("NervousState", true);
            anim.SetFloat("Speed", -1.0f);
        }
    }
}
