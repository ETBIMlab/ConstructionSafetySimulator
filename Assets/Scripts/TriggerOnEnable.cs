using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerOnEnable : MonoBehaviour
{
    public UnityEvent triggers;

    private void Start()
    {
        Debug.Log("Trigger!");
        triggers.Invoke();
    }

    private void Update()
    {

    }
}
