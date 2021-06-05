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
[RequireComponent(typeof(CharacterController))]
public class ContinuousMovement : MonoBehaviour
{
    public float speed = 1;
    public float gravity = -9.81f;
    public SteamVR_Action_Vector2 inputSource;
    public LayerMask groundLayer;
    public float additionalHeight;
    [Tooltip("Tracks that look direction and head position of the player")]
    public Camera SteamVRCamera;
    [Tooltip("Audio Clip that plays only when moving.")]
    public AudioClip audioClipOnMove = null;
    [Range(0,1)]
    public float audioVolume;

    private float fallingSpeed;
    private Vector2 inputAxis;
    private CharacterController _characterController;
    private AudioSource _audioSource;
    private bool audioIsPlaying = false;

    // Start is called before the first frame update
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        if(audioClipOnMove)
            if((_audioSource = GetComponent<AudioSource>()) == null)
            {
                Debug.Log("Audio Source Not found on object. Disabling audio on move");
                audioClipOnMove = null;
            }
        audioIsPlaying = false;
    }

    // Update is called once per frame
    void Update()
    {
        inputAxis = inputSource.GetAxis(SteamVR_Input_Sources.LeftHand);
        if(audioClipOnMove)
            updateMoveAudio();
    }

    // TODO:: When audio stops, All audio coming from this source is stopped. Fix that.
    private void updateMoveAudio()
    {
        if(inputAxis.x + inputAxis.y > Mathf.Epsilon)
        {
            if(!audioIsPlaying)
            {
                audioIsPlaying = true;
                _audioSource.PlayOneShot(audioClipOnMove, 0.7f);
            }
        }
        else if(audioIsPlaying)
        {
            audioIsPlaying = false;
            _audioSource.Stop();
        }
    }

    private void FixedUpdate()
    {
        CapsuleFollowHeadset();

        Quaternion rotation = Quaternion.Euler(0, SteamVRCamera.transform.eulerAngles.y, 0);
        Vector3 direction = rotation * new Vector3(inputAxis.x, 0, inputAxis.y);
        _characterController.Move(direction * Time.fixedDeltaTime * speed);
         
        if (CheckGrounded())
            fallingSpeed = 0;
        else
            fallingSpeed += gravity * Time.fixedDeltaTime;

        _characterController.Move(Vector3.up * fallingSpeed * Time.fixedDeltaTime);


    }
    
    // TODO:: Function a little buggy, debug/update/test later.
    void CapsuleFollowHeadset()
    {
        /*
        Vector3 distanceMoved = transform.InverseTransformPoint(rig.cameraGameObject.transform.position) - _characterController.center;
        distanceMoved.Scale(new Vector3(1, 0, 1));
        rig.transform.position -= distanceMoved;
        _characterController.Move(distanceMoved);*/

        _characterController.height = GetComponent<Player>().eyeHeight + additionalHeight;
        Vector3 campos = GetComponent<Player>().hmdTransforms[0].localPosition;
        _characterController.center = new Vector3(campos.x, _characterController.height / 2 + _characterController.skinWidth, campos.z);
    }

    bool CheckGrounded()
    {
        Vector3 rayStart = transform.TransformPoint(_characterController.center);
        float rayLenth = _characterController.center.y + 0.003f;
        return Physics.SphereCast(rayStart, _characterController.radius, Vector3.down, out RaycastHit hitInfo, rayLenth, groundLayer);
    }
}
