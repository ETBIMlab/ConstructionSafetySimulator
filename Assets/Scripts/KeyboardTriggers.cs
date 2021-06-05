using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class KeyboardTriggers : MonoBehaviour
{
    [Serializable]
    public class KeystrokeEvent
    {
        public KeyCode keyCode;
        public UnityEvent trigger;
    }

    public KeystrokeEvent[] keystrokeEvents;

    // Update is called once per frame
    void Update()
    {
        foreach (var item in keystrokeEvents)
            if(Input.GetKeyDown(item.keyCode))
                item.trigger.Invoke();
    }
}
