using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class TimelineResetManager : MonoBehaviour
{
    private PlayableDirector _playableDirector;

    private void Start()
    {
        _playableDirector = GetComponent<PlayableDirector>();
    }

    public void PlayPlayable(PlayableAsset playableAsset)
    {
        _playableDirector.Play(playableAsset);
    }

    public void ResetCurrentPlayable()
    {
        _playableDirector.Stop();
        _playableDirector.time = 0;
        _playableDirector.Evaluate();
    }
}
