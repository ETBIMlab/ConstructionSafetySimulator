using UnityEngine;
using System.Collections;

public class gate : MonoBehaviour {
	Animator anim;

	public float lightIntencity=0;
	public AudioClip gateSound;
	AudioSource gateAudio;
	bool soundEnable=false;

	void Start () {
		
		anim=GetComponent<Animator>();

		gateAudio = gameObject.AddComponent<AudioSource>();
		gateAudio.loop = true;
		gateAudio.clip = gateSound;
		gateAudio.volume = 0.5f;
		gateAudio.spatialBlend=1f;
	}

	public void SetOpen(bool open){

		anim.SetBool("open",open);

	}

	void EnableSound(){
		soundEnable = !soundEnable;
		if(soundEnable && !gateAudio.isPlaying)
			gateAudio.Play();
		else
			gateAudio.Stop();
	}
}
