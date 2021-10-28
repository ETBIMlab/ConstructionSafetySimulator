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
    public bool useTag;
    public string tagName;
    public UnityEvent triggerEnter;
    public UnityEvent triggerExit;
    public UnityEvent triggerStay;
    public UnityEvent firstTriggerEnter;
    public UnityEvent lastTriggerExit;


    [HideInInspector]
    [System.NonSerialized]
    public int collisionCount;

    private void Awake()
    {
        collisionCount = -1;
    }

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
        if (collisionCount == -1) return;

        Debug.Log("Trigger entered, other: " + other.name, other);
        collisionCount++;
        if (collisionCount == 1)
            firstTriggerEnter.Invoke();
        triggerEnter.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (collisionCount == -1) return;

        collisionCount--;
        if (collisionCount == 0)
            lastTriggerExit.Invoke();
        triggerExit.Invoke();
    }

    private void OnTriggerStay(Collider other)
    {
        if (collisionCount == -1) return;

        triggerStay.Invoke();
    }

    public void OnTriggerEnter()
    {
        if (collisionCount == -1) return;

        collisionCount++;
        if (collisionCount == 1)
            firstTriggerEnter.Invoke();
        triggerEnter.Invoke();
    }

    public void OnTriggerExit()
    {
        if (collisionCount == -1) return;

        collisionCount--;
        if (collisionCount < 0)
            collisionCount = 0;
        if (collisionCount == 0)
            lastTriggerExit.Invoke();
        triggerExit.Invoke();
    }

    public void OnTriggerStay()
    {
        if (collisionCount == -1) return;

        triggerStay.Invoke();
    }
}
