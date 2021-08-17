using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bhaptics.Tact.Unity;

/**
* Author: Rustin Richardson
* Demo for landslide/trench collapse
*/
public class LandslideController : MonoBehaviour
{
    public Animator Landslide;
    public Animator Fill_In;
    public ParticleSystem Dust;
    public AudioSource Explosion;
    public HapticSource HapticFeedback;
    public KeyCode AnimationControlKey;
    bool isReset;

    
    // Start is called before the first frame update
    void Start()
    {
        ResetAnimations();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(AnimationControlKey))
        {
            if (isReset) InitiateLandslide();
            else ResetAnimations();
        }
    }

    void InitiateLandslide()
    {
        isReset = false;
        Dust.Play();
        Explosion.Play();
        HapticFeedback.PlayLoop();
        Fill_In.SetBool("Reset", false);
        Fill_In.SetBool("Start", true);
        Landslide.SetBool("Reset", false);
        Landslide.SetBool("Start", true);
    }
    void ResetAnimations()
    {
        HapticFeedback.Stop();
        Dust.Stop();
        Dust.Clear();
        Explosion.Stop();
        Explosion.time = 0;
        Fill_In.SetBool("Start", false);
        Fill_In.SetBool("Reset", true);
        Landslide.SetBool("Start", false);
        Landslide.SetBool("Reset", true);
        isReset = true;
    }
}
