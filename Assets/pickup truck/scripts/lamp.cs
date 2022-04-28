using UnityEngine;
using System.Collections;

public class lamp : MonoBehaviour {
	
	public GameObject lampLight;
	public GameObject glass;

	public Material notEmissive;
	public Material emissive;

	LensFlare flare;

	bool lightEnable;

	void Start () {
		
		flare=lampLight.GetComponent<LensFlare>();

	}
	

	void Update () {

			lampLight.SetActive(lightEnable);

			if(lightEnable)
				glass.GetComponent<Renderer>().material=emissive;
			else
				glass.GetComponent<Renderer>().material=notEmissive;


		if(lightEnable)
			flare.brightness=Random.Range(0.33f, 0.35f);
		
	}

	public void SetLightEnable(bool enable){

		lightEnable=enable;

	}
}
