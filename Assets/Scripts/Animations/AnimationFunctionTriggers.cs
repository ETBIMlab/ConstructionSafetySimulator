using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class AnimationFunctionTriggers : MonoBehaviour
{
    [SerializeField]
    public UnityEvent[] events;

    public void TriggerIndex(int index)
    {
        Debug.Assert(index >= 0 && index < events.Length, "AnimationFunctionTriggers: Index is out of bounds.", this);
        events[index].Invoke();
    }
}
