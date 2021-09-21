using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeRecorder : MonoBehaviour
{
    [System.NonSerialized]
    public static System.DateTime startTime = System.DateTime.Now;

    [System.NonSerialized]
    public static System.DateTime sceneStartTime = System.DateTime.Now;
    public static List<string> timeRecords = new List<string>();
    //public static List<string> durationRecords = new List<string>();

    public static void updateTimeRecords(string message) {
        System.DateTime currentTime = System.DateTime.Now;
        System.TimeSpan currentSceneDuration = currentTime.Subtract(sceneStartTime);
        System.TimeSpan duration = currentTime.Subtract(startTime);

        string currentTimeMessage = message + " Timestamp: " + currentTime.ToShortTimeString() + " Total Duration: " + duration.TotalMinutes + ":" + duration.TotalSeconds + " Scene Duration:  " + currentSceneDuration.TotalMinutes + ":" + duration.TotalSeconds;
        // string durationMessage = message + " " + duration.TotalMinutes + ":" + duration.TotalSeconds;
        timeRecords.Add(currentTimeMessage);
        Debug.Log(currentTimeMessage);
        //durationRecords.Add(durationMessage);

    }

    public static void printRecords() {

    }
}
