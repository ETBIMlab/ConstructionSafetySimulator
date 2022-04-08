using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class TimeRecorderSceneInterface : MonoBehaviour
{
	public void Awake()
	{
		SceneManager.sceneLoaded += SceneLoaded;
	}

	public void OnDestroy()
	{
		SceneManager.sceneLoaded -= SceneLoaded;
	}

	private void SceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (mode == LoadSceneMode.Single) UpdateSceneTime();

		UpdateTimeRecords("Entered Scene: " + scene.name);
	}

	public void UpdateTimeRecords(string message)
	{
		TimeRecorder.UpdateTimeRecords(message);
	}

	public void UpdateSceneTime()
	{
		TimeRecorder.sceneStartTime = System.DateTime.Now;
	}
}
