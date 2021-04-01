using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(CharacterController))]
public class ContinuousMovement : MonoBehaviour
{
    public float speed = 1;
    public float gravity = -9.81f;
    public SteamVR_Action_Vector2 inputSource;
    public LayerMask groundLayer;
    public float additionalHeight;
    public Camera SteamVRCamera;

    private float fallingSpeed;
    private Vector2 inputAxis;
    private CharacterController character;

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
        CapsuleFollowHeadset();

        Quaternion rotation = Quaternion.Euler(0, SteamVRCamera.transform.eulerAngles.y, 0);
        Vector3 direction = rotation * new Vector3(inputAxis.x, 0, inputAxis.y);
        character.Move(direction * Time.fixedDeltaTime * speed);
         
        if (CheckGrounded())
            fallingSpeed = 0;
        else
            fallingSpeed += gravity * Time.fixedDeltaTime;

        character.Move(Vector3.up * fallingSpeed * Time.fixedDeltaTime);


    }
    

    void CapsuleFollowHeadset()
    {
        /*
        Vector3 distanceMoved = transform.InverseTransformPoint(rig.cameraGameObject.transform.position) - character.center;
        distanceMoved.Scale(new Vector3(1, 0, 1));
        rig.transform.position -= distanceMoved;
        character.Move(distanceMoved);*/

        //character.height = rig.cameraInRigSpaceHeight + additionalHeight;
        //Vector3 campos = transform.InverseTransformPoint(rig.cameraGameObject.transform.position);
        //character.center = new Vector3(campos.x, character.height / 2 + character.skinWidth, campos.z);
    }

    bool CheckGrounded()
    {
        Vector3 rayStart = transform.TransformPoint(character.center);
        float rayLenth = character.center.y + 0.003f;
        return Physics.SphereCast(rayStart, character.radius, Vector3.down, out RaycastHit hitInfo, rayLenth, groundLayer);
    }
}
