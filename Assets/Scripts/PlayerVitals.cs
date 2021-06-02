using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVitals : MonoBehaviour
{
    [Range(60, 180)]
    public int HeartRate = 60;

    [Range(0.0f, 1.0f)]
    public float HeartBeatIntensity = 0.0f;

    [Range(0.0f, 1.0f)]
    public float BloodOxygenPercent = 1.0f;

    [Range(0.0f, 1.0f)]
    public float Hydration = 1.0f;


    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
