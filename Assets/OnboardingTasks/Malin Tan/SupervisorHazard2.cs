using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SupervisorHazard2 : MonoBehaviour
{
    Animator animator;
    private bool isWalking = false;
    public Transform target;
    NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void SetTarget()
    {
        if (animator.GetBool("UserClose"))
        {
            agent.SetDestination(target.position);
            isWalking = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isWalking)
        {
            SetTarget();
        }
        else
        {
            CheckReachedDestination();
        }
    }

    private void CheckReachedDestination()
    {
        float remainingDistance = Vector3.Distance(transform.position, target.position);
        if (remainingDistance < 0.3f)
        {
            isWalking = false;
            agent.isStopped = true;
            animator.ResetTrigger("UserClose");
            animator.SetBool("reachedDest", true);

        }
    }
}
