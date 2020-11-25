using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallPlayerDeath : MonoBehaviour
{
    GameObject playerObj;

    void Start()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");   
    }

    private void OnTriggerEnter(Collider other) {
        playerObj.GetComponent<PlayerDeathHandler>().StartDeathCycle();
    }
}
