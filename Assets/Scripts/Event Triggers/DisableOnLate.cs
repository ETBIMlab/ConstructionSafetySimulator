using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to disable components after one update cycle. This is useful for when we want a component to start disabled but still run the start and update functions to initailize values. Specifically used with ONI ropes to begin render but prefent updates.
/// </summary>
public class DisableOnLate : MonoBehaviour
{
    [Tooltip("The component that should be disabled after one update")]
    public Behaviour comp;

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        if (comp != null)
            comp.enabled = false;
        this.enabled = false;
    }

    
}
