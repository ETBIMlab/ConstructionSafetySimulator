using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

//AUTHOR: William Jenkins
//Trigger script to effect targeted post processing profile
//Apply this script to a game object that acts as a trigger and assign it the main post processing profile
//this script is specifically tuned to simulate being "shocked" 
//reduces saturation to greyscale, increases contrast, and applies a vignette

public class ElectroPostProc : MonoBehaviour
{
    //PostProcessing profile
    public VolumeProfile _profile;

    //post processing effects
    private ColorAdjustments _colorAdjustments;
    private Vignette _vignette;

    //target values
    public float vignetteAmount = 1.0f;
    public float saturationAmount = -100.0f;
    public float contrastAmount = 60.0f;
    public float exposureAmount = 2.0f;
    
    //current values
    private float currentContrast;
    private float currentSaturation;
    private float currentVignette;
    private float currentExposure;

    // Start is called before the first frame update
    void Start()
    {   
        _profile.TryGet(out _colorAdjustments);
        _profile.TryGet(out _vignette);
        currentContrast = _colorAdjustments.contrast.value;
        currentSaturation = _colorAdjustments.saturation.value;
        currentVignette = _vignette.intensity.value;
        currentExposure = _colorAdjustments.postExposure.value;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        _colorAdjustments.saturation.value = saturationAmount;
        _colorAdjustments.contrast.value = contrastAmount;
        _vignette.intensity.value = vignetteAmount;
        _colorAdjustments.postExposure.value = exposureAmount;
    }

    private void OnTriggerExit(Collider other) {
        _colorAdjustments.saturation.value = currentSaturation;
        _colorAdjustments.contrast.value = currentContrast;
        _vignette.intensity.value = currentVignette;
        _colorAdjustments.postExposure.value = currentExposure;
    }
}
