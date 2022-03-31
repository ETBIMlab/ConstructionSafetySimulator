using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class FreezeFrameController : MonoBehaviour
{
    public UnityEvent OnStartFreezeFrame;
    public UnityEvent OnStopFreezeFrame;

    
    public float lerpDuration = 3;
    public float startValue = 1;
    public float endValue = 0.001f;

    private float timeElapsed = 0;
    private float valueToLerp = 0;

    public bool slowOn = false;
    public bool reset = false;

    public void StartFreezeFrame()
    {
        if (TimeScaleLock.timeScaleWritable)
            OnStartFreezeFrame.Invoke();

        reset = false;
        timeElapsed = 0;
        valueToLerp = 0;
        SetTimeScale(1);
        slowOn = true;
    }

    public void StartFreezeFrame(float duration)
    {
        lerpDuration = duration;
        StartFreezeFrame();
    }

    public void StopFreezeFrame()
    {
        if (TimeScaleLock.timeScaleWritable)
            OnStopFreezeFrame.Invoke();

        slowOn = false;
        reset = false;
        timeElapsed = 0;
        valueToLerp = 0;
        SetTimeScale(1);
    }

    public void FullStop()
    {
        SetTimeScale(0.0001f); //full stop the timeline
    }

    private void Update()
    {
        if(slowOn && (timeElapsed < lerpDuration))
        {
            valueToLerp = Mathf.Lerp(startValue, endValue, timeElapsed / lerpDuration);
            SetTimeScale(valueToLerp); //set the current time after lerp
            timeElapsed += Time.deltaTime;
        }

        if(reset)
        {
            timeElapsed = 0;
            valueToLerp = 0;
            SetTimeScale(1); //reset to normal time
        }
    }

    public void SetTimeScale(float timeScale)
    {
        if (TimeScaleLock.timeScaleWritable)
            Time.timeScale = timeScale;
        else
        {
            // This is kind of a desired functionality (try to pause when unable), so commented out the debug to avoid unneeded logs.
            Debug.Log("Could not change time scale as the time scale is being locked by another resource.", this);
        }
    }

    public void OnDestroy()
    {
        TimeScaleLock.timeScaleWritable = true;
        Time.timeScale = 1;
    }

    /// <summary>
    /// Toggles if freeze frame is happening. Note that if the time scale is not frozen, then the application is considered playing and the freeze frame is STARTED. If the freeze frame is stopped after this, the time scale will be set to 1 and not what the time scale was before.
    /// </summary>
    public void ToggleFreezeFrame()
    {
        if (Time.timeScale != 1)
            StopFreezeFrame();
        else
            StartFreezeFrame();
    }

}

public static class TimeScaleLock
{
    [System.NonSerialized]
    public static bool timeScaleWritable = true;
}
