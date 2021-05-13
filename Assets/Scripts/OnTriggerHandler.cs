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

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("triggerEnter");
        triggerEnter.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        triggerExit.Invoke();
    }

    private void OnTriggerStay(Collider other)
    {
        triggerStay.Invoke();
    }
}
