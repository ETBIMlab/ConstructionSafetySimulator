using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

// AUTHOR: William Jenkins
// Script for playing footstep sound when moving forward in VR
// Built so that when the user touches the top of the touchpad they move forward
// plays the audioClip when touching forward on touchpad

public class AudioOnMove : MonoBehaviour
{
    public SteamVR_Action_Vector2 input;
    public AudioClip audioClip;
    AudioSource aSource;
    
    void Start()
    {
        aSource = GetComponent<AudioSource>();
    }

    // detect if the touchpad y-axis in being touched and plays sound
    void Update()
    {
        if(input.axis.y > 0 && !aSource.isPlaying) {
            aSource.PlayOneShot(audioClip, 0.7f);
        } else if (input.axis.y == 0 && aSource.isPlaying) {
            aSource.Stop();
        }
        
    }
    
}
