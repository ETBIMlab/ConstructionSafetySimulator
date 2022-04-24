using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class street : MonoBehaviour {
	public AudioMixer streetMixer;
	public gate leftGate;
	public gate rightGate;
	public float maxIntensity, maxBounce;
	Light mainLight;

	float openingFactor;
	float lowPass;

	void Start () {
		mainLight=GetComponent<Light>();
		mainLight.intensity=0;
		mainLight.bounceIntensity=0;
	}

	void Update () {
		openingFactor=leftGate.lightIntencity+rightGate.lightIntencity;
		mainLight.bounceIntensity=maxBounce*openingFactor;
		mainLight.intensity=maxIntensity*openingFactor;

		lowPass=openingFactor*5000;
		if(lowPass<200)
			lowPass=200;
		streetMixer.SetFloat("lowpass",lowPass);
	}
}
