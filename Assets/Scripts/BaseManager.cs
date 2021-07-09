using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public abstract class BaseManager : MonoBehaviour
{
    protected static BaseManager instance;

    protected void OnEnable()
    {
        if (instance != null)
        {
            Debug.LogError("Can only have one manager enabled in ");
        }
    }

    protected void OnDisable()
    {
        
    }
}
