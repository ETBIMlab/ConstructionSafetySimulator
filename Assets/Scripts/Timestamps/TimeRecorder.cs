using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeRecorder
{
	public const string RECORD_FILE = "Saved/Time Stamp Log.txt";
	public const bool CLEAR_ON_LOAD = true;

	[System.NonSerialized]
	public static System.DateTime startTime;
	[System.NonSerialized]
	public static System.DateTime sceneStartTime;

	public static List<string> timeRecords = new List<string>();

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void InitailizeFile()
	{
		startTime = System.DateTime.Now;
		sceneStartTime = System.DateTime.Now;

		UpdateTimeRecords("Construction Saftey Simulator has begun", CLEAR_ON_LOAD);
	}

	public static void UpdateTimeRecords(string message, bool clearlog = false)
	{
		string currentTimeMessage = CreateTimeMessage(message);

		Debug.Log("[Logging TimeRecorder]:: " + currentTimeMessage);

		using(System.IO.StreamWriter writetext = new System.IO.StreamWriter(Application.dataPath + "/../" + RECORD_FILE, append: !clearlog))
		{
			writetext.WriteLine(currentTimeMessage);
		}
	}

	private static string CreateTimeMessage(string message)
	{
		System.DateTime currentTime = System.DateTime.Now;
		System.TimeSpan currentSceneDuration = currentTime.Subtract(sceneStartTime);
		System.TimeSpan duration = currentTime.Subtract(startTime);

		return string.Format("Time: {1}, App Dur.: {2}, Scene Dur.: {3}\n\t{0}",
			message, currentTime.ToShortTimeString(), duration.ToString("mm':'ss"), currentSceneDuration.ToString("mm':'ss"));
	}

	public static void printRecords() {

	}
}
