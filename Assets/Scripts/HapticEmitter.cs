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
    public class DynamicHapticEmitterClip
    {
        public SimpleHapticClip clip;
        public PositionTag positionTag = PositionTag.Default;

        [NonSerialized] public float lastIntensity;
    }

    [Serializable]
    public class StaticHapticEmitterClip
    {
        public HapticClip clip;
        public PositionTag positionTag = PositionTag.Default;
    }

    [Serializable]
    public class DynamicHapticGroup
    {
        public AnimationCurve positionIntensityCurve;
        public AnimationCurve timeIntensityCurve;
        public DynamicHapticEmitterClip[] emitterClip;
    }

    [Serializable]
    public class StaticHapticGroup
    {
        public AnimationCurve positionIntensityCurve;
        public StaticHapticEmitterClip[] emitterClip;
    }

    [Header("Haptic Info")]
    public DynamicHapticGroup[] dynamicHaptics;
    public StaticHapticGroup[] staticHaptics;

    [Header("Emitter Info")]
    public DoublePlayMode doublePlayMode = DoublePlayMode.Reset;
    [Range(0,5)]
    public float intensityMultiplier = 1;
    public bool playOnAwake = false;
    public bool loop = false;
    //public float hapticsUpdateTimeStep;
    [Range(0.01f, 10)]
    public float totalDuration = 1.5f;
    public bool scaleDurations = false;

    private float currentDuration;
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
            foreach (var staticHapticGroup in staticHaptics)
            {
                foreach (var staticEmitter in staticHapticGroup.emitterClip)
                {
                    float intensity = intensityMultiplier;
                    if (staticEmitter.positionTag != PositionTag.None)
                        intensity = staticHapticGroup.positionIntensityCurve.Evaluate(HapticListener.GetDistance(transform.position, staticEmitter.positionTag));
                    if (intensity > 0)
                    {
                        if (scaleDurations)
                            staticEmitter.clip.Play(
                                intensity: intensity,
                                duration: totalDuration
                                );
                        else
                            staticEmitter.clip.Play(
                                intensity: intensity
                                );
                    }
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
        foreach (var group in dynamicHaptics)
            foreach (var emitter in group.emitterClip)
                emitter.clip.Stop();

        foreach (var group in staticHaptics)
            foreach (var emitter in group.emitterClip)
                emitter.clip.Stop();
    }

    private void Update()
    {
        if (currentDuration > 0)
        {
            foreach (var dynamicHapticGroup in dynamicHaptics)
            {
                foreach (var dynamicHaptic in dynamicHapticGroup.emitterClip)
                {
                    float intensity = intensityMultiplier;
                    if (dynamicHaptic.positionTag != PositionTag.None)
                        intensity *= dynamicHapticGroup.positionIntensityCurve.Evaluate(HapticListener.GetDistance(transform.position, dynamicHaptic.positionTag));

                    if (scaleDurations)
                        intensity *= dynamicHapticGroup.timeIntensityCurve.Evaluate(1 - currentDuration / totalDuration);
                    else
                        intensity *= dynamicHapticGroup.timeIntensityCurve.Evaluate(1 - (((dynamicHaptic.clip.TimeMillis / 1000f) - (totalDuration - currentDuration)) / (dynamicHaptic.clip.TimeMillis / 1000f)));


                    if (dynamicHaptic.clip.IsPlaying() && dynamicHaptic.lastIntensity != dynamicHaptic.clip.currentPlayIntensity && intensity < dynamicHaptic.clip.currentPlayIntensity)
                        continue;

                    if (intensity > 0)
                    {
                        if (scaleDurations)
                        {
                            dynamicHaptic.clip.Play(
                                intensity: intensity,
                                duration: currentDuration + (loop ? LoopPadding : 0)
                                );
                            dynamicHaptic.lastIntensity = intensity;
                            continue;
                        }
                        else
                        {
                            float duration = (dynamicHaptic.clip.TimeMillis / 1000f) - (totalDuration - currentDuration) + (loop ? LoopPadding : 0);
                            if (duration > 0)
                            {
                                dynamicHaptic.clip.Play(
                                    intensity: intensity,
                                    duration: duration
                                    );
                                dynamicHaptic.lastIntensity = intensity;
                                continue;
                            }
                        }
                    }
                    if (dynamicHaptic.clip.IsPlaying())
                    {
                        dynamicHaptic.clip.Stop();
                        dynamicHaptic.lastIntensity = 0.0001f;
                    }
                }

                currentDuration -= Time.deltaTime;
                if (currentDuration <= 0)
                    if (loop || queue)
                        PlayClips();
            }
        }
    }


}
