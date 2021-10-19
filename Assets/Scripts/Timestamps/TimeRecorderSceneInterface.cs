using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeRecorderSceneInterface : MonoBehaviour
{
    public void UpdateTimeRecords(string message) { TimeRecorder.updateTimeRecords(message); }
    public void UpdateSceneTime() { TimeRecorder.sceneStartTime = System.DateTime.Now;  }
}
