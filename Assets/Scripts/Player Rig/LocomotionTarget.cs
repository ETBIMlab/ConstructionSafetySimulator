using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(OnTriggerHandler))]
public class LocomotionTarget : MonoBehaviour
{
    [System.NonSerialized] public static List<LocomotionTarget> CurrentTargets;

    public static LocomotionTarget CurrentTarget
    {
        get
        {
            if (CurrentTargets == null || CurrentTargets.Count <= 0)
                return null;
            return CurrentTargets[0];
        }
    }

    private OnTriggerHandler _onTriggerHandler;

    public UnityEvent<bool> OnStateChange;

    private void Awake()
    {
        _onTriggerHandler = GetComponent<OnTriggerHandler>();
    }

    private void OnEnable()
    {
        _onTriggerHandler.firstTriggerEnter.AddListener(AddToTargets);
        _onTriggerHandler.lastTriggerExit.AddListener(RemoveFromTargets);
    }

    private void OnDisable()
    {
        _onTriggerHandler.firstTriggerEnter.RemoveListener(AddToTargets);
        _onTriggerHandler.lastTriggerExit.RemoveListener(RemoveFromTargets);
        RemoveFromTargets();
    }

    private void AddToTargets()
    {
        if (CurrentTargets == null)
            CurrentTargets = new List<LocomotionTarget>();

        CurrentTargets.Add(this);
        OnStateChange.Invoke(true);
    }

    private void RemoveFromTargets()
    {
        if (CurrentTargets != null && CurrentTargets.Contains(this))
        {
            CurrentTargets.Remove(this);
            OnStateChange.Invoke(false);
        }
    }
}
