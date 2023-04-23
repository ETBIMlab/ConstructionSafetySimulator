using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTreadController : MonoBehaviour
{
    [SerializeField]
    //public static List<GameObject> rTreadList = new List<GameObject>();
    public static List<Animator> rAnimatorList = new List<Animator>();
    public static List<Vector3> rLastPos = new List<Vector3>();

    public void TreadSpeed()
    {
        //string rTreadName = gameObject.name;

        if (rAnimatorList.Count >= 1) //make sure list isn't empty
        {
            for (int i = 0; i < rAnimatorList.Count; i++) //NOTE: do "valvesList.Length - 1" instead, if you get index out of range error
            {
                if (true)//rAnimatorList[i].name == rTreadName)
                {
                    //DO STUFF HERE
                    Vector3 currentPos = rAnimatorList[i].rootPosition;
                    float speed = (currentPos * 100 - rLastPos[i] * 100).magnitude*1000 / Time.deltaTime;
                    if (speed > 0)
                    {
                        Console.Out.WriteLine("Right Movement: {0}", speed);
                        rLastPos[i] = currentPos;
                        

                    }
                    rAnimatorList[i].SetFloat("R Tread Speed", speed);
                    rAnimatorList[i].speed = speed;
                    /*
                    float x = rAnimatorList[i].rootRotation.x + 2;
                    float y = rAnimatorList[i].rootRotation.y + 2;
                    float z = rAnimatorList[i].rootRotation.z + 2;
                    float w = rAnimatorList[i].rootRotation.w + 2;

                    rAnimatorList[i].rootRotation.Set(x, y, z, w);
                    */
                    //Remember to turn off this specific animator to avoid turning when another valve is activated. i = the number of the animator in the list. if in the inspector it says: "Element 0" then this would be the same as "rAnimatorList[0]"
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
        rAnimatorList.Add(animator); //fill up your list with animators components from valve gameobjects
        rLastPos.Add(animator.transform.localPosition);
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
