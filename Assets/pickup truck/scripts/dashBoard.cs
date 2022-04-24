using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class dashBoard : MonoBehaviour {

	public float maxSteeringWheelAngle;
	public float steeringWheelAngle;

	public Transform steeringWheel;

	public bool gloveCompartmentOpen;

	public GameObject oilIcon;
	public bool enableOil;
	float oilEmissive;

	public GameObject beamIcon;
	public bool enableBeam;
	float beamEmissive;

	public GameObject brakeIcon;
	public bool enableBrake;
	float brakeEmissive;

	public GameObject chgIcon;
	public bool enableChg;
	float chgEmissive;

	public GameObject fastenBeltsIcon;
	public bool enableFastenBelts;
	float fastenBeltsEmissive;

	public GameObject wd4Icon;
	public bool enableWd4;
	float wd4Emissive;

	public GameObject leftArrowIcon;
	public bool enableLeftArrow;
	float leftArrowEmissive;

	public GameObject rightArrowIcon;
	public bool enableRightArrow;
	float rightArrowEmissive;

	public bool dashBoardLightEnable;
	public Light[] dashBoardLights;

	public enum gears{neutral, first, second, third, fourth, fifth, rear};
	public gears currentGear;

	public float currentSpeed;
	public float currentRpm;
	public float fuelLevel;
	public float engineTemperature;

	public Transform speedPointer;
	public float maxSpeedValue;
	public float maxSpeedPointerAngle;
	float speedPointerAngle;
	Vector3 speedPointerDefRot;

	public Transform rpmPointer;
	public float maxRpmValue;
	public float maxRpmPointerAngle;
	float rpmPointerAngle;
	Vector3 rpmPointerDefRot;

	public Transform fuelPointer;
	public float maxFuelValue;
	public float maxFuelPointerAngle;
	float fuelPointerAngle;
	Vector3 fuelPointerDefRot;

	public Transform temperaturePointer;
	public float maxTemperatureValue;
	public float maxTemperaturePointerAngle;
	float temperaturePointerAngle;
	Vector3 temperaturePointerDefRot;

	Animator anim;

	void Start () {

		speedPointerDefRot=speedPointer.localEulerAngles;
		rpmPointerDefRot=rpmPointer.localEulerAngles;
		fuelPointerDefRot=fuelPointer.localEulerAngles;
		temperaturePointerDefRot=temperaturePointer.localEulerAngles;

		anim=transform.root.GetComponent<Animator>();

	}

	void Update () {
		
		steeringWheelAngle=Mathf.Clamp(steeringWheelAngle, -maxSteeringWheelAngle,maxSteeringWheelAngle);
		steeringWheel.localEulerAngles=new Vector3(steeringWheel.localEulerAngles.x,steeringWheel.localEulerAngles.y,steeringWheelAngle);

		oilEmissive=SetEmissive(oilIcon,oilEmissive,enableOil);
		beamEmissive=SetEmissive(beamIcon,beamEmissive,enableBeam);
		chgEmissive=SetEmissive(chgIcon,chgEmissive,enableChg);
		brakeEmissive=SetEmissive(brakeIcon,brakeEmissive,enableBrake);
		fastenBeltsEmissive=SetEmissive(fastenBeltsIcon,fastenBeltsEmissive,enableFastenBelts);
		wd4Emissive=SetEmissive(wd4Icon,wd4Emissive,enableWd4);
		SetEmissive(rightArrowIcon,transform.root.GetComponent<carControl>().rightTurnIntensity);
		SetEmissive(leftArrowIcon,transform.root.GetComponent<carControl>().leftTurnIntensity);


		speedPointerAngle=Mathf.Lerp(speedPointerDefRot.z,maxSpeedPointerAngle,currentSpeed/maxSpeedValue);
		speedPointer.localEulerAngles=new Vector3(speedPointerDefRot.x,speedPointerDefRot.y,speedPointerAngle);

		rpmPointerAngle=Mathf.Lerp(rpmPointerDefRot.z,maxRpmPointerAngle,currentRpm/maxRpmValue);
		rpmPointer.localEulerAngles=new Vector3(rpmPointerDefRot.x,rpmPointerDefRot.y,rpmPointerAngle);

		fuelPointerAngle=Mathf.Lerp(fuelPointerDefRot.z,maxFuelPointerAngle,fuelLevel/maxFuelValue);
		fuelPointer.localEulerAngles=new Vector3(fuelPointerDefRot.x,fuelPointerDefRot.y,fuelPointerAngle);

		temperaturePointerAngle=Mathf.Lerp(temperaturePointerDefRot.z,maxTemperaturePointerAngle,engineTemperature/maxTemperatureValue);
		temperaturePointer.localEulerAngles=new Vector3(temperaturePointerDefRot.x,temperaturePointerDefRot.y,temperaturePointerAngle);

		foreach(Light l in dashBoardLights)
			l.enabled=dashBoardLightEnable;

		anim.SetBool("gloveCompartmentOpen", gloveCompartmentOpen);
		anim.SetInteger("gear", (int)currentGear);

	}

	float SetEmissive(GameObject emissiveObject, float emissive, bool enable){

		if(enable)
			emissive=Mathf.Lerp(emissive,1,Time.deltaTime*20);
		else
			emissive=Mathf.Lerp(emissive,0,Time.deltaTime*20);

		emissive=Mathf.Clamp01(emissive);

		emissiveObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.white*Mathf.LinearToGammaSpace(emissive));

		return emissive;

	}

	void SetEmissive(GameObject emissiveObject, float emissive){

		emissiveObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.white*Mathf.LinearToGammaSpace(emissive));

	}


	public void SetSpeed(float speed){
		currentSpeed=speed;
	}

	public void SetRPM(float rpm){
		currentRpm=rpm;
	}

	public void SetFuelLevel(float level){
		fuelLevel=level;
	}

	public void SetTemperature(float temperature){
		engineTemperature=temperature;
	}

	public void EnableDashboardLights(bool enable){
		dashBoardLightEnable=enable;
	}

	public void OpenGloveCompartment(bool open){
		gloveCompartmentOpen=open;
	}

	public void SetSteeringWheelAngle(float angle){
		steeringWheelAngle=angle;
	}

	public void SetGear(int gear){
		currentGear=(gears)gear;
	}

	public void SetOil(bool enable){
		enableOil=enable;
	}

	public void SetBeam(bool enable){
		enableBeam=enable;
	}

	public void SetCharge(bool enable){
		enableChg=enable;
	}

	public void SetFastenBelts(bool enable){
		enableFastenBelts=enable;
	}

	public void SetBrake(bool enable){
		enableBrake=enable;
	}

	public void Set4WD(bool enable){
		enableWd4=enable;
	}
}
