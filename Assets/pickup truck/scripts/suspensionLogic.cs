using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AxleInfo {
	
	public WheelCollider leftWheelCol;
	public WheelCollider rightWheelCol;

	public Transform leftTire;
	public Transform rightTire;

	public Transform leftContainer;
	public Transform rightContainer;

	public Transform leftCylinder;
	public Transform rightCylinder;

	public Transform leftBone;
	public Transform rightBone;

	public bool steering;
	public bool driven;

	Vector3 rightTireDefPos;
	Vector3 leftTireDefPos;

	Vector3 rightCylinderDefPos;
	Vector3 leftCylinderDefPos;

	public void RotateWheels(float angle){

		if(steering){
			
			leftWheelCol.steerAngle = angle;
			leftContainer.localEulerAngles=new Vector3(0,angle,0);

			rightWheelCol.steerAngle = angle;
			rightContainer.localEulerAngles=new Vector3(0,angle,0);

			leftBone.localEulerAngles=new Vector3(0,-angle,0);
			rightBone.localEulerAngles=new Vector3(0,-angle,0);

		}

	}

	public void SetBrakeTorque(float brakeTorque){

		leftWheelCol.brakeTorque=brakeTorque;
		rightWheelCol.brakeTorque=brakeTorque;

	}

	public void SetupDefaultValues(){

		rightTireDefPos=rightTire.localPosition;
		leftTireDefPos=leftTire.localPosition;

		rightCylinderDefPos=rightCylinder.localPosition;
		leftCylinderDefPos=leftCylinder.localPosition;

	}

	public void CylinderLocalPos(){

		Vector3 offset = leftTireDefPos-leftTire.localPosition;
		leftCylinder.localPosition=new Vector3(leftCylinderDefPos.x,leftCylinderDefPos.y-offset.y,leftCylinderDefPos.z);

		offset=rightTireDefPos-rightTire.localPosition;
		rightCylinder.localPosition=new Vector3(rightCylinderDefPos.x,rightCylinderDefPos.y-offset.y,rightCylinderDefPos.z);

	}

	public void SetTorque(float torque, float brakeTorque, bool brake){

		if(driven){

			leftWheelCol.motorTorque=torque;
			rightWheelCol.motorTorque=torque;

		}

		leftWheelCol.brakeTorque=0;
		rightWheelCol.brakeTorque=0;

		if(brake){

			leftWheelCol.brakeTorque=brakeTorque;
			rightWheelCol.brakeTorque=brakeTorque;

		}

	}

}

public class suspensionLogic : MonoBehaviour {

	public bool controlled;

	public List<AxleInfo> axleInfos;

	public Transform centerOfMass;

	public float angle;
	public float angleChangeSpeed=2;
	public float maxAngle=40;

	public Transform jointLeft;
	public Transform jointRight;

	public Transform boneRightA;
	public Transform boneLeftA;

	public float maxMotorTorque=2000;
	public float maxBrakeTorque=2000;
	public float maxSpeed=120;

	bool handBrake;

	float speed;
	float currentTorque;

	void Start () {
	
		foreach (AxleInfo axleInfo in axleInfos){
			
			axleInfo.SetupDefaultValues();
			axleInfo.SetBrakeTorque(maxBrakeTorque);

		}
			
		GetComponent<Rigidbody>().centerOfMass=centerOfMass.localPosition;

	}
	

	void FixedUpdate () {

		if(controlled){

			speed=GetComponent<Rigidbody>().velocity.magnitude;
			currentTorque=maxMotorTorque*Input.GetAxis("Vertical");

			if(speed>maxSpeed)
				currentTorque=0;

			handBrake=(Input.GetKey(KeyCode.Space) || ((int)speed==0 && currentTorque==0) );

			//angle += angleChangeSpeed * Input.GetAxis("Horizontal");
			angle += Mathf.Lerp(angleChangeSpeed,0,speed/maxSpeed) * Input.GetAxis("Horizontal");

			if(Input.GetAxis("Horizontal")==0)
				angle=Mathf.Lerp(angle,0,speed/maxSpeed);
				
			angle=Mathf.Clamp(angle, -maxAngle, maxAngle);

		}

		foreach (AxleInfo axleInfo in axleInfos) {

			if(controlled){

				axleInfo.RotateWheels(angle);
				axleInfo.SetTorque(currentTorque,maxBrakeTorque,handBrake);

			}

			Vector3 position;
			Quaternion rotation;

			axleInfo.leftWheelCol.GetWorldPose(out position, out rotation);
			axleInfo.leftTire.position=position;
			axleInfo.leftTire.rotation=rotation;

			axleInfo.rightWheelCol.GetWorldPose(out position, out rotation);
			axleInfo.rightTire.position=position;
			axleInfo.rightTire.rotation=rotation;

			axleInfo.CylinderLocalPos();

		}

		SetBonePosition(jointLeft, boneLeftA);
		SetBonePosition(jointRight, boneRightA);

	}

	void SetBonePosition(Transform trans, Transform bone){

		bone.position=trans.position;

	}

}
