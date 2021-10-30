using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneLoadedTrigger : MonoBehaviour
{

    public UnityEvent OnSceneFinishLoading;

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
        OnSceneFinishLoading.Invoke();
    }

    public void ForceQuitApp()
    {
        Application.Quit();
    }
}
