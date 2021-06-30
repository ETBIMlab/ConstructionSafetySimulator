using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

[System.Serializable]
public class VRHandMap
{
   public Hand hand;
   public Transform rigTarget;
   public Vector3 trackingPositionOffset;
   public Vector3 trackingRotationOffset;

   public void Map()
   {
        if (hand == null || hand.mainRenderModel == null || hand.mainRenderModel.GetBone(0) == null) return;

        rigTarget.SetPositionAndRotation(
            hand.mainRenderModel.GetBone(0).TransformPoint(trackingPositionOffset),
            hand.mainRenderModel.GetBone(0).rotation * Quaternion.Euler(trackingRotationOffset)
            );
   }
}

[System.Serializable]
public class VRHeadMap
{
    public Transform vrTarget;
    public Transform rigTarget;
    public Vector3 trackingPositionOffset;
    public Vector3 trackingRotationOffset;

    public void Map()
    {
        rigTarget.SetPositionAndRotation(
            vrTarget.TransformPoint(trackingPositionOffset),
            vrTarget.rotation * Quaternion.Euler(trackingRotationOffset)
            );
    }
}

[RequireComponent(typeof(VR_Animator_Controller))]
public class VR_Rig : MonoBehaviour
{
    //public float turnAngle;
    public VRHeadMap head;
    public VRHandMap leftHand;
    public VRHandMap rightHand;
    [Range(0,180)]
    public float followAngle = 0;
    [Range(0.05f,180)]
    public float updateAngleStep = 1;
    public float headDownPadding = 0.5f;

    public Transform LeftArmExtention;
    public Transform RightArmExtention;
    public Transform LeftForeArm;
    public Transform RightForeArm;


    private float distance;
    private Vector3 headBodyOffset;
    private VR_Animator_Controller _VR_Animator_Controller;


    // Start is called before the first frame update
    void Start()
    {
        _VR_Animator_Controller = GetComponent<VR_Animator_Controller>();

        headBodyOffset = transform.position - head.rigTarget.position;
        Vector3 noy = headBodyOffset;
        noy.y = 0;
        distance = Vector3.Magnitude(noy);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (SteamVR.initializedState != SteamVR.InitializedStates.InitializeSuccess)
            return;

        Vector3 campos = head.rigTarget.position;

        Vector3 positionChange = -transform.position + (headBodyOffset + campos);
        positionChange.x = (campos - (transform.position + transform.forward * distance)).x;
        positionChange.z = (campos - (transform.position + transform.forward * distance)).z;
        

        Quaternion camrot = Quaternion.Euler(head.vrTarget.eulerAngles.y * Vector3.up);
        
        while (followAngle < Quaternion.Angle(transform.rotation, camrot) * (1 + _VR_Animator_Controller.Speed/3))
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, camrot, updateAngleStep);
        }

        transform.position += positionChange;

        head.Map();
        head.rigTarget.position -= transform.forward * (headDownPadding * (1 - Mathf.Min(Vector3.Angle(head.rigTarget.forward, Vector3.down), 90)/90));
        leftHand.Map();
        rightHand.Map();

        if (RightArmExtention)
        {
            RightArmExtention.position = rightHand.rigTarget.position;
            RightArmExtention.up = (RightForeArm.position - rightHand.rigTarget.position).normalized;
        }

        if (LeftArmExtention)
        {
            LeftArmExtention.position = leftHand.rigTarget.position;
            LeftArmExtention.up = (LeftForeArm.position - leftHand.rigTarget.position).normalized;
        }
    }
}
