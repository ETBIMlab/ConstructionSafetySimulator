//using Bhaptics.Tact.Unity;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class DirectionalHaptic : MonoBehaviour {

//    [SerializeField]
//    GameObject rightHand;
//    [SerializeField]
//    GameObject leftHand;
//    [SerializeField]
//    GameObject sourceObject;

//    Camera mainCam;

//    Vector3 lookVector;
//    Vector3 camToObjVector;

//    TactSource[] devices;

//    Transform playerTrans;
//    float distance;

//    float angleBetween;

//    private void Start() {
//        mainCam = Camera.main;
//        playerTrans = GameObject.FindGameObjectWithTag("MainCamera").transform;
//        devices = GetComponents<TactSource>();
//    }


//    private void Update() {
//        if (Input.GetKeyDown(KeyCode.V)) {
//            ActivateTrigger();
//        }

//        CalculateAngle();
//    }

//    void CalculateAngle() {
//        //lookVector = Vector3.ProjectOnPlane(mainCam.transform.forward, Vector3.up);
//        lookVector = mainCam.transform.forward;
//        lookVector = new Vector3(lookVector.x, 0.0f, lookVector.z);
//        camToObjVector = sourceObject.transform.position - mainCam.transform.position;
//        //camToObjVector = Vector3.ProjectOnPlane(camToObjVector, Vector3.up);
//        camToObjVector = new Vector3(camToObjVector.x, 0.0f, camToObjVector.z);

//        angleBetween = Vector3.SignedAngle(lookVector, camToObjVector, Vector3.up);
//        //Debug.Log("DEGREES: " + angleBetween);
//        //Debug.Log(Mathf.Sin(angleBetween* Mathf.Deg2Rad)); // = 1 when to right of user, -1 when to left
//        //Debug.Log(Mathf.Cos(angleBetween * Mathf.Deg2Rad)); // = 1 when in front of user, -1 when behind
//    }


//    private void ActivateTrigger() {
//        distance = Vector3.Distance(transform.position, playerTrans.position);
//        distance = Mathf.Abs(distance);
//        distance = 1 / distance;

//        Debug.Log("Remote BUZZ: " + distance);
//        StartCoroutine("Fade");
//    }



//    IEnumerator Fade() {

//        float value = 100.00f;

//        for (int i = 0; i < 11; i++) {
//            value = 100.00f;

//            if (i == (int)HapticManager.HapticDevice.Head) {
//                SetAllPads(devices[i], value);
//            } else if (i == (int)HapticManager.HapticDevice.VestFront) {
//                value = 50 + 50*Mathf.Cos(angleBetween * Mathf.Deg2Rad);

//                SetAllPads(devices[i], value);
//            } else if (i == (int)HapticManager.HapticDevice.VestBack) {
//                value = 50 + 50*-1*Mathf.Cos(angleBetween * Mathf.Deg2Rad);

//                SetAllPads(devices[i], value);
//            } else if (i == (int) HapticManager.HapticDevice.LeftArm || i == (int)HapticManager.HapticDevice.LeftFoot || i == (int)HapticManager.HapticDevice.LeftForearm || i == (int)HapticManager.HapticDevice.LeftHand) {
//                value = 50 + 50*-1*Mathf.Sin(angleBetween * Mathf.Deg2Rad);

//                SetAllPads(devices[i], value);
//            } else if (i == (int)HapticManager.HapticDevice.RightArm || i == (int)HapticManager.HapticDevice.RightFoot || i == (int)HapticManager.HapticDevice.RightForearm || i == (int)HapticManager.HapticDevice.RightHand) {
//                value = 50 + 50*Mathf.Sin(angleBetween * Mathf.Deg2Rad);

//                SetAllPads(devices[i], value);
//            }

//            Debug.Log(i + " " + value);
//        }

//        for (int i = 0; i < 11; i++) {
//            devices[i].Play();
//        }

//        yield return new WaitForSeconds(0.5f);
//    }


//    void SetAllPads(TactSource device, float value) {
//        for (int j = 0; j < 20; j++) {
//            device.DotPoints[j] = (byte)value;
//        }
//    }

//    public void TriggerCall() {
//        ActivateTrigger();
//    }
//}
