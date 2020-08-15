using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Use this script on a rigid body with an audio source component to play an audio clip
// if the object has any nonzero velocity 

public class PlayAudioOnVelocity : MonoBehaviour
{
    public AudioClip audioClip;
    AudioSource audioSource;
    public Rigidbody rb;

    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
    }

    
    void FixedUpdate()
    {
        if(rb.velocity.magnitude >= 0.01 && !audioSource.isPlaying) {
            audioSource.PlayOneShot(audioClip, 0.7f);
        }
    }
}
