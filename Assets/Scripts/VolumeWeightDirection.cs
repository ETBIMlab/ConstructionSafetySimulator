using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Volume))]
public class VolumeWeightDirection : MonoBehaviour
{
    public Transform playerCam;
    public Transform target;
    public float minAngle = 0;
    public float maxAngle = 360;
    [Range(0,10)]
    public float maxDecSpeed = 0.1f;
    [Range(0,10)]
    public float maxIncSpeed = 0.1f;
    // Linear..

    private Volume _volume;
    // Start is called before the first frame update
    void Start()
    {
        _volume = GetComponent<Volume>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetDir = target.position - playerCam.position;
        float angle = Vector3.Angle(targetDir, playerCam.forward);
        angle = Mathf.Clamp(angle, minAngle, maxAngle);


        _volume.weight += Mathf.Clamp(1 - Mathf.InverseLerp(minAngle, maxAngle, angle) - (float) _volume.weight, -maxDecSpeed, maxIncSpeed) * Time.deltaTime;
    }
}
