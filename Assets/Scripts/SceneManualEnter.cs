using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManualEnter : MonoBehaviour
{
    [Tooltip("The scene number of this hazard.")]
    [SerializeField] public int sceneNumber;
    int currentScene;
    
    void Start() 
    {
        currentScene = SceneControlVars.currentScene;
    }
    
    private void OnTriggerEnter(Collider other) 
    {      
        SceneControlVars.currentScene = sceneNumber;
    }

    private void OnTriggerExit(Collider other) 
    {
        //SceneControlVars.currentScene = -1;
    }
}
