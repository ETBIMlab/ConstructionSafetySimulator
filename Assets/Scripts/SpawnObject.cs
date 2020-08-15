using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObject : MonoBehaviour
{
    public Transform Spawnpoint;
    public GameObject Prefab;
    
    // Will spawn the prefab object at the location of the spawnpoint.
    // Use an empty game object to define the spawn point
    void OnTriggerEnter()
    {
        Instantiate(Prefab, Spawnpoint.position, Spawnpoint.rotation);
    }

}
