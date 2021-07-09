using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ResetManager))]
public class ResetComponent : MonoBehaviour
{
    public const int WarningRecordPerSecondCap = 225;
    public const int WarningRecordMemoryCap = WarningRecordPerSecondCap * 40;

    [SerializeField]
    private float SnapshotInterval = 1f / 15f;
    [SerializeField]
    private PositionRotationResetRecorder[] TransformRecorders;
    [SerializeField]
    private AnimatorResetRecorder[] AnimatorRecorders;

    private float currentCounter = 0;
    private int recordedValues = 0;

    public void LogPreformanceWarnings()
    {
        if (Mathf.CeilToInt(ResetManager.MaxReverseTime / SnapshotInterval) * CountRecorders() > WarningRecordMemoryCap)
            Debug.LogWarning("Recorder settings are recording a total recorder memory of '" + Mathf.CeilToInt(ResetManager.MaxReverseTime / SnapshotInterval) * CountRecorders() + "' values at once, which is above the recommended max of '" + WarningRecordMemoryCap + "'", this);

        if (Mathf.CeilToInt(1 / SnapshotInterval) * CountRecorders() > WarningRecordPerSecondCap)
            Debug.LogWarning("Recorder settings are recording '" + (Mathf.CeilToInt(1 / SnapshotInterval) * CountRecorders()) + "' recorders per second, which is above the recommended max of '" + WarningRecordPerSecondCap + "'", this);
    }

    // Update is called once per frame
    public void UpdateRecording()
    {
        if (!ResetManager.ReversingTime)
        {
            currentCounter += Time.deltaTime;
            while (currentCounter > SnapshotInterval)
            {
                currentCounter -= SnapshotInterval;

                RecordValues();
                if (recordedValues > ResetManager.MaxReverseTime / SnapshotInterval)
                    DiscardValues();
            }
        }
    }

    public void ResetTime(float time)
    {
        currentCounter = 0;

        while (time > 0)
        {
            if (recordedValues == 1)
            {
                Debug.LogWarning("ReverseTime parameter 'time' is further back than what is recorded. Rewound as much as possible", this);
                break;
            }
            DiscardValues(false);
            time -= SnapshotInterval;
        }
        SetToLastValues();
    }

    public void RewindTimeOverTime(float time, float scaleSpeed)
    {
        StopAllCoroutines();
        StartCoroutine(StartRewindTime(time, scaleSpeed));
    }

    private IEnumerator StartRewindTime(float time, float playbackScale)
    {
        ResetManager.ReversingTime = true;
        currentCounter = 0;

        foreach (var recorder in AnimatorRecorders)
            recorder.SetAnimatorSpeed(0);

        float tempCounter = 0;
        while (time > 0)
        {
            time -= Time.deltaTime * playbackScale;
            tempCounter += Time.deltaTime * playbackScale;
            while (tempCounter > SnapshotInterval)
            {
                tempCounter -= SnapshotInterval;

                if (recordedValues == 1)
                    break;

                DiscardValues(false);
            }
            SetToLastValues();
            if (recordedValues == 1)
            {
                ResetManager.LogTimeTooLong();
                break;
            }
            yield return null;
        }

        RewindFinished();
    }

    private void RewindFinished()
    {
        foreach (AnimatorResetRecorder recorder in AnimatorRecorders)
            recorder.SetAnimatorSpeed(1);

        ResetManager.ReversingTime = false;
    }

    public void ForceStopRewind()
    {
        RewindFinished();
        StopAllCoroutines();
    }

    private void RecordValues()
    {
        recordedValues++;

        foreach (var recorder in TransformRecorders)
            recorder.RecordValue();

        foreach (var recorder in AnimatorRecorders)
            recorder.RecordValue();

    }

    private void DiscardValues(bool fromFront = true)
    {
        Debug.Assert(recordedValues > 0, "ResetManager is trying to discard values past what has been recorded!", this);

        recordedValues--;

        foreach (var recorder in TransformRecorders)
            recorder.DiscardValue(fromFront);

        foreach (var recorder in AnimatorRecorders)
            recorder.DiscardValue(fromFront);
    }

    public void SetToLastValues()
    {
        foreach (var recorder in TransformRecorders)
            recorder.SetToLastValue();

        foreach (var recorder in AnimatorRecorders)
            recorder.SetToLastValue();
    }

    public int CountRecorders()
    {
        return TransformRecorders.Length + AnimatorRecorders.Length;
    }



    [Serializable]
    public class PositionRotationResetRecorder : ResetRecorder
    {
        [SerializeField]
        private Transform RecordedObject;
        [SerializeField]
        private bool UseLocalSpace;

        private List<Tuple<Vector3, Quaternion>> history = new List<Tuple<Vector3, Quaternion>>();

        public void RecordValue()
        {
            if (UseLocalSpace)
                history.Add(new Tuple<Vector3, Quaternion>(RecordedObject.localPosition, RecordedObject.localRotation));
            else
                history.Add(new Tuple<Vector3, Quaternion>(RecordedObject.position, RecordedObject.rotation));
        }

        public void DiscardValue(bool fromFront)
        {
            if (fromFront)
                history.RemoveAt(0);
            else
                history.RemoveAt(history.Count - 1);
        }


        public void SetToLastValue()
        {
            Tuple<Vector3, Quaternion> value = history[history.Count - 1];

            if (UseLocalSpace)
            {
                RecordedObject.localPosition = value.Item1;
                RecordedObject.localRotation = value.Item2;
            }
            else RecordedObject.SetPositionAndRotation(value.Item1, value.Item2);
        }
    }

    /// <summary>
    /// Animator Speed is expected to be set to 0.
    /// </summary>
    [Serializable]
    public class AnimatorResetRecorder : ResetRecorder
    {
        [SerializeField]
        private Animator RecordedObject;
        [SerializeField]
        private int RecordLayer;

        /// <summary>
        /// Recorded history of the Animator in a Tuple: 
        ///  - AnimatorStateInfo stores the current clip that was playing and current playback time.
        ///  - The bool keeps track if the animator is active.
        /// </summary>
        private List<Tuple<AnimatorStateInfo, bool>> history = new List<Tuple<AnimatorStateInfo, bool>>();

        public void DiscardValue(bool fromFront)
        {
            if (fromFront)
                history.RemoveAt(0);
            else
                history.RemoveAt(history.Count - 1);
        }

        public void RecordValue()
        {
            history.Add(new Tuple<AnimatorStateInfo, bool>(RecordedObject.GetCurrentAnimatorStateInfo(RecordLayer), RecordedObject.enabled));
        }

        public void SetToLastValue()
        {
            Tuple<AnimatorStateInfo, bool> value = history[history.Count - 1];
            if (value.Item2)
                RecordedObject.Play(value.Item1.fullPathHash, RecordLayer, value.Item1.normalizedTime);
            else
            {
                // If the animator is disabled (animation finished), we will assume that this frame is the first frame of the animation.
                // If instead the animator is disabled and we are on the last frame (because the animation has not finished), we will skip updating the animator (see bellow).
                RecordedObject.Play(value.Item1.fullPathHash, RecordLayer, 0.0f);
            }
            // Update forces the animation to display the current frame in the scene. This is helpful if:
            //  - the animator becomes disabled and is no longer able to update to recorded state
            //  - the timeScale is set to 0, thus the updates will not be called.
            // We want to skip update if the animator was false, and is false now (prevents animations that have finished and disabled from updating the starting frame as it is set above^^). 
            if (RecordedObject.enabled || value.Item2)
            {
                RecordedObject.Update(0.0f);
            }
            RecordedObject.enabled = value.Item2;
        }

        public void SetAnimatorSpeed(int speed)
        {
            RecordedObject.speed = speed;
        }
    }

    public interface ResetRecorder
    {
        public void RecordValue();

        public void DiscardValue(bool fromFront);

        public void SetToLastValue();
    }
}
