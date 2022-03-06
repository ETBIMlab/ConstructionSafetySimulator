using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineController : MonoBehaviour
{
    public PlayableDirector m_playableDirector;
    //public float timeDeltaForReset = 1f / 60f;
    private bool isPlaying = true;

    private void Start()
    {
        if(!m_playableDirector)
        {
            m_playableDirector = GetComponent<PlayableDirector>(); //for now get the first instance of a timeline if none set
        }
    }

    public void PlayDirector()
    {
        Debug.Log("<color=green>Playing the timeline.</color>");
        m_playableDirector.Play();
        isPlaying = true;
    }

    public void StopDirector()
    {
        Debug.Log("<color=green>Stopping the timeline.</color>");
        m_playableDirector.Stop();
        isPlaying = false;
    }

    public void FreezeTimeline()
    {
        Debug.Log("<color=green>Freezing the timeline.</color>");
        m_playableDirector.time = m_playableDirector.time;
        m_playableDirector.initialTime = m_playableDirector.time;
        m_playableDirector.Evaluate();
        m_playableDirector.Pause();
        isPlaying = false;
    }

    public void UnFreezeTimeline()
    {
        Debug.Log("<color=green>Unfreezing the timeline.</color>");
        m_playableDirector.Resume();
        isPlaying = true;         
    }

    public void ToggleFreezeTimeline()
    {
        if(isPlaying)
        {
            FreezeTimeline();
        } else
        {
            UnFreezeTimeline();
        }
    }

    public void ToggleTimelineState()
    {
        if (isPlaying)
        {
            StopDirector();
        }
        else
        {
            PlayDirector();
        }
    }

    public void ResetTimeline()
    {
        Debug.Log("Resetting the current timeline.");

        //go back to the beginning of the timeline
        m_playableDirector.time = 0;
        m_playableDirector.initialTime = 0;
        m_playableDirector.Evaluate();

        //stop the timeline
        StopDirector();
        isPlaying = false;

        //reset fade to white here!

    }

}
