using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMover : MonoBehaviour   // BEWARE: Currently only works with objects that use Box Colliders
{
    public GameObject objectToMove;
    //public GameObject destination;
    public GameObject[] destinations;

    Transform objTransform;
    //Transform destTransform;

    Vector3 destNoY;
    Vector3[] destPositions;

    public float speedOfMotion;

    bool easeInAndOut;

    RaycastHit hitInfo;

    bool moving;
    float halfHeight;
    
    int currDest;
    int last;

    float t;

    private void Start() {
        moving = false;
        easeInAndOut = false;
        t = 0;

        objTransform = objectToMove.transform;
        //destTransform = destination.transform;

        destPositions = new Vector3[destinations.Length + 1];
        destPositions[0] = objTransform.position;


        currDest = 1;
        last = 0;

        foreach (GameObject obj in destinations) {
            last++;
            destPositions[last] = obj.transform.position;
        }

        if (GetComponent<BoxCollider>() != null) {
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


            if(Vector3.Distance(objTransform.position, destPositions[last]) <= 0.15) {
                moving = false;
                return;
            }

            if (Vector3.Distance(objTransform.position, destPositions[currDest]) <= 0.15) {
                currDest++;
                t = 0;
            }

            if (easeInAndOut) {
                destNoY = Vector3.Slerp(destPositions[currDest - 1], destPositions[currDest], t);  // Slerp interpolates between two positions, moving slower near the beginning and the end
            } else {
                destNoY = Vector3.Lerp(destPositions[currDest - 1], destPositions[currDest], t);  // Lerp interpolates between two positions at a constant rate
            }

            t += speedOfMotion * Time.deltaTime;

            destNoY.y = hitInfo.point.y + halfHeight;
            objTransform.LookAt(destNoY);
            objTransform.position = destNoY;
            
        }
    }

    private void OnTriggerEnter(Collider other) {   // Activates when a rigidbody enters the collider on this gameObject, if the gameObject has 'isTrigger' enabled
        moving = true;
    }

}
