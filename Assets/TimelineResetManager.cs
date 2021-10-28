using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineResetManager : MonoBehaviour
{
    private PlayableDirector m_playableDirector;

    private void Start()
    {
        m_playableDirector = GetComponent<PlayableDirector>();
    }

    public void PlayDirector(PlayableDirector playableDirector)
    {
        m_playableDirector = playableDirector;
        m_playableDirector.Play();
    }

    public void ResetCurrentPlayable()
    {
        m_playableDirector.Stop();
        m_playableDirector.time = 0;
        m_playableDirector.Evaluate();
    }
}
