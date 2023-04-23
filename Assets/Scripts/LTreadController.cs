using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LTreadController : MonoBehaviour
{
    [SerializeField]
    //public static List<GameObject> lTreadList = new List<GameObject>();
    public static List<Animator> lAnimatorList = new List<Animator>();
    public static List<Vector3> lLastPos = new List<Vector3>();

    public void TreadSpeed()
    {
        //string lTreadName = gameObject.name;

        if (lAnimatorList.Count >= 1) //make sure list isn't empty
        {
            for (int i = 0; i < lAnimatorList.Count; i++) //NOTE: do "valvesList.Length - 1" instead, if you get index out of range error
            {
                if (true)//animatorList[i].name == lTreadName)
                {
                    //DO STUFF HERE
                    Vector3 currentPos = lAnimatorList[i].rootPosition;
                    float speed = (currentPos * 100 - lLastPos[i] * 100).magnitude / Time.deltaTime;
                    if (speed > 0)
                    {
                        Console.Out.WriteLine("Left Movement: {0}", speed);
                        lLastPos[i] = currentPos;
                        
                    }
                    lAnimatorList[i].SetFloat("L Tread Speed", speed);
                    lAnimatorList[i].speed = speed;
                    /*
                    float x = animatorList[i].rootRotation.x + 2;
                    float y = animatorList[i].rootRotation.y + 2;
                    float z = animatorList[i].rootRotation.z + 2;
                    float w = animatorList[i].rootRotation.w + 2;

                    animatorList[i].rootRotation.Set(x, y, z, w);
                    */
                    //Remember to turn off this specific animator to avoid turning when another valve is activated. i = the number of the animator in the list. if in the inspector it says: "Element 0" then this would be the same as "animatorList[0]"
                }
            }
        }
        else
        {
            return; //if list is empty do nothing
        }
    }

    //OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.enabled = true; //turn on each animator component at the start
        lAnimatorList.Add(animator); //fill up your list with animators components from valve gameobjects
        lLastPos.Add(animator.transform.localPosition);
    }

    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    public void OnStateMachineUpdate(Animator animator, int stateMachinePathHash)
    {
        TreadSpeed();
    }


    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        TreadSpeed();
    }

    // OnStateMachineExit is called when exiting a state machine via its Exit Node
    public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        TreadSpeed();
    }

    public void Update()
    {
        TreadSpeed();
    }
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        TreadSpeed();
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        TreadSpeed();
    }

    // OnStateIK is called right after Animator.OnAnimatorIK()
    public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        TreadSpeed();
    }

    public void Start()
    {
        TreadSpeed();
    }

}
