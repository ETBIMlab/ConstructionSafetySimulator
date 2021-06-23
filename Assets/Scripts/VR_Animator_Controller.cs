using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_Animator_Controller : MonoBehaviour
{
    public float speedThreshold = 0.1f;
    [Range(0,1)]
    public float smoothing = 1;
    public float turnSmoothness;
    public float followAngle;
    private Animator animator;
    private Vector3 previousPos;
    private VR_Rig vrRig;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        vrRig = GetComponent<VR_Rig>();
        previousPos = vrRig.head.vrTarget.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Compute the Speed
        Vector3 headsetSpeed = (vrRig.head.vrTarget.position - previousPos) / Time.deltaTime;
        headsetSpeed.y = 0;

        //Local Speed
        Vector3 headsetLocalSpeed = transform.InverseTransformDirection(headsetSpeed);
        previousPos = vrRig.head.vrTarget.position;

        //Update body rotation
        if (followAngle < Quaternion.Angle(transform.rotation, vrRig.head.rigTarget.rotation) * (1))
        {
            Vector3 camrot = vrRig.head.vrTarget.eulerAngles;
            camrot.z = camrot.x = 0;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(camrot), turnSmoothness);
        }

            

        //Set Animator Values
        float previousDirectionX = animator.GetFloat("directionX");
        float previousDirectionY = animator.GetFloat("directionY");

        animator.SetBool("isMoving", headsetLocalSpeed.magnitude > speedThreshold);
        animator.SetFloat("directionX", Mathf.Lerp(previousDirectionX, Mathf.Clamp(headsetLocalSpeed.x, -1, 1), smoothing));
        animator.SetFloat("directionY", Mathf.Lerp(previousDirectionY, Mathf.Clamp(headsetLocalSpeed.z, -1, 1), smoothing));
    }
}
