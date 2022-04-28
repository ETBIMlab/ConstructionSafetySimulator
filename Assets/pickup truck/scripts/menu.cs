using UnityEngine;
using System.Collections;


public class menu : MonoBehaviour {
	
	public GameObject[] carPrefabs;
	GameObject car;

	public bool controlCar;

	int currentCar=0;

	public Texture2D fadeOutTexture;
	public float fadeSpeed=2f;

	public float yOffset;

	float alpha=0f;
	int direction=-1;

	Vector3 target;

	void Start () {

		Destroy(GameObject.FindGameObjectWithTag("Player"));

		car = (GameObject)Instantiate(carPrefabs[currentCar], carPrefabs[currentCar].transform.position, carPrefabs[currentCar].transform.rotation);

		FindObjectOfType<carMenu>().SetCar(car);

		car.GetComponent<suspensionLogic>().controlled=controlCar;

		target =  car.transform.position + Vector3.up*yOffset;
		GetComponent<cameraControl>().target = target;
		GetComponent<cameraControl>().SetCam(car.GetComponent<carControl>().interiorCam);

	}
	

	void Update () {

		target =  car.transform.position + Vector3.up*yOffset;
		GetComponent<cameraControl>().target = target;

		FadeScreen(direction);

	}

	void OnGUI () {

		GUI.color=new Color(GUI.color.r,GUI.color.g,GUI.color.b,alpha);

		GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height), fadeOutTexture);

	}

	public void SetDirection(){

		direction=1;

	}

	void ChangeCar(){
	
		GameObject[] obj=GameObject.FindGameObjectsWithTag("Respawn");
		for(int i=0; i<obj.Length;i++){
			Destroy(obj[i]);
		}

		Destroy(car);
		currentCar++;

		if(currentCar>carPrefabs.Length-1)
			currentCar=0;

		car = (GameObject)Instantiate(carPrefabs[currentCar], carPrefabs[currentCar].transform.position, carPrefabs[currentCar].transform.rotation);
		FindObjectOfType<carMenu>().SetCar(car);
		car.GetComponent<suspensionLogic>().controlled=controlCar;
		GetComponent<cameraControl>().SetCam(car.GetComponent<carControl>().interiorCam);

	}

	void FadeScreen(int dir){
		
		alpha+=Time.deltaTime* fadeSpeed * dir;
		alpha=Mathf.Clamp01(alpha);

		if(alpha>=0.99f){
			
			ChangeCar();
			direction=-1;

		}

	}
		
}
