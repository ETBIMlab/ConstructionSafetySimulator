using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnInputPress : MonoBehaviour
{
    public List<UnityEvent> spaceKey;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("space"))
        {
            foreach(UnityEvent e in spaceKey) {
                e.Invoke(); //call the current unity event
            }
        }
    }
}
