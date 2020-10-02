using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMover : MonoBehaviour   // BEWARE: NOT TESTED YET
{
    public GameObject objectToMove;
    public GameObject destination;
    
    public float speedOfMotion;

    public bool easeInAndOut;

    RaycastHit hitInfo;

    bool moving;

    private void Start() {
        moving = false;
    }

    private void Update() {
        if (moving) {
            Ray ray = new Ray(objectToMove.transform.position, -objectToMove.transform.up);

            if(Physics.Raycast (ray, out hitInfo)) {
                objectToMove.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            }


            if(Vector3.Distance(objectToMove.transform.position, destination.transform.position) <= 0.05) {
                moving = false;
            }

            if (easeInAndOut) {
                objectToMove.transform.position = Vector3.Slerp(objectToMove.transform.position, destination.transform.position, speedOfMotion * Time.deltaTime);  // Slerp interpolates between two positions, moving slower near the beginning and the end
            } else {
                objectToMove.transform.position = Vector3.Lerp(objectToMove.transform.position, destination.transform.position, speedOfMotion * Time.deltaTime);  // Lerp interpolates between two positions at a constant rate
            }
            
        }
    }

    private void OnTriggerEnter(Collider other) {   // Activates when a rigidbody enters the collider on this gameObject, if the gameObject has 'isTrigger' enabled
        moving = true;
    }

}
