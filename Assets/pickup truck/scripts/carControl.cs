using UnityEngine;
using System.Collections;

public class carControl : MonoBehaviour {

	public Transform interiorCam;

	public lights rightHeadlight;
	public lights leftHeadlight;
	public lights backlightLeft;
	public lights backlightRight;
	public lights[] turnSignalFrontRight;
	public lights[] turnSignalFrontLeft;
	public lights turnSignalBackRight;
	public lights turnSignalBackLeft;
	public lights reverseLightsLeft;
	public lights reverseLightsRight;

	public bool leftDoorOpen;
	public bool rightDoorOpen;
	public bool backDoorOpen;
	public bool hoodOpen;

	float headLightsIntensity;
	float backlightsIntensity;
	[HideInInspector]
	public float leftTurnIntensity;
	[HideInInspector]
	public float rightTurnIntensity;
	float reverseLightIntensity;
	float timer;

	int turnSelect;

	bool headlights;
	bool backlights;
	bool reverseLights;
	bool emergency;
	bool turnLeft;
	bool enableTurnLeft;
	bool turnRight;
	bool enableTurnRight;

	Animator anim;

	void Start () {

		anim=GetComponent<Animator>();

	}
		

	void Update () {

		if(headlights)
			headLightsIntensity+=Time.deltaTime*10;
		else
			headLightsIntensity-=Time.deltaTime*10;
		
		headLightsIntensity=Mathf.Clamp01(headLightsIntensity);


		if(backlights)
			backlightsIntensity+=Time.deltaTime*10;
		else
			backlightsIntensity-=Time.deltaTime*10;

		backlightsIntensity=Mathf.Clamp01(backlightsIntensity);

		if(reverseLights)
			reverseLightIntensity+=Time.deltaTime*10;
		else
			reverseLightIntensity-=Time.deltaTime*10;

		reverseLightIntensity=Mathf.Clamp01(reverseLightIntensity);

		switch(turnSelect){
		case 0:
			emergency=false;
			enableTurnLeft=false;
			enableTurnRight=false;
			turnLeft=false;
			turnRight=false;
			timer=0;
			break;
		case 1:
			emergency=true;
			enableTurnLeft=false;
			enableTurnRight=false;
			break;
		case 2:
			emergency=false;
			enableTurnLeft=true;
			enableTurnRight=false;
			break;
		case 3:
			emergency=false;
			enableTurnLeft=false;
			enableTurnRight=true;
			break;
		}

		if(emergency){
			timer+=Time.deltaTime;
			if(timer<0.4f){
				turnLeft=false;
				turnRight=false;
			}
			 if(timer>0.4f){
				turnLeft=true;
				turnRight=true;
			}
			 if(timer>0.8f)
				timer=0;
		}

		if(enableTurnLeft){
			timer+=Time.deltaTime;
			turnRight=false;
			if(timer<0.4f)
				turnLeft=false;
			 if(timer>0.4f)
				turnLeft=true;
			 if(timer>0.8f)
				timer=0;
		}
			
		if(enableTurnRight){
			timer+=Time.deltaTime;
			turnLeft=false;
			if(timer<0.4f)
				turnRight=false;
			 if(timer>0.4f)
				turnRight=true;
			 if(timer>0.8f)
				timer=0;
		}


		if(turnLeft)
			leftTurnIntensity+=Time.deltaTime*10;
		else
			leftTurnIntensity-=Time.deltaTime*10;

		leftTurnIntensity=Mathf.Clamp01(leftTurnIntensity);


		if(turnRight)
			rightTurnIntensity+=Time.deltaTime*10;
		else
			rightTurnIntensity-=Time.deltaTime*10;

		rightTurnIntensity=Mathf.Clamp01(rightTurnIntensity);

		rightHeadlight.lightIntensity=headLightsIntensity;
		leftHeadlight.lightIntensity=headLightsIntensity;

		backlightLeft.lightIntensity=backlightsIntensity;
		backlightRight.lightIntensity=backlightsIntensity;

		reverseLightsLeft.lightIntensity=reverseLightsRight.lightIntensity=reverseLightIntensity;

		turnSignalBackLeft.lightIntensity=leftTurnIntensity;

		foreach(lights l in turnSignalFrontLeft)
			l.lightIntensity=leftTurnIntensity;

		turnSignalBackRight.lightIntensity=rightTurnIntensity;

		foreach(lights l in turnSignalFrontRight)
			l.lightIntensity=rightTurnIntensity;

		anim.SetBool("leftDoor", leftDoorOpen);
		anim.SetBool("rightDoor", rightDoorOpen);
		anim.SetBool("backDoor", backDoorOpen);
		anim.SetBool("hood", hoodOpen);

	}

	public void SetHeadLight(bool enable){//        Headlight enable
		headlights=enable;
	}

	public void SetBackLight(bool enable){//        Backlight enable
		backlights=enable;
	}

	public void SetReverseLight(bool enable){//        Reverselight enable
		reverseLights=enable;
	}

	public void LeftHeadLightDestroy(bool destroy){//        left headlight destroy
		leftHeadlight.destroy=destroy;
	}

	public void RightHeadLightDestroy(bool destroy){//        right headlight destroy
		rightHeadlight.destroy=destroy;
	}

	public void LeftBackLightDestroy(bool destroy){//        left backlight destroy
		backlightLeft.destroy=destroy;
	}

	public void RightBackLightDestroy(bool destroy){//        right backlight destroy
		backlightRight.destroy=destroy;
	}

	public void LeftReverseLightDestroy(bool destroy){//        left reverselight destroy
		reverseLightsLeft.destroy=destroy;
	}

	public void RightReverseLightDestroy(bool destroy){//        right reverselight destroy
		reverseLightsRight.destroy=destroy;
	}

	public void SetTurnSelect(int select){//                     set turn signal
		turnSelect=select;
	}

	public void FrontLeftTurnSignalDestroy(bool destroy){//     Destroy front left turn signal
		foreach(lights l in turnSignalFrontLeft)
			l.destroy=destroy;
	}

	public void FrontRightTurnSignalDestroy(bool destroy){//    Destroy front right turn signal
		foreach(lights l in turnSignalFrontRight)
			l.destroy=destroy;
	}

	public void BackLeftTurnSignalDestroy(bool destroy){//       Destroy back left turn signal
		turnSignalBackLeft.destroy=destroy;
	}

	public void BackRightTurnSignalDestroy(bool destroy){//      Destroy back right turn signal
		turnSignalBackRight.destroy=destroy;
	}
		
}
