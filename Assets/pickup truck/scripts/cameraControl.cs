using UnityEngine;
using System.Collections;

public class cameraControl : MonoBehaviour {
	
	public Vector3 target;

	public float xSpeed, ySpeed, zoomSpeed=10;

	public float minValueY=-20;
	public float maxValueY=80;

	Quaternion rotation;
	Vector3 position;
	float distance=5;

	float x,y;
	float tempX, tempY;
	public float smoothTime=0.3f;
	float xSmooth=0.0f;
	float ySmooth=0.0f;

	Transform interiorCam;

	bool inside;
	bool SliderPulled;

	float xInterior,yInterior,zInterior;
	float yAngle, xAngle;
	float tempInteriorZ;

	public float maxInteriorX=0.5f;
	public float maxInteriorY=1.95f;
	public float minInteriorY=1.55f;
	public float maxInteriorZ=0.023f;
	public float minInteriorZ=-0.05f;
	public float maxAngleX=90;
	public float maxAngleY=50;

	void Start () {

		x=transform.eulerAngles.y;
		y=transform.eulerAngles.x;

	}

	void LateUpdate () {


		if(!SliderPulled){

			if(!inside){//                                    OUTSIDE

				if(Input.GetMouseButton(0)){
					x+=Input.GetAxis("Mouse X")*xSpeed;
					y-=Input.GetAxis("Mouse Y")*ySpeed;
				}

				tempX=Mathf.SmoothDamp(tempX, x, ref xSmooth,smoothTime);
				tempY=Mathf.SmoothDamp(tempY, y, ref ySmooth,smoothTime);

				if(Input.GetMouseButton(1))
					distance+=Input.GetAxis("Mouse Y")*zoomSpeed;

				y = ClampAngle(y,minValueY,maxValueY);

				rotation=Quaternion.Euler(tempY,tempX,0);
				position=rotation*new Vector3(0,0,-distance) + target;

				transform.rotation=rotation;
				transform.position=position;

			}
			else{//                                           INSIDE

				//                             position

				xInterior=interiorCam.localPosition.x-Input.GetAxis("Mouse X")*xSpeed/50;
				yInterior=interiorCam.localPosition.y-Input.GetAxis("Mouse Y")*ySpeed/50;

				xInterior=Mathf.Clamp(xInterior,-maxInteriorX,maxInteriorX);
				yInterior=Mathf.Clamp(yInterior,minInteriorY,maxInteriorY);

				if(Input.GetMouseButton(0))
					interiorCam.localPosition=new Vector3(xInterior,yInterior,interiorCam.localPosition.z);



				tempInteriorZ=Mathf.SmoothDamp(tempInteriorZ, Input.GetAxis("Mouse ScrollWheel"), ref xSmooth,smoothTime);
				zInterior=interiorCam.localPosition.z+tempInteriorZ;

				zInterior=Mathf.Clamp(zInterior,minInteriorZ,maxInteriorZ);

				interiorCam.localPosition=new Vector3(interiorCam.localPosition.x,interiorCam.localPosition.y,zInterior);

				//                            rotation

				if(Input.GetMouseButton(1)){

					yAngle+=Input.GetAxis("Mouse X")*xSpeed;
					yAngle=Mathf.Clamp(yAngle,-maxAngleX,maxAngleX);
					xAngle-=Input.GetAxis("Mouse Y")*ySpeed;
					xAngle=Mathf.Clamp(xAngle,-maxAngleY,maxAngleY);

				}

				interiorCam.localEulerAngles=new Vector3(xAngle,yAngle,0);

				transform.position=interiorCam.position;
				transform.rotation=interiorCam.rotation;

			}

		}
			
	}
		
	float ClampAngle(float angle, float min, float max){
		if(angle < -360)
			angle+=360;
		if(angle>360)
			angle-=360;

		return Mathf.Clamp(angle, min, max);
	}

	public void SetInside(bool enable){
		inside=enable;
	}

	public void SetCam(Transform cam){
		
		interiorCam=cam;

	}

	public void PullSlider(bool pull){
		SliderPulled=pull;
	}
}
