using Bhaptics.Tact.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteVibrate : MonoBehaviour { // Currently the 'distance' functionality is not fully implemented
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
            ActivateTrigger(this.gameObject, 1);
        }
    }


    public void ActivateTrigger(GameObject source, float fallOffMultiplier) {
        StopCoroutine("Vibrate");
        distance = Vector3.Distance(source.transform.position, playerTrans.position);  // This bit is unused, but it just gets the distance from this object (the haptic parent) to the user
        distance = Mathf.Abs(distance);
        distance = fallOffMultiplier / distance;

        Debug.Log("Remote BUZZ: " + distance);
        StartCoroutine("Vibrate", distance);

    }

    public void ActivateTrigger() {
        StopCoroutine("Vibrate");
        Debug.Log("BUZZ");
        StartCoroutine("Vibrate", 1);
             
    }



    IEnumerator Vibrate(float inverseDistance) {
        foreach (HapticSource device in devices) {
            device.Play();


            for(int i = 0; i < 20; i++) {
                //device.clip.DotPoints[i].value = inverseDistance  // This doesn't work because we need to modify the bHaptics source code to publicize the DotPoints field first. See Nick's Documentation on DirectionalHaptic.cs for more info.
            }
            //Debug.Log(device + " VIBRATED");
        }
        yield return null;
    }
}
