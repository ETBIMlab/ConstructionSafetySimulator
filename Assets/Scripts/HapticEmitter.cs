using System;
using System.Collections;
using System.Collections.Generic;
using Bhaptics.Tact.Unity;
using UnityEngine;

public class HapticEmitter : MonoBehaviour
{
    public const float LoopPadding = 0.1f;

    public enum DoublePlayMode
    {
        /// <summary>
        /// If haptics are played while already playing, reset all clips back to the start (stop the haptics and start them again).
        /// </summary>
        Reset,
        /// <summary>
        /// If haptics are played while already playing, skip playing the haptics (let the haptics that are already playing play out).
        /// </summary>
        Skip,
        /// <summary>
        /// 
        /// </summary>
        Queue
    }

    [Serializable]
    public class DynamicHapticEmitterSource
    {
        public AnimationCurve positionIntensityCurve;
        public AnimationCurve timeIntensityCurve;
        public SimpleHapticClip clip;
        public PositionTag positionTag = PositionTag.Default;

        //public float EvaluateBeyond(float time)
        //{
        //    intensityFalloffCurve.
        //}
    }

    [Serializable]
    public class StaticHapticEmitterSource
    {
        public AnimationCurve positionIntensityCurve;
        public HapticClip clip;
        public PositionTag positionTag = PositionTag.Default;

        //public float EvaluateBeyond(float time)
        //{
        //    intensityFalloffCurve.
        //}
    }

    public DynamicHapticEmitterSource[] dynamicHaptics;
    public StaticHapticEmitterSource[] staticHaptics;
    public DoublePlayMode doublePlayMode = DoublePlayMode.Reset;
    public float intensityMultiplier;
    public bool playOnAwake = false;
    public bool loop = false;
    //public float hapticsUpdateTimeStep;
    [Range(LoopPadding, 10)]
    public float totalDuration;
    public bool scaleDurations = true;

    public float currentDuration;

    private bool queue = false;

    private void Awake()
    {
        // Call get haptic to make sure that the haptics manager has a haptic device set.
        BhapticsManager.GetHaptic();

        // Make sure that BHaptics is hooked up to unity (ensure that there is a Bhaptics_Setup in the scene)
        var findObjectOfType = FindObjectOfType<Bhaptics_Setup>();
        if (findObjectOfType == null)
        {
            var go = new GameObject("[bhaptics]");
            go.SetActive(false);
            var setup = go.AddComponent<Bhaptics_Setup>();

            var config = Resources.Load<BhapticsConfig>("BhapticsConfig");

            if (config == null)
            {
                BhapticsLogger.LogError("Cannot find 'BhapticsConfig' in the Resources folder.");
            }

            setup.Config = config;

            go.SetActive(true);
        }

        currentDuration = -1;

        // Play on awake
        if (playOnAwake)
        {
            PlayClips();
        } else
        {
            this.enabled = false;
        }
    }

    public void PlayClips()
    {
        this.enabled = true;

        if (currentDuration > 0 && doublePlayMode != DoublePlayMode.Reset)
        {
            if (doublePlayMode == DoublePlayMode.Queue)
                queue = true;
        }
        else
        {
            foreach (var item in staticHaptics)
            {
                float intensity = intensityMultiplier;
                if (item.positionTag != PositionTag.None)
                    intensity = item.positionIntensityCurve.Evaluate(HapticListener.GetDistance(transform.position, item.positionTag));
                if (intensity > 0)
                {
                    if (scaleDurations)
                        item.clip.Play(
                            intensity: intensity,
                            duration: totalDuration
                            );
                    else
                        item.clip.Play(
                            intensity: intensity
                            );
                }
            }

            currentDuration = totalDuration;
            queue = false;
        }
    }

    public void ResumeDynamicClips()
    {
        this.enabled = true;
    }

    public void StopClips()
    {
        this.enabled = false;
        foreach (var item in dynamicHaptics)
            item.clip.Stop();

        foreach (var item in staticHaptics)
            item.clip.Stop();
    }

    private void Update()
    {
        if (currentDuration > 0)
        {
            foreach (var hes in dynamicHaptics)
            {
                float intensity = intensityMultiplier;
                if (hes.positionTag != PositionTag.None)
                    intensity *= hes.positionIntensityCurve.Evaluate(HapticListener.GetDistance(transform.position, hes.positionTag));

                if (scaleDurations)
                    intensity *= hes.timeIntensityCurve.Evaluate(1 - currentDuration/totalDuration);
                else
                    intensity *= hes.timeIntensityCurve.Evaluate(1 - (((hes.clip.TimeMillis/1000f) - (totalDuration - currentDuration))/ (hes.clip.TimeMillis / 1000f)));

                if (intensity > 0)
                {
                    if (scaleDurations)
                    {
                        hes.clip.Play(
                            intensity: intensity,
                            duration: currentDuration + (loop ? LoopPadding : 0)
                            );
                        continue;
                    }
                    else
                    {
                        float duration = (hes.clip.TimeMillis / 1000f) - (totalDuration - currentDuration) + (loop ? LoopPadding : 0);
                        if (duration > 0)
                        {
                            hes.clip.Play(
                                intensity: intensity,
                                duration: duration
                                );
                            continue;
                        }
                    }
                }
                hes.clip.Stop();
            }

            currentDuration -= Time.deltaTime;
            if (currentDuration <= 0)
                if (loop || queue)
                    PlayClips();
        }
    }


}
