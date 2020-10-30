using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleOutlet : MonoBehaviour
{
    
    public GameObject triggerObject;
    private bool enable;

    void Start() {
        enable = false;
        
    }

    private void OnTriggerEnter(Collider other) {
        triggerObject.SetActive(enable);
        enable = !enable;
    }
}
