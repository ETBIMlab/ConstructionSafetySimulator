using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMover : MonoBehaviour   // BEWARE: Currently only works with objects that use Box Colliders
{
    public GameObject objectToMove;
    public GameObject destination;

    Transform objTransform;
    Transform destTransform;

    Vector3 destNoY;

    public float speedOfMotion;

    public bool easeInAndOut;

    RaycastHit hitInfo;

    bool moving;
    float halfHeight;

    private void Start() {
        moving = false;

        objTransform = objectToMove.transform;
        destTransform = destination.transform;

        if(GetComponent<BoxCollider>() != null) {
            halfHeight = GetComponent<BoxCollider>().size.y / 2;
        } else {
            halfHeight = 0;
        }
    }

    private void Update() {
        if (moving) {
            Ray ray = new Ray(objTransform.position, -objTransform.up);

            if(Physics.Raycast (ray, out hitInfo)) {
                objTransform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            }


            if(Vector3.Distance(objTransform.position, destTransform.position) <= 0.05) {
                moving = false;
            }

            if (easeInAndOut) {
                destNoY = Vector3.Slerp(objTransform.position, destTransform.position, speedOfMotion * Time.deltaTime);  // Slerp interpolates between two positions, moving slower near the beginning and the end
            } else {
                destNoY = Vector3.Lerp(objTransform.position, destTransform.position, speedOfMotion * Time.deltaTime);  // Lerp interpolates between two positions at a constant rate
            }


            destNoY.y = hitInfo.point.y + halfHeight;
            objTransform.position = destNoY;
        }
    }

    private void OnTriggerEnter(Collider other) {   // Activates when a rigidbody enters the collider on this gameObject, if the gameObject has 'isTrigger' enabled
        moving = true;
    }

}
