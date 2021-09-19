using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    /* 
     *  Static Functions
     */

    private static AudioManager instance;

    public static void PlaySoundEffect(AudioClip clip, float volume = 1)
    {
        if (instance == null)
            Debug.LogError("There is no audio manager in the scene to play audio from");

        else if (instance.SoundEffectSource == null)
            Debug.LogError("The audio manager in the scene does not have a SoundEffects source set");

        else
        {
            instance.SoundEffectSource.PlayOneShot(clip, volume);
        }
    }

    public static void PlaySoundEffect(string soundName, float volume = 1)
    {
        NamedSound sound = instance.Sounds.Find(x => x.name == soundName);
        if (sound == null)
        {
            Debug.LogWarning("Sound with the name: '" + soundName + "' was not found in the list of sounds on the active AudioManager", instance);
        }
    }

    /* 
     *  Manager Data
     */

    public AudioSource SoundEffectSource;
    [SerializeField]
    public List<NamedSound> Sounds;

    private void OnValidate()
    {
        for (int i = 0; i < Sounds.Count; i++)
            if (!string.IsNullOrEmpty(Sounds[i].name))
                for (int j = i + 1; j < Sounds.Count; j++)
                    if (Sounds[i].name == Sounds[j].name)
                        Debug.LogWarning("Multiple sounds in the AudioManager '" + name + "' share the same name. All but one of the sounds will be remove on application start.", this);
    }

   
    private void Start()
    {
        // Make sure all the named clips have a name, have a audioclip, and are unique.
        List<NamedSound> singles = new List<NamedSound>();
        foreach (NamedSound sound in Sounds)
        {
            if (string.IsNullOrEmpty(sound.name))
                Debug.LogWarning("Sounds in the AudioManager '" + name + "' has an empty name. Removing the sound.", this);

            else if (sound.audioClip == null)
                Debug.LogWarning("The sound '" + sound.name + "' in the AudioManager '" + name + "' has an empty name. Removing the sound.", this);

            else if (singles.Find(x => x.name == sound.name) != null)
                Debug.LogWarning("Multiple sounds with name '" + sound.name + "' in the AudioManager '" + name + "' share the same name. Removing one duplicate.", this);

            else
                singles.Add(sound);
        }

        Sounds = singles;
        //------------------------------------------------------------------------------

    }

    private void OnEnable()
    {
        if (instance != null)
        {
            Debug.LogError("There can only be one AudioManager active at a time. Current AudioManager: " + instance.name, this);
            this.enabled = false;
            return;
        }

        instance = this;
    }

    private void OnDisable()
    {
        if (instance = this)
            instance = null;
    }

    /*
     *  Play a sound using a UnityEvent
     */

    public void PlaySoundEffect(AudioClip clip)
    {
        PlaySoundEffect(clip, 1.0f);
    }

    public void PlaySoundEffect(string soundName)
    {
        PlaySoundEffect(soundName, 1.0f);
    }

    [Serializable]
    public class NamedSound
    {
        public string name;
        public AudioClip audioClip;
    }

}
