using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowObject : MonoBehaviour
{
    public bool hide;
    public GameObject target;
    
    // Causes the target object to show if hidden. Check "hide" to hide object on trigger 
    void OnTriggerEnter()
    {
        target.SetActive(!hide);
    }
    
}
