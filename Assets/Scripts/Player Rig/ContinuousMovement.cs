using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

/**
* Author: Nevin Foster
* ContinuousMovement is intended to be attached to a SteamVR controlled player object.
* Uses VR Input and look dirction to update a CharacterController.
*/
[RequireComponent(typeof(Player))]
[RequireComponent(typeof(CharacterController))]
public class ContinuousMovement : MonoBehaviour
{
    [Header("Interaction")]
    [Tooltip("Tracks that look direction and head position of the player")]
    public Camera SteamVRCamera;
    [Tooltip("The controller that is associated with the movement will be positioned at this transform (x and z). Otherwise, it'll use the camera.")]
    public Transform PlayerRoot;
    [Tooltip("Input for move strenght and direction")]
    public SteamVR_Action_Vector2 inputSource;
    [Tooltip("The max speed will toggle, but turn off once player stops moving")]
    public SteamVR_Action_Boolean sprintSource;


    [Header("Movement")]
    [Tooltip("The max player speed in meters/second")]
    public float speed = 1;
    [Tooltip("The player sprint speed in meters/second")]
    public float sprintSpeed = 2;
    [Tooltip("Gravity is calculated in the update function of this class. Gravity in meters/sec^2")]
    public float gravity = -9.81f;
    [Tooltip("Ground coliders. Helpful if the ground is different than walls")]
    public LayerMask groundLayer;
    [Tooltip("Increases the character collider height, intended to simulate the forehead")]
    public float additionalHeight;
    [Tooltip("The distance at which the character controller detects ground. Useful for padding and preventing unwanted ground sliding")]
    public float groundRaycastLength = 0.003f;
    [Tooltip("The angle at which the player no longer is affected by gravity. Useful for ground terrains without physics materials")]
    [Range(0,90)]
    public float groundedAngle = 5f;

    [Header("Audio")]
    [Tooltip("Audio source that plays only when moving. If there is an audio source attached, it is intended to be used ONLY for this script.")]
    public AudioSource MoveAudioSource = null;

    
    // Hips and body possitions moved to the "Body Visuals" prefab.
    //[Header("Hips")]
    //[Tooltip("The movement script will update this tranform to equal where the players hip is.")]
    //public Transform Hip;
    //[Tooltip("The percent of the height at which the hips are located.")]
    //[Range(0,1)]


// TODO:: Add in a calabraction system for setting the hip height.
    //public float hipHeight = 0.52f;

    // Attached Components
    private Player _player;
    private CharacterController _characterController;

    // private variables
    private float fallingSpeed;
    private Vector2 inputAxis;
    private bool isSprinting;

    void Start()
    {
        _player = GetComponent<Player>();
        _characterController = GetComponent<CharacterController>();

        isSprinting = false;
    }

    void Update()
    {
        // We update the input value on updates but do all physics during the fixed update.
        inputAxis = inputSource.GetAxis(SteamVR_Input_Sources.LeftHand);
        if (inputAxis.x + inputAxis.y < Mathf.Epsilon)
            isSprinting = false;
        else if (sprintSource.stateDown)
            isSprinting = !isSprinting;

        // play audio if needed
        if (MoveAudioSource != null)
            UpdateMoveAudio();

        // update the transform for the hips.
        //if(Hip != null)
        //{
        //    Hip.position = transform.TransformPoint(new Vector3(
        //        _characterController.center.x, // The x and z are equal to the characterCotroller
        //        (_characterController.center.y * 2) * hipHeight, // The y is positioned on the body using the percentage
        //        _characterController.center.z));
        //    Hip.rotation = Quaternion.Euler(0, SteamVRCamera.transform.eulerAngles.y, 0);
        //}
    }

    // TODO:: When audio stops, All audio coming from this source is stopped. Fix that.
    /// <summary>
    /// Checks if input values are not 0 and if so, then the player is moving and audio is played.
    /// </summary>
    private void UpdateMoveAudio()
    {
        // Maybe replace epsilon with an actual deadzone?
        // Maybe use body visuals for audio on move? (as body visuals could make sound when the player moves with the joystick AND in the playspace).
        if(inputAxis.x + inputAxis.y > Mathf.Epsilon)
        {
            if(!MoveAudioSource.isPlaying)
            {
                MoveAudioSource.Play();
            }
        }
        else if (MoveAudioSource.isPlaying)
        {
            MoveAudioSource.Stop();
        }
    }

    private void FixedUpdate()
    {
        FollowHeadset();

        Vector3 direction = Quaternion.Euler(0, SteamVRCamera.transform.eulerAngles.y, 0) * new Vector3(inputAxis.x, 0, inputAxis.y);
        direction *= Time.fixedDeltaTime * (isSprinting ? sprintSpeed : speed);

        if (CheckGrounded())
            fallingSpeed = 0;
        else
            fallingSpeed += gravity * Time.fixedDeltaTime;

        direction += fallingSpeed * Time.fixedDeltaTime * Vector3.up;
        _characterController.Move(direction);

    }

    /// <summary>
    /// Updates the center of the capsule collider to the headset. The y position is set instead by using the half the eye height.
    /// </summary>
    private void FollowHeadset()
    {
        _characterController.height = _player.eyeHeight + additionalHeight;

        Vector3 pos;
        if (PlayerRoot != null && PlayerRoot.gameObject.activeInHierarchy)
            pos = transform.InverseTransformPoint(PlayerRoot.position);
        else
            pos = _player.hmdTransforms[0].localPosition;

        _characterController.center = new Vector3(pos.x, _characterController.height / 2 + _characterController.skinWidth, pos.z);
    }

    bool CheckGrounded()
    {
        Vector3 rayStart = transform.TransformPoint(_characterController.center);
        float rayLenth = _characterController.center.y + groundRaycastLength;
        if (Physics.SphereCast(rayStart, _characterController.radius, Vector3.down, out RaycastHit hitInfo, rayLenth, groundLayer))
            return groundedAngle < Vector3.Angle(hitInfo.normal, Vector3.down);
        return false;

    }

    /// <summary>
    /// This check the current values for any possible flaws.
    /// </summary>
    private void OnValidate()
    {
        if (speed > sprintSpeed)
            Debug.LogWarning("Sprint speed is lower than the normal walk speed. Consider changing these values.");
    }
}
