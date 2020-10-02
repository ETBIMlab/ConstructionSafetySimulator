using Bhaptics.Tact.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteVibrate : MonoBehaviour {
    HapticSource[] devices;

    Transform playerTrans;
    float distance;

    // Start is called before the first frame update
    void Start() {
        playerTrans = GameObject.FindGameObjectWithTag("MainCamera").transform;     // The player's position in the scene
        devices = GetComponents<HapticSource>();                                    // Gets all the attached haptic sources (our virtual connection to the physical device)
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.V)) {      // Temporary code. If 'v' is pressed on the keyboard, the haptics activate
            ActivateTrigger();
        }
    }


    private void ActivateTrigger() {
        StopCoroutine("Vibrate");
        distance = Vector3.Distance(transform.position, playerTrans.position);  // This bit is unused, but it just gets the distance from this object (the haptic parent) to the user
        distance = Mathf.Abs(distance);
        distance = 1 / distance;

        Debug.Log("Remote BUZZ: " + distance);
        StartCoroutine("Vibrate");
             
    }



    IEnumerator Vibrate() {
        foreach (HapticSource device in devices) {
            device.Play();
            Debug.Log(device + " VIBRATED");
        }
        yield return null;
    }
}
