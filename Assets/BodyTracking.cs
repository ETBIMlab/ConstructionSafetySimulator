using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyTracking : MonoBehaviour
{
    public float DefaultHeight;// = 1.62 is the normal height of a human;
    public float rotationWhenCrouched;
  
    public GameObject Head;//these keep track of our body objects.
    public GameObject CameraRig;
    public GameObject BodyRoot;
    public GameObject HeadRoot;
    public GameObject Torso;
    public GameObject Hips;

    private Quaternion HeadRotation;//these are all offset varibles to make sure evrything stays in place.
    private Quaternion TorsoRotation;
    private Vector3 TorsoOffset;
    private Vector3 HipOffset;
   // publi Vector3 HeadOffset;
    private Quaternion HipOffsetRot;
    private Quaternion TorsoOffsetRotation;
    private Quaternion HipOffsetRotation;
    
    public IK LeftArm;
    public IK RightArm;

void Awake()
    {
        HeadRotation = HeadRoot.transform.rotation;
       // HeadOffset = Head.transform.position - CameraRig.transform.position;
        TorsoRotation = BodyRoot.transform.rotation;
        TorsoOffset = Torso.transform.position -HeadRoot.transform.position;
        HipOffset = Hips.transform.position - HeadRoot.transform.position;
        HipOffsetRot = Hips.transform.rotation;
        TorsoOffsetRotation = Torso.transform.rotation;
        HipOffsetRotation = Torso.transform.rotation;
      //  Head.transform.position = new Vector3(Head.transform.position.x + HeadToCameraOffsetX, Head.transform.position.y + HeadToCameraOffsetY, Head.transform.position.z + HeadToCameraOffsetY);
       // Head.transform.position = vrTarget.TransformPoint(trackingPositionOffset);
    }
void Update()
    {
        //position the entire body in the same place as the HMD or camera.
        BodyRoot.transform.position = new Vector3(Head.transform.position.x,CameraRig.transform.position.y, Head.transform.position.z);

       // Head.position = Head.TransformPoint(HeadToCameraOffset);

        //If the head is looking more than 90% to the left or right rotate the body in that direction
        for (int i = 0; i < 5; i++)
        {
            if (Quaternion.Angle(Quaternion.Euler(0, BodyRoot.transform.rotation.eulerAngles.y, 0), Quaternion.Euler(0, Head.transform.rotation.eulerAngles.y, 0)) > 90)
            {
                BodyRoot.transform.rotation = Quaternion.RotateTowards(BodyRoot.transform.rotation, Quaternion.Euler(0, Head.transform.rotation.eulerAngles.y, 0) * TorsoRotation, 3);
            }
            else
            {
                if (RightArm.CantReach)
                {
                    BodyRoot.transform.rotation = Quaternion.RotateTowards(BodyRoot.transform.rotation, Quaternion.Euler(0, -90, 0) * Quaternion.Euler(0, Quaternion.FromToRotation(Vector3.forward, RightArm.Target.position - BodyRoot.transform.position).eulerAngles.y, 0) * TorsoRotation, 3);
                }
                if (LeftArm.CantReach)
                {
                    BodyRoot.transform.rotation = Quaternion.RotateTowards(BodyRoot.transform.rotation, Quaternion.Euler(0, 90, 0) * Quaternion.Euler(0, Quaternion.FromToRotation(Vector3.forward, LeftArm.Target.position - BodyRoot.transform.position).eulerAngles.y, 0) * TorsoRotation, 3);
                }
            }
            RightArm.UpdateIK();
            LeftArm.UpdateIK();
        }
        //And good luck understanding these, I barely understand my own math but it works. Explained Simply this makes you crouch when the headset gets close to the ground.
        Torso.transform.position= BodyRoot.transform.rotation * Quaternion.Euler((1-Head.transform.localPosition.y/DefaultHeight)*rotationWhenCrouched,0,0) * (TorsoOffset)+Head.transform.position-(Quaternion.Euler(0, Head.transform.rotation.eulerAngles.y, 0) * TorsoRotation*Vector3.forward  *(0.3f)* (FixEuler(Head.transform.rotation.eulerAngles.x)/ 180));
        Hips.transform.position= BodyRoot.transform.rotation * Quaternion.Euler((1 - Head.transform.localPosition.y / DefaultHeight) * rotationWhenCrouched, 0, 0) * (HipOffset) + Head.transform.position - (Quaternion.Euler(0, Head.transform.rotation.eulerAngles.y, 0) * TorsoRotation * Vector3.forward * (0.3f) * (FixEuler(Head.transform.rotation.eulerAngles.x) / 180));
        Torso.transform.rotation = BodyRoot.transform.rotation * Quaternion.Euler((1 - Head.transform.localPosition.y / DefaultHeight) * rotationWhenCrouched, 0, 0) * TorsoOffsetRotation;
        
        HeadRoot.transform.position = Head.transform.position;
        HeadRoot.transform.rotation = Head.transform.rotation* HeadRotation;
    }
float FixEuler(float angle)
    {
        if(angle < 180)
        {
            return angle;
        }
        else
        {
            return angle - 360;
        }
    }
}