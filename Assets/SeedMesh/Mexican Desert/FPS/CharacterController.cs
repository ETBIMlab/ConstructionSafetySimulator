using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float speed = 10;
    public float sprintSpeed = 5;
    float baseSpeed;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        baseSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        float translation = Input.GetAxis("Vertical") * speed;
        float straffe = Input.GetAxis("Horizontal") * speed;
        translation *= Time.deltaTime;
        straffe *= Time.deltaTime;

        transform.Translate(straffe, 0, translation);

        if (Input.GetKeyDown("escape"));
        Cursor.lockState = CursorLockMode.None;

        if (Input.GetKeyDown("left shift"))
        {
            speed += sprintSpeed;
        } else if (Input.GetKeyUp("left shift"))
        {
            speed = baseSpeed;
        }
    }
}
