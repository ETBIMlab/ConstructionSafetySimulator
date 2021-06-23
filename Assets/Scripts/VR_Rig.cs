using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VRMap
{
   public Transform vrTarget;
   public Transform rigTarget;
   public Vector3 trackingPositionOffset;
   public Vector3 trackingRotationOffset;

   public void Map()
   {
        rigTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
        rigTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
   }
}

public class VR_Rig : MonoBehaviour
{
    public float turnSmoothness;
    public VRMap head;
    public VRMap leftHand;
    public VRMap rightHand;


    private float distance;
    private Vector3 headBodyOffset;


    // Start is called before the first frame update
    void Start()
    {
        headBodyOffset = transform.position - head.rigTarget.position;
        Vector3 noy = headBodyOffset;
        noy.y = 0;
        distance = Vector3.Magnitude(noy);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
       
        Vector3 campos = head.rigTarget.position;

        Vector3 positionChange = -transform.position + (headBodyOffset + campos);
        positionChange.x = (campos - (transform.position + transform.forward * distance)).x;
        positionChange.z = (campos - (transform.position + transform.forward * distance)).z;


        transform.position += positionChange;


        head.Map();
        leftHand.Map();
        rightHand.Map();       
    }
}
