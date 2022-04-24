using UnityEngine;
using System.Collections;

public class lights : MonoBehaviour {

	[Range(0.0f,1f)]
	public float lightIntensity;

	Transform m_Camera;

	float distanceTocam;
	float emissive;
	float angle;
	float initialValueOfTheLight;

	LensFlare flare;
	Color flareColor;
	Color lightColor;

	public float flareBrightness=1;
	float finalFlareBrightness;

	public bool udirected;
	public enum directions{forvard, back, up, down, left, right, forvardLeft, forvardRight, backLeft, backRight};
	public directions flareDirection;

	Vector3 direction;

	Color finalColor;

	Light m_light;

	public bool twoSided;

	public GameObject destroyedPrefab;
	GameObject cloneOfPrefab;

	public bool destroy;
	bool destroyed;
	public float force=125.0f;
	public float radius=5.0f;
	public float upwardModifier=0.1f;
	public ForceMode forceMode;

	public AudioClip[] brokenSound;
	AudioSource brokenAudio;

	void Start () {

		flare=GetComponent<LensFlare>();
		flareColor=flare.color;
		flare.enabled=true;

		if(GetComponent<Light>()!=null){
			
			m_light=GetComponent<Light>();
			initialValueOfTheLight=m_light.intensity;
			lightColor=m_light.color;

		}

		m_Camera=Camera.main.transform;

		brokenAudio = gameObject.AddComponent<AudioSource>();
		brokenAudio.loop = false;
		brokenAudio.volume = 1f;
		brokenAudio.spatialBlend=1f;

	}

	void LateUpdate () {

		Fracture();

		finalColor=Color.white*Mathf.LinearToGammaSpace(emissive*lightIntensity);

		if(transform.parent!=null)
			transform.parent.GetComponent<Renderer>().material.SetColor("_EmissionColor", finalColor);

		if(lightIntensity>0 && !destroyed){
			
			emissive=1;
			direction=FlareDir(flareDirection);
			distanceTocam=Vector3.Distance(transform.position, m_Camera.position);

			angle=Vector3.Angle(direction,  m_Camera.position-transform.position);

			if(!udirected){
				
				if(angle!=0 && !twoSided)
					finalFlareBrightness=flareBrightness*(4/distanceTocam)*((100-(1.11f*angle))/100);
				else if(angle!=0 && twoSided)
					finalFlareBrightness=flareBrightness*((4/distanceTocam)*(Mathf.Abs((100-(1.11f*angle)))/100));
				
			}
			else{
				if(angle!=0 )
					finalFlareBrightness=flareBrightness*(4/distanceTocam);
			}


			flare.brightness=finalFlareBrightness*lightIntensity;
		}
		else{
			
			flare.brightness=0;
			emissive=0;

		}

		if(GetComponent<Light>()!=null){
			
			if(lightIntensity>0 && !destroyed){
				
				m_light.enabled=true;
				m_light.intensity=lightIntensity*initialValueOfTheLight;

			}
			else
				m_light.enabled=false;
		}


	}


	void Fracture(){
		
		if(destroy && !destroyed){

			if(destroyedPrefab!=null)
				cloneOfPrefab=(GameObject)Instantiate(destroyedPrefab,transform.root.position, transform.root.rotation);

			if(transform.parent!=null)
				transform.parent.GetComponent<MeshRenderer>().enabled=false;

			if(transform.parent.GetComponent<MeshCollider>()!=null)
				transform.parent.GetComponent<MeshCollider>().enabled=false;

			if(brokenSound.Length!=0){
				
				brokenAudio.clip=brokenSound[Random.Range(0,brokenSound.Length)];
				brokenAudio.Play();

			}

			lightIntensity=0;

			foreach(Collider col in Physics.OverlapSphere(transform.position, radius)){
				
				if(col.attachedRigidbody!=null)
					col.attachedRigidbody.AddExplosionForce(force,transform.position,radius,upwardModifier,forceMode);
				
			}
		}

		if(!destroy && destroyed){

			if(transform.parent!=null)
				transform.parent.GetComponent<MeshRenderer>().enabled=true;

			if(transform.parent.GetComponent<MeshCollider>()!=null)
				transform.parent.GetComponent<MeshCollider>().enabled=true;

			if(cloneOfPrefab!=null)
				Destroy(cloneOfPrefab);
			
		}

		destroyed = destroy;
	}



	Vector3 FlareDir(directions d){
		
		switch(d){
		case directions.forvard:
			return transform.forward;
		case directions.back:
			return transform.forward*-1;
		case directions.up:
			return transform.up;
		case directions.down:
			return transform.up*-1;
		case directions.left:
			return transform.right*-1;
		case directions.right:
			return transform.right;
		case directions.forvardLeft:
			return transform.forward+(transform.right*-1);
		case directions.forvardRight:
			return transform.forward+transform.right;
		case directions.backLeft:
			return (transform.forward*-1)+(transform.right*-1);
		case directions.backRight:
			return (transform.forward*-1)+transform.right;
		default:
			return transform.forward;
		}

	}

	public void SetColorOfFlare(Color col){
		
		flare.color=col;

		if(m_light!=null)
			m_light.color=col;

	}

	public void SetColorOfFlare(bool value){
		
		if(!value){

			flare.color=flareColor;

			if(m_light!=null)
				m_light.color=lightColor;

		}
		
	}
}
