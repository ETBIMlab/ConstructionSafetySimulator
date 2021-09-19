using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Playables;

[RequireComponent(typeof(RigBuilder))]
[RequireComponent(typeof(VR_Rig))]
[RequireComponent(typeof(Animator))]
public class VR_Animator_Controller : MonoBehaviour
{
    public float speedThreshold = 0.1f;
    [Range(0,1)]
    public float smoothing = 1;

    private Animator _animator;
    private Vector3 previousPos;
    private VR_Rig _vrRig;
    private RigBuilder _rigBuilder;

    public float Speed { get; private set; } = 0;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _vrRig = GetComponent<VR_Rig>();
        _rigBuilder = GetComponent<RigBuilder>();

        previousPos = _vrRig.head.vrTarget.position;
        //_rigBuilder.Build();
        //_rigBuilder.graph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

        //_animator.enabled = false;
    }

    internal void ForceUpdate()
    {
        //Compute the Speed
        Vector3 headsetSpeed = (_vrRig.head.vrTarget.position - previousPos) / Time.deltaTime;
        headsetSpeed.y = 0;

        //Local Speed
        Vector3 headsetLocalSpeed = transform.InverseTransformDirection(headsetSpeed);
        previousPos = _vrRig.head.vrTarget.position;


        Speed = headsetSpeed.magnitude;

        //Set Animator Values
        float previousDirectionX = _animator.GetFloat("directionX");
        float previousDirectionY = _animator.GetFloat("directionY");

        _animator.SetBool("isMoving", headsetLocalSpeed.magnitude > speedThreshold);
        _animator.SetFloat("directionX", Mathf.Lerp(previousDirectionX, Mathf.Clamp(headsetLocalSpeed.x, -1, 1), smoothing));
        _animator.SetFloat("directionY", Mathf.Lerp(previousDirectionY, Mathf.Clamp(headsetLocalSpeed.z, -1, 1), smoothing));

        //_rigBuilder.graph.Evaluate(Time.deltaTime);
        //_animator.Update(Time.deltaTime);

    }
}
