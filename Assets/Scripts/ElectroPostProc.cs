using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

//AUTHOR: William Jenkins
//Trigger script to effect targeted post processing profile
//Apply this script to a game object that acts as a trigger and assign it the main post processing profile
//this script is specifically tuned to simulate being "shocked" 
//reduces saturation to greyscale, increases contrast, and applies a vignette

public class ElectroPostProc : MonoBehaviour
{
    public PostProcessVolume targetProfile;
    public float vignetteAmount = 1.0f;
    public float saturationAmount = -100.0f;
    public float contrastAmount = 60.0f;
    public float exposureAmount = 2.0f;
    private ColorGrading _colorgrading;
    private Vignette _vignette;
    private float currentContrast;
    private float currentSaturation;
    private float currentVignette;
    private float currentExposure;

    // Start is called before the first frame update
    void Start()
    {
        targetProfile.profile.TryGetSettings(out _colorgrading);
        targetProfile.profile.TryGetSettings(out _vignette);
        currentContrast = _colorgrading.contrast.value;
        currentSaturation = _colorgrading.saturation.value;
        currentVignette = _vignette.intensity.value;
        currentExposure = _colorgrading.postExposure.value;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        _colorgrading.saturation.value = saturationAmount;
        _colorgrading.contrast.value = contrastAmount;
        _vignette.intensity.value = vignetteAmount;
        _colorgrading.postExposure.value = exposureAmount;
    }

    private void OnTriggerExit(Collider other) {
        _colorgrading.saturation.value = currentSaturation;
        _colorgrading.contrast.value = currentContrast;
        _vignette.intensity.value = currentVignette;
        _colorgrading.postExposure.value = currentExposure;

    }
}
