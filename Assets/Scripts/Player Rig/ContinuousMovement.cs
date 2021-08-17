using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using ETBIM.StaticFunctions;

/**
* Author: Nevin Foster
* ContinuousMovement is intended to be attached to a SteamVR controlled player object.
* Uses VR Input and look dirction to update a CharacterController.
* 
* Overview:
* 
* Update: Updates inputs and audio. Calls Physics update
* PhysicsUpdate: Uses a characture controller to test movements with collisions.
* LateUpdate: (last update before render) Updates the players position to match the characture controller.
* 
*/

/// <summary>
/// Adds Continuous Movement support to a SteamVR Player.
/// </summary>
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
    public SteamVR_Action_Boolean enableMoveSource;


    [Header("Movement")]
    [Tooltip("The max player speed in meters/second")]
    public float speed = 1;

// Sprint button was moved to activate movement.
// Sprinting is no longer supported.
    //[Tooltip("The player sprint speed in meters/second")]
    //public float sprintSpeed = 2;

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

    [Header("Debuging/Testing")]
    [Tooltip("Show movement setup and update information.")]
    public bool ShowDebuging = false;
    [Tooltip("Show the CharactureController that is copied from the player")]
    public bool ShowSimulatedController = false;


    // Hips and body possitions moved to the "Body Visuals" prefab.
    //[Header("Hips")]
    //[Tooltip("The movement script will update this tranform to equal where the players hip is.")]
    //public Transform Hip;
    //[Tooltip("The percent of the height at which the hips are located.")]
    //[Range(0,1)]


    // TODO:: Add in a calabraction system for setting the hip height.
    //public float hipHeight = 0.52f;

    // Attached Components -----------------------------------------------------------------------------
    /// <summary>
    /// We are avoiding using "Player.Instance" because it can only be assesed after some of the SteamVR setup. For simplicity, and given that this component must be attached, we reference the player directly.
    /// </summary>
    private Player _player;
    /// <summary>
    /// The attached characterController is just used for initail values. We use a simulated controller to do all of the movement.
    /// </summary>
    private CharacterController _characterController;
    // -------------------------------------------------------------------------------------------------

    // private variables -------------------------------------------------------------------------------
    private float fallingSpeed;
    private Vector2 inputAxis;
    private Vector3 LastHeadPosition;
    private CharacterController characterControllerSimulator;
    //private bool isSprinting;
    // -------------------------------------------------------------------------------------------------

    private IEnumerator Start()
    {
        while (SteamVR.initializedState == SteamVR.InitializedStates.None || SteamVR.initializedState == SteamVR.InitializedStates.Initializing)
            yield return null;

        _player = GetComponent<Player>();
        _characterController = GetComponent<CharacterController>();
        
        
        LastHeadPosition = SteamVRCamera.transform.position;
        LastHeadPosition.y = transform.position.y;

        GameObject charactureControllerSimulator_go = new GameObject("Player's CharactureControllerSimulator");
        charactureControllerSimulator_go.layer = gameObject.layer;
        charactureControllerSimulator_go.transform.parent = null;
        charactureControllerSimulator_go.transform.SetPositionAndRotation(
            LastHeadPosition,
            transform.rotation);
        if (!ShowSimulatedController)
            charactureControllerSimulator_go.hideFlags = HideFlags.HideAndDontSave;

        characterControllerSimulator = charactureControllerSimulator_go.AddComponent<CharacterController>();
        if (!characterControllerSimulator.CopyFrom(_characterController))
        {
            Debug.LogWarning("Could not copy Characture controller. Using fallback calculation system.");
            characterControllerSimulator = null;
            Destroy(charactureControllerSimulator_go);
        }
        else
        {
            if (ShowDebuging)
                Debug.Log("<b>[ContinuousMovement]</b> Characture Controller Simulator is setup");
            // We no longer want the _character controller because it is a build in collider.
            _characterController.enabled = false;

            characterControllerSimulator.height = _player.eyeHeight + additionalHeight;
            characterControllerSimulator.center = new Vector3(
                characterControllerSimulator.center.x,
                characterControllerSimulator.height / 2 + characterControllerSimulator.skinWidth,
                characterControllerSimulator.center.z);
        }

        
        //isSprinting = false;
    }

    /// <summary>
    /// Update the inputs and audio.
    /// </summary>
    void Update()
    {
        // We update the input value on updates but do all physics during the fixed update.
        if (enableMoveSource.state)
        {
            inputAxis = inputSource.GetAxis(SteamVR_Input_Sources.LeftHand);
            //if (inputAxis.x + inputAxis.y < Mathf.Epsilon)
            //    isSprinting = false;
            //else if (sprintSource.stateDown)
            //    isSprinting = !isSprinting;

            if (ShowDebuging)
                Debug.Log("<b>[ContinuousMovement]</b> Input Axis: " + inputAxis);
        }
        else
        {
            inputAxis = Vector2.zero;
        }

        // play audio if needed
        if (MoveAudioSource != null)
            UpdateMoveAudio();

        // MOVED TO PLAYER BODY!
        //update the transform for the hips.
        //if(Hip != null)
        //{
        //    Hip.position = transform.TransformPoint(new Vector3(
        //        _characterController.center.x, // The x and z are equal to the characterCotroller
        //        (_characterController.center.y * 2) * hipHeight, // The y is positioned on the body using the percentage
        //        _characterController.center.z));
        //    Hip.rotation = Quaternion.Euler(0, SteamVRCamera.transform.eulerAngles.y, 0);
        //}

        UpdatePhysics();
    }

    // TODO:: When audio stops, All audio coming from this source is stopped. Fix that.
    /// <summary>
    /// Checks if input values are not 0 and if so, then the player is moving and audio is played.
    /// </summary>
    /// 
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

    /// <summary>
    /// Simulate the character controller. controller.move() happens on Fixed update to match other physics.
    /// </summary>
    private void UpdatePhysics()
    {
        if (characterControllerSimulator == null)
        {
            FallbackUpdate();
            return;
        }

        // Update the character controller to match the players height (for crouching)
        characterControllerSimulator.height = _player.eyeHeight + additionalHeight;
        characterControllerSimulator.center = new Vector3(
            characterControllerSimulator.center.x,
            characterControllerSimulator.height / 2 + characterControllerSimulator.skinWidth,
            characterControllerSimulator.center.z);

        LastHeadPosition.y = characterControllerSimulator.transform.position.y;
        characterControllerSimulator.transform.position = LastHeadPosition;


        Vector3 direction, inputComp, headComp, gravComp;

        // Add Input component
        inputComp = Quaternion.Euler(0, SteamVRCamera.transform.eulerAngles.y, 0) * new Vector3(inputAxis.x, 0, inputAxis.y);
        inputComp *= Time.deltaTime * (/*isSprinting ? sprintSpeed :*/ speed);

        // Add Headset movement component
        headComp = (SteamVRCamera.transform.position - LastHeadPosition);
        headComp.y = 0;
        LastHeadPosition = SteamVRCamera.transform.position;

        // Add gravity component
        if (CheckGrounded())
            fallingSpeed = 0;
        else
            fallingSpeed += gravity * Time.deltaTime;
        gravComp = Vector3.up * fallingSpeed * Time.deltaTime;
        
        direction = inputComp + headComp + gravComp;

        // Move the charature controller to check for collisions.
        if (ShowDebuging)
            Debug.Log("<b>[ContinuousMovement]</b> Position Change: " + direction + "  Input Component: " + inputComp + "  Head Component: " + headComp + "  Gravity Component: " + gravComp + "  Track Pad: " + inputAxis + "  Enabled Track pad: " + enableMoveSource.state + "  Time.deltaTime: " + Time.deltaTime + "  TimeScale: " + Time.timeScale + "  LastHeadPosition: " + LastHeadPosition);

        characterControllerSimulator.Move(direction);
    }

    /// <summary>
    /// Before moving on to render, move the player to match the simulated position.
    /// </summary>
    private void LateUpdate()
    {
        if (characterControllerSimulator != null)
        {
            if (ShowDebuging)
                Debug.Log("<b>[ContinuousMovement]</b> Updating SteamVR player position");

            transform.position = new Vector3(
                transform.position.x + characterControllerSimulator.transform.position.x - LastHeadPosition.x,
                characterControllerSimulator.transform.position.y,
                transform.position.z + characterControllerSimulator.transform.position.z - LastHeadPosition.z
                );

            LastHeadPosition = SteamVRCamera.transform.position;
        }
    }

    private void FallbackUpdate()
    {
        FallbackFollowHeadset();

        Vector3 direction = Quaternion.Euler(0, SteamVRCamera.transform.eulerAngles.y, 0) * new Vector3(inputAxis.x, 0, inputAxis.y);
        direction *= Time.deltaTime * (/*isSprinting ? sprintSpeed :*/ speed);

        if (FallbackCheckGrounded())
            fallingSpeed = 0;
        else
            fallingSpeed += gravity * Time.deltaTime;

        direction += fallingSpeed * Time.deltaTime * Vector3.up;

        if (ShowDebuging)
            Debug.Log("<b>[ContinuousMovement]</b> Position Change: " + direction);

        _characterController.Move(direction);
    }

    /// <summary>
    /// Updates the center of the capsule collider to the headset. The y position is set instead by using the half the eye height.
    /// </summary>
    private void FallbackFollowHeadset()
    {
        _characterController.height = _player.eyeHeight + additionalHeight;

        Vector3 pos;
        if (PlayerRoot != null && PlayerRoot.gameObject.activeInHierarchy)
            pos = transform.InverseTransformPoint(PlayerRoot.position);
        else
            pos = _player.hmdTransforms[0].localPosition;

        _characterController.center = new Vector3(pos.x, _characterController.height / 2 + _characterController.skinWidth, pos.z);
    }

    bool FallbackCheckGrounded()
    {
        Vector3 rayStart = transform.TransformPoint(_characterController.center);
        float rayLenth = _characterController.center.y + groundRaycastLength;
        if (Physics.SphereCast(rayStart, _characterController.radius, Vector3.down, out RaycastHit hitInfo, rayLenth, groundLayer))
            return groundedAngle < Vector3.Angle(hitInfo.normal, Vector3.down);
        return false;
    }

    bool CheckGrounded()
    {
        Vector3 rayStart = transform.TransformPoint(characterControllerSimulator.center);
        float rayLenth = characterControllerSimulator.center.y + groundRaycastLength;
        if (Physics.SphereCast(rayStart, characterControllerSimulator.radius, Vector3.down, out RaycastHit hitInfo, rayLenth, groundLayer))
            return groundedAngle < Vector3.Angle(hitInfo.normal, Vector3.down);
        return false;

    }

    /// <summary>
    /// This check the current values for any possible flaws.
    /// </summary>
    private void OnValidate()
    {
        //if (speed > sprintSpeed)
        //    Debug.LogWarning("Sprint speed is lower than the normal walk speed. Consider changing these values.");
    }
}
