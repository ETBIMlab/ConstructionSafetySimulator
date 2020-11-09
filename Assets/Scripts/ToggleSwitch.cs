using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  How to use:
 *  Apply this script to a button or switch gameObject
 *  It will find any objects with the tag "Toggleable" and disable/enable them every time the script is triggered 
 *  If you want, for example, to turn off simulated "electricity", set all of your electric shock triggers inside of an empty gameObject
 *  that is part of your electrified model and give that object a collider and the tag "Toggleable" 
 * 
 */

public class ToggleSwitch : MonoBehaviour
{
    public bool firstToggleTurnsOff = true; //set true if you want the first "press" of your switch to disable all objects
    private GameObject[] objects;
    private bool isActive;

    void Start()
    {
        objects = GameObject.FindGameObjectsWithTag("Toggleable");
        if (firstToggleTurnsOff) {
            isActive = false;
        } else {
            isActive = true;
        }
    }

    private void OnTriggerEnter(Collider other) {

        foreach (GameObject obj in objects) {
            obj.SetActive(isActive);
        }
        isActive = !isActive;
        
    }
}
