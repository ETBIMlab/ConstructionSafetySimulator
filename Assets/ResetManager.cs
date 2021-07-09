using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResetManager : MonoBehaviour
{
    private static ResetManager instance;
    private static ResetManager Instance
    {
        get
        {
            Debug.Assert(instance != null, "There are no active reset managers on the scene.");
            return instance;
        }
        set => instance = value;
    }

    public static float MaxReverseTime { get => Instance.m_MaxReverseTime; set => Instance.m_MaxReverseTime = value; }
    public static float DefaultPlaybackScale { get => Instance.m_DefaultPlaybackScale; set => Instance.m_DefaultPlaybackScale = value; }

    [SerializeField]
    private float m_MaxReverseTime = 40f;
    [SerializeField]
    private float m_DefaultPlaybackScale = 1.0f;
    [SerializeField]
    private bool OverrideFreezeFrame = true;

    public UnityEvent OnResetTime;
    public UnityEvent OnRewindStart;
    public UnityEvent OnRewindFinished;


    private List<ResetComponent> resetComponents;

    private void Start()
    {
        resetComponents = new List<ResetComponent>();
        resetComponents.AddRange(GetComponents<ResetComponent>());
        resetComponents.ForEach(x => x.LogPreformanceWarnings());

        OnRewindStart.AddListener(() =>
        {
            if (!TimeScaleLock.timeScaleWritable)
                Debug.LogWarning("ResetManager is not set up to be compatible if the timescalelock is taken.");

            TimeScaleLock.timeScaleWritable = false;
            previousTimeScale = Time.timeScale;
            Time.timeScale = 1;
        });

        OnRewindFinished.AddListener(() =>
        {
            TimeScaleLock.timeScaleWritable = true;
            Time.timeScale = previousTimeScale;
        });
    }

    private void Update()
    {
        foreach (var rc in resetComponents)
        {
            rc.UpdateRecording();
        }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("There should only be one ResetManager active at a time. Current ResetManager: " + instance.name + ". Keeping both will likely cause even more preformance issues.", this);
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance = this)
            Instance = null;
    }

    private static bool m_ReversingTime = false;
    private static float previousTimeScale = 1.0f;

    public static bool ReversingTime
    {
        get
        {
            return m_ReversingTime;
        }
        set
        {
            if (value != m_ReversingTime)
            {
                if (Instance.OverrideFreezeFrame)
                {
                    if (value)
                        Instance.OnRewindStart.Invoke();
                    else
                        Instance.OnRewindFinished.Invoke();
                }

                m_ReversingTime = value;
            }
            
        }
    }

    [SerializeField]
    public static void ResetTime(float time)
    {
        logged = false;

        Instance.OnResetTime.Invoke();

        foreach (var rc in Instance.resetComponents)
            rc.ResetTime(time);
    }

    public static void RewindTimeOverTime(float time)
    {
        RewindTimeOverTime(time, DefaultPlaybackScale);
    }

    public static void RewindTimeOverTime(float time, float scaleSpeed)
    {
        logged = false;

        foreach (var rc in Instance.resetComponents)
            rc.RewindTimeOverTime(time, scaleSpeed);
    }

    public static void StopAllReverse()
    {
        foreach (var rc in Instance.resetComponents)
            rc.ForceStopRewind();
    }

    private static bool logged;
    internal static void LogTimeTooLong()
    {
        if (!logged)
            Debug.Log("ReverseTime parameter 'time' is further back than what is recorded. Rewound as much as possible");

        logged = true;
    }
}
