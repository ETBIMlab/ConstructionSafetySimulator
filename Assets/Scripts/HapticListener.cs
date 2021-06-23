using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bhaptics.Tact.Unity;
using System;

/// <summary>
/// Used for 
/// </summary>
[ExecuteInEditMode]
public class HapticListener : MonoBehaviour
{
    private static HapticListener instance;

    [Serializable]
    public class HapticListenerReceiver
    {
        public Transform trackedPosition;
        public PositionTag positionTag = PositionTag.Default;
    }

    [Header("HapticLocations (can be empty)")]
    [Tooltip("Use this to define different listener positions for the locations on the body. Each PositionTag should only be used once. If no position tag is found matching the emmitter, than the position on the attached object is used.")]
    public HapticListenerReceiver[] hapticLocations = new HapticListenerReceiver[] { };

    private void OnEnable()
    {
        if (instance != null)
        {
            Debug.LogWarning("There cannot be two haptic listeners in the scene. Active listener: {" + instance.name + "}. Disabling component...", this);
            this.enabled = false;
        }
        else
        {
            instance = this;
        }
    }

    private void OnDisable()
    {
        if (instance == this)
            instance = null;
    }

    public static float GetDistance(Vector3 position, PositionTag positionTag = PositionTag.Default)
    {
        foreach (var hlr in instance.hapticLocations)
            if (hlr.positionTag == positionTag)
                return Vector3.Distance(position, hlr.trackedPosition.position);

        return Vector3.Distance(position, instance.transform.position);
    }

    private void OnValidate()
    {
        for (int i = 0; i < hapticLocations.Length; i++)
        {
            for (int j = i + 1; j < hapticLocations.Length; j++)
            {
                if (hapticLocations[i].positionTag == hapticLocations[j].positionTag && hapticLocations[i].positionTag != PositionTag.Default)
                {
                    Debug.LogWarning("Each PositionTag in 'HapticLocations' should only be used once ({" + hapticLocations[i].positionTag + "} is used twice). Only the first instance will be used.", this);
                }
            }
            if (hapticLocations[i].positionTag == PositionTag.None)
            {
                Debug.LogWarning("None is not intended to be used as a device position. At best, this item will be ignored.", this);
            }
        }
    }
}
