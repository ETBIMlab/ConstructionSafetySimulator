using Bhaptics.Tact.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrateTrigger : MonoBehaviour {

    TactSource[] devices;

    private void Start() {
        devices = GetComponents<TactSource>();
    }


    private void OnTriggerEnter(Collider other) {
        Debug.Log("BUZZ");

        StartCoroutine("Fade");
    }


    private void OnTriggerExit(Collider other) {
        Debug.Log("STOP BUZZ");

        StopCoroutine("Fade");
        devices[0].Stop();
        devices[1].Stop();
    }


    IEnumerator Fade() {


        for (int i = 0; i < 4; i++) {
            foreach (TactSource device in devices) {
                if (i == 0) {
                    device.DotPoints[0] = 100;
                    device.DotPoints[1] = 0;
                    device.DotPoints[2] = 0;
                    device.DotPoints[3] = 100;
                    device.DotPoints[4] = 0;
                    device.DotPoints[5] = 0;
                } else if (i == 1) {
                    device.DotPoints[0] = 0;
                    device.DotPoints[1] = 100;
                    device.DotPoints[2] = 0;
                    device.DotPoints[3] = 0;
                    device.DotPoints[4] = 100;
                    device.DotPoints[5] = 0;
                } else if (i == 2) {
                    device.DotPoints[0] = 0;
                    device.DotPoints[1] = 0;
                    device.DotPoints[2] = 100;
                    device.DotPoints[3] = 0;
                    device.DotPoints[4] = 0;
                    device.DotPoints[5] = 100;
                }

            }
            devices[0].Play();
            devices[1].Play();
            yield return new WaitForSeconds(0.5f);
        }
    }
}