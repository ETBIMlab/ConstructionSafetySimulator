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

    private static bool firstUse = true;

    public static void updateTimeRecords(string message) {
        System.DateTime currentTime = System.DateTime.Now;
        System.TimeSpan currentSceneDuration = currentTime.Subtract(sceneStartTime);
        System.TimeSpan duration = currentTime.Subtract(startTime);

        string currentTimeMessage = message + " Timestamp: " + currentTime.ToShortTimeString() + " Total Duration: " + duration.ToString("mm':'ss") + " Scene Duration:  " + currentSceneDuration.ToString("mm':'ss");
        // string durationMessage = message + " " + duration.TotalMinutes + ":" + duration.TotalSeconds;
        timeRecords.Add(currentTimeMessage);
        Debug.Log(currentTimeMessage);
        //durationRecords.Add(durationMessage);
        if(firstUse == true){
             using(System.IO.StreamWriter writetext = new System.IO.StreamWriter("Time Stamp Log.txt", append: false))
        {
            writetext.WriteLine(string.Empty);
        }
            firstUse = false;
        }
        using(System.IO.StreamWriter writetext = new System.IO.StreamWriter("Time Stamp Log.txt", append: true))
        {
            writetext.WriteLine(currentTimeMessage);
        }

    }

    public static void printRecords() {

    }
}
