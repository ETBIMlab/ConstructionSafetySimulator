using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundParticleTrigger : MonoBehaviour
{
    public AudioClip triggerSound;
    AudioSource audioSource;
    ParticleSystem particles;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        particles = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerSound != null)
        {
            audioSource.PlayOneShot(triggerSound, 0.2f);
            particles.Play();
        }
    }
}
