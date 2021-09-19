using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Add this component to a GameObject with an AudioSource to allow for fading in and out the volume.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class FadeAudio : MonoBehaviour
{
    private AudioSource _audioSource;
    private float awakeVolume;

    public bool AutomaticDisabling = true;


    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        awakeVolume = _audioSource.volume;
    }


    public void FadeIn(float duration)
    {
        _audioSource.enabled = true;
        _audioSource.volume = 0;
        StartFadeAbsolute(duration, awakeVolume);
    }

    public void FadeOut(float duration)
    {
        StartFadeAbsolute(duration, 0);
    }

    /// <summary>
    /// Linear transition between current and target volume that lasts 'duration' seconds (works on attached audio source). If two fades are overlapping, only the last fade call will execute.
    /// </summary>
    /// <param name="duration">The time that the fade should last</param>
    /// <param name="targetVolume">The volume that the AudioSource should transition to</param>
    /// <returns></returns>
    private void StartFadeAbsolute(float duration, float targetVolume)
    {
        StopAllCoroutines();
        StartCoroutine(FadeAbsolute(duration, targetVolume));
    }

    public IEnumerator FadeAbsolute(float duration, float targetVolume)
    {
        float currentTime = 0;
        float initVolume = _audioSource.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            _audioSource.volume = Mathf.Lerp(initVolume, targetVolume, currentTime / duration);
            yield return null;
        }

        if (AutomaticDisabling && _audioSource.volume <= 0)
        {
            _audioSource.enabled = false;
        }

        yield break;
    }

    public void StartFadeRate(float fadeRate, float targetVolume)
    {
        StartFadeAbsolute(fadeRate * Mathf.Abs(_audioSource.volume - targetVolume), targetVolume);
    }
}
