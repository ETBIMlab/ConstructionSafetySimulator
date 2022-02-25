using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnInputPress : MonoBehaviour
{
    public List<UnityEvent> resetTimelineKey;
    public List<UnityEvent> toggleTimelineStateKey;
    public List<UnityEvent> toggleTimelineFreezeKey;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("space"))
        {
            foreach(UnityEvent e in resetTimelineKey) {
                e.Invoke(); //call the current unity event
            }
        }
        if (Input.GetKeyDown("f"))
        {
            foreach (UnityEvent e in toggleTimelineFreezeKey)
            {
                e.Invoke(); //call the current unity event
            }
        }
        if (Input.GetKeyDown("p"))
        {
            foreach (UnityEvent e in toggleTimelineStateKey)
            {
                e.Invoke(); //call the current unity event
            }
        }
    }
}
