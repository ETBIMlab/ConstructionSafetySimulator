using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class FreezeFrameController : MonoBehaviour
{
    public UnityEvent OnStartFreezeFrame;
    public UnityEvent OnStopFreezeFrame;

    public void StartFreezeFrame()
    {
        if (TimeScaleLock.timeScaleWritable)
            OnStartFreezeFrame.Invoke();

        SetTimeScale(0);
    }

    public void StopFreezeFrame()
    {
        if (TimeScaleLock.timeScaleWritable)
            OnStopFreezeFrame.Invoke();

        SetTimeScale(1);
    }

    public void SetTimeScale(int timeScale)
    {
        if (TimeScaleLock.timeScaleWritable)
            Time.timeScale = timeScale;
        else
        {
            // This is kind of a desired functionality (try to pause when unable), so commented out the debug to avoid unneeded logs.
            //Debug.Log("Could not change time scale as the time scale is being locked by another resource.", this);
        }
    }

    /// <summary>
    /// Toggles if freeze frame is happening. Note that if the time scale is not frozen, then the application is considered playing and the freeze frame is STARTED. If the freeze frame is stopped after this, the time scale will be set to 1 and not what the time scale was before.
    /// </summary>
    public void ToggleFreezeFrame()
    {
        if (Time.timeScale == 0)
            StopFreezeFrame();
        else
            StartFreezeFrame();
    }

}

public static class TimeScaleLock
{
    public static bool timeScaleWritable = true;
}
