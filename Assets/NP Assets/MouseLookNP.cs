using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MouseLookNP : MonoBehaviour
{
    private float X;
    private float Y;

    public float Sensitivity;

    void Awake()
    {
        Vector3 euler = transform.rotation.eulerAngles;
        X = euler.x;
        Y = euler.y;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        const float MIN_X = 0.0f;
        const float MAX_X = 360.0f;
        const float MIN_Y = -90.0f;
        const float MAX_Y = 90.0f;

        X += Input.GetAxis("Mouse X") * (Sensitivity * Time.deltaTime);
        if (X < MIN_X) X += MAX_X;
        else if (X > MAX_X) X -= MAX_X;
        Y -= Input.GetAxis("Mouse Y") * (Sensitivity * Time.deltaTime);
        if (Y < MIN_Y) Y = MIN_Y;
        else if (Y > MAX_Y) Y = MAX_Y;

        transform.rotation = Quaternion.Euler(Y, X, 0.0f);
    }

}
