using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Limits the amount of enabled direct children objects. Does this by Maintaining a queue of the active children on the transform of this object.
/// </summary>
public class ActiveGroup : MonoBehaviour
{
    [Tooltip("A list of active objects once the ")]
    public Transform[] activeOnStart;
    [Tooltip("Determines if the gameobject should disable on start. THIS SHOULD ONLY BE ON IN EXTREME CASES.")]
    public bool AutomaticUpdating = false;

    private Queue<Transform> activeQueue;

    private void OnValidate()
    {
        if (activeOnStart != null)
            for (int i = 0; i < activeOnStart.Length; i++)
            {
                if (activeOnStart[i] != null && activeOnStart[i].parent != transform)
                {
                    Debug.LogError("ActiveOnStart can only include transforms that are children of this component. Removing transform: " + activeOnStart[i].name, this);
                    activeOnStart[i] = null;
                }
            }
    }

    private void Start()
    {
        if (!AutomaticUpdating)
        {
            this.enabled = false;
        }

        ResetToStart();
    }

    public void ForceUpdate()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf)
            {
                if (!activeQueue.Contains(child))
                {
                    activeQueue.Enqueue(child);
                }
            }
        }

        for (int i = activeOnStart.Length; i < activeQueue.Count; i++)
        {
            Transform last = activeQueue.Dequeue();
            if (last != null) last.gameObject.SetActive(false);
        }
    }

    public void ResetToStart()
    {
        activeQueue = new Queue<Transform>(activeOnStart);

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(activeQueue.Contains(child));
        }
    }

    private void Update()
    {
        ForceUpdate();
    }
}
