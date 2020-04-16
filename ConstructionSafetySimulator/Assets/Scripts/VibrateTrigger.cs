using Bhaptics.Tact.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrateTrigger : MonoBehaviour {

    TactSource[] devices;
    [SerializeField]
    GameObject rightHand;
    [SerializeField]
    GameObject leftHand;

    private void Start() {
        devices = GetComponents<TactSource>();
    }


    private void OnTriggerEnter(Collider other) {
        if(other.gameObject == rightHand) {
            Debug.Log("R BUZZ");
            StartCoroutine("RightBuzz");
        } else if (other.gameObject == leftHand) {
            Debug.Log("L BUZZ");
            StartCoroutine("LeftBuzz");
        } else {
            Debug.Log("BUZZ");
            StartCoroutine("Fade");
        }

        
    }


    private void OnTriggerExit(Collider other) {
        Debug.Log("STOP BUZZ");

        StopCoroutine("Fade");
        StopCoroutine("RightBuzz");
        StopCoroutine("LeftBuzz");
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

    IEnumerator RightBuzz() { 

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
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator LeftBuzz() {
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
            devices[1].Play();
            yield return new WaitForSeconds(0.5f);
        }
    }
}