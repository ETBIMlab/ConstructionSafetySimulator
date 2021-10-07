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

    // void Start() {
    //     KeystrokeEvent keystroke = new KeystrokeEvent();
    //     keystroke.keyCode = KeyCode.Alpha0;
    //     keystroke.trigger = new UnityEvent();
    //     keystroke.trigger.AddListener(()=>{
    //         TimeRecorder.updateTimeRecords("test");
    //     });
    //     keystrokeEvents = new KeystrokeEvent[]{keystroke};
    // }

    // Update is called once per frame
    void Update()
    {
        foreach (var item in keystrokeEvents)
            if(Input.GetKeyDown(item.keyCode))
                item.trigger.Invoke();
    }

    public void ForceKeyPress(KeyCode keyCode)
    {
        foreach (var item in keystrokeEvents)
            if (item.keyCode == keyCode)
                item.trigger.Invoke();
    }

    public void ForceElementPress(int element)
    {
        if (element >= 0 && element < keystrokeEvents.Length)
            keystrokeEvents[element].trigger.Invoke();
        else
            Debug.LogWarning("Index is out of bounds. Ignoring element press", this);
    }
}
