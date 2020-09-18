//using Bhaptics.Tact.Unity;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class HapticManager : MonoBehaviour
//{
//    public enum HapticDevice {Head, VestFront, VestBack, RightArm, LeftArm, LeftHand, RightHand, LeftFoot, RightFoot, RightForearm, LeftForearm};

//    TactSource[] devices;
//    bool[] deviceStatus;

//    public void VibrateDevice(bool[] toActivate) {
//        StartCoroutine("Fade");
//    }



//    IEnumerator Fade() {

//        for(int i = 0; i < 11; i++) {
//            if (deviceStatus[i]) {
//                SetAllPads(devices[i], 100);
//            } else {
//                SetAllPads(devices[i], 0);
//            }
//        }

//        for (int i = 0; i < 11; i++) {
//            devices[i].Play();
//        }
        
//        yield return new WaitForSeconds(0.5f);
//    }


//    void SetAllPads(TactSource device, float value) {
//        for (int j = 0; j < 20; j++) {
//            device.DotPoints[0] = (byte)value;
//        }
//    }
//}
