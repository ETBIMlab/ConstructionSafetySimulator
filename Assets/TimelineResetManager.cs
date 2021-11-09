using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineResetManager : MonoBehaviour
{
    private PlayableDirector m_playableDirector;
    public float timeDeltaForReset = 1f / 60f;
    private bool isPlaying;

    private void Start()
    {
        m_playableDirector = GetComponent<PlayableDirector>();
    }

    public void PlayDirector(PlayableDirector playableDirector)
    {
        m_playableDirector = playableDirector;
        m_playableDirector.Play();
        isPlaying = true;
    }

    public void StopDirector()
    {
        m_playableDirector.Stop();
    }

    public void FreezeTimeline()
    {
        if (isPlaying)
            m_playableDirector.Pause();
    }

    public void UnFreezeTimeline()
    {
        if (isPlaying)
            m_playableDirector.Resume();
    }

    public void ResetCurrentPlayable()
    {
        isPlaying = false;
        m_playableDirector.Stop();
        
        while(m_playableDirector.time > 0)
        {
            m_playableDirector.time -= timeDeltaForReset;
            m_playableDirector.Evaluate();
        }

        m_playableDirector.time = 0;
        m_playableDirector.Evaluate();
    }
}
