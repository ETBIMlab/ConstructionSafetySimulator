using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Volume))]
public class VolumeWeightDirection : MonoBehaviour
{
    [Header("Transforms")]
    [Tooltip("The tranform that resembles the current \"gaze\" direction")]
    public Transform playerCam;
    [Tooltip("The position of the object causing the glaze. Will most often be the sun light soure")]
    public Transform target;

    [Header("Angles")]
    [Tooltip("The angle at which the sun glare starts")]
    [Range(0,360)]
    public float minAngle = 0;
    [Tooltip("The angle at which the sun glare is alway at it's max")]
    [Range(0, 360)]
    public float maxAngle = 360;
    [Tooltip("Determines the distribution of the weight between min and max angle")]
    public AnimationCurve angleBlendMethod;

    [Header("Durations")]
    [Tooltip("The amount of time it takes to go from \"Full glare\" to \"No glare\"")]
    [Range(0,10)]
    public float maxDecTime = 0.1f;
    [Tooltip("The amount of time it takes to go from \"No glare\" to \"Full glare\"")]
    [Range(0,10)]
    public float maxIncTime = 0.1f;
    [Tooltip("Determines the distribution of the change in weight between min and max time")]
    public AnimationCurve timeBlendMethod;

    // Required Components
    private Volume _volume;

    void Start()
    {
        _volume = GetComponent<Volume>();
    }

    void Update()
    {
        // Calculate the Gaze 

        float angle = Vector3.Angle(
            target.position - playerCam.position, // Target Direction
            playerCam.forward); // Current Direction

        angle = Mathf.Clamp(angle, minAngle, maxAngle);

        // This calculates how much the weight should change. 
        _volume.weight += Mathf.Clamp(
            1 - angleBlendMethod.Evaluate(Mathf.InverseLerp(minAngle, maxAngle, angle)) - (float)_volume.weight,
            (Time.deltaTime > maxDecTime) ? -1 : -timeBlendMethod.Evaluate(Mathf.InverseLerp(0, maxDecTime, Time.deltaTime)),
            (Time.deltaTime > maxIncTime) ? 1 : timeBlendMethod.Evaluate(Mathf.InverseLerp(0, maxIncTime, Time.deltaTime))
        );
        
    }
}
