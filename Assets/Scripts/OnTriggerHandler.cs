using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/**
* Author: Nevin Foster
* Adds UnityEvents to object triggers
*/
public class OnTriggerHandler : MonoBehaviour
{
    public UnityEvent triggerEnter;
    public UnityEvent triggerExit;
    public UnityEvent triggerStay;
    public UnityEvent firstTriggerEnter;
    public UnityEvent lastTriggerExit;

    public int collisionCount;

    private void OnEnable()
    {
        collisionCount = 0;
    }

    private void OnDisable()
    {
        collisionCount = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Enter! : " + collisionCount);
        collisionCount++;
        if (collisionCount == 1)
            firstTriggerEnter.Invoke();
        triggerEnter.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        collisionCount--;
        if (collisionCount == 0)
            lastTriggerExit.Invoke();
        triggerExit.Invoke();
    }

    private void OnTriggerStay(Collider other)
    {
        triggerStay.Invoke();
    }

    public void OnTriggerEnter()
    {
        collisionCount++;
        if (collisionCount == 1)
            firstTriggerEnter.Invoke();
        triggerEnter.Invoke();
    }

    public void OnTriggerExit()
    {
        collisionCount--;
        if (collisionCount < 0)
            collisionCount = 0;
        if (collisionCount == 0)
            lastTriggerExit.Invoke();
        triggerExit.Invoke();
    }

    public void OnTriggerStay()
    {
        triggerStay.Invoke();
    }
}
