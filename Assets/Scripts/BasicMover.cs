using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @TODO: Add functionality beyond box collider (not super important, can place a game object on an empty box collider)
// @TODO: Allow weighting to bezier points, for a 'tighter' curve (one that passes closer to the points)
public class BasicMover : MonoBehaviour   // BEWARE: Currently only works with objects that use Box Colliders
{
    public GameObject objectToMove;     // the script is on the trigger, so we need a reference to the object we're moving
    public GameObject[] destinations;   // the list of destinations

    Transform objTransform;

    Vector3 destNoY;                    // currently, we remove the y component of the transform, and raycast down, getting the y component and normal vector of the terrain
    Vector3[] destPositions;            // the physical Vector3 positions of the GameObjects located in the destinations array

    public float speedOfMotion;         // the speed of the motion

    public bool useBezierCurve;         // when false, just linearly interpolates between 2 points

    RaycastHit hitInfo;                 // the raycast info to get the normal vector of the terrain

    bool moving;        // moving is a discrete action that should start and stop. We have a bool to help cut out math calculations when moving is not happening
    float halfHeight;
    
    int currDest;       // index of current destination for the linear interpolation
    int last;           // used to add objects to our destPositions array

    float t;            // the linear interpolation value, goes from 0 to 1. At 0 we are at the first point, and 1 we are at the next point (or last point when using bezierCurve)

    private void Start() {
        moving = false;    
        t = 0;

        objTransform = objectToMove.transform;

        destPositions = new Vector3[destinations.Length + 1];   // we need an extra point for the object's start position, so we do destinations.Length + 1
        destPositions[0] = objTransform.position;               // the first destination is our object's starting position
  

        currDest = 1;
        last = 0;

        foreach (GameObject obj in destinations) {
            last++;
            destPositions[last] = obj.transform.position;
        }

        if (GetComponent<BoxCollider>() != null) {              // this would need to be modified to support other colliders
            halfHeight = GetComponent<BoxCollider>().size.y / 2;
        } else {
            halfHeight = 0;
        }
    }

    private void Update() {
        if (moving) {
            if (!useBezierCurve) {
                Ray ray = new Ray(objTransform.position, -objTransform.up);

                if (Physics.Raycast(ray, out hitInfo)) {
                    objTransform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal); // rotates the object to the normal vector of the terrain
                }


                if (Vector3.Distance(objTransform.position, destPositions[last]) <= 0.15) { // if we're very lose to the last position, we can stop moving
                    moving = false;
                    return;
                }

                if (Vector3.Distance(objTransform.position, destPositions[currDest]) <= 0.15) { // if we're very close to the next position, we can start moving towards that position
                    currDest++;
                    t = 0;
                }

                destNoY = Vector3.Lerp(destPositions[currDest - 1], destPositions[currDest], t);  // Lerp interpolates between two positions at a constant rate

                t += speedOfMotion * Time.deltaTime;

                destNoY.y = hitInfo.point.y + halfHeight;   // set the y component to the position of the terrain + half the height of the box collider
                objTransform.LookAt(destNoY);               // look forward
                objTransform.position = destNoY;            // set position to the modified position
            } else {

                destNoY = getBezierPoint(t, destPositions);
                destNoY.y = hitInfo.point.y + halfHeight;
                objTransform.position = destNoY;

                t += speedOfMotion/4.5f * Time.deltaTime * destinations.Length; // the t value is between the first and last point for bezier curves, so we calculate speed differently

                objTransform.LookAt(getBezierPoint(t, destPositions));
                if (t >= 1) {
                    moving = false;
                }
            }            
        }
    }

    private void OnTriggerEnter(Collider other) {   // Activates when a rigidbody enters the collider on this gameObject, if the gameObject has 'isTrigger' enabled
        moving = true;
    }





    // ----- Bezier implementation ----- 

    Vector3 getBezierPoint(float t, Vector3[] contPoints) { // this one is hard to fully explain in comments. Look up De Casteljau's Algorithm
        Vector3 pt = new Vector3(0, 0, 0);

        int degree = contPoints.Length - 1;

        int i = 0;

        foreach (Vector3 pointOnLine in contPoints) {
            Debug.DrawLine(pointOnLine, pointOnLine + 3 * Vector3.up, Color.blue);
            pt += binCoef(degree, i) *
                localPow(t, i) *
                localPow(1 - t, degree - i) *
                pointOnLine;

            i++;
        }

        Debug.DrawLine(pt, pt + 3 * Vector3.up, Color.red);
        return pt;
    }


    int binCoef(int degree, int num) {  // calculates the binomial coefficient of the given degree at the position of 'num' so degree 3 would give: (num=0) = 1, (num=1) = 3, (num=2) = 3, (num=3) = 1
        if (degree == 0 && num == 0) {
            return 1;
        }

        return localFac(degree) / (localFac(num) * localFac(degree - num));
    }

    float localPow(float toPow, int num) { // recursively calculates toPow^num. Might be faster to use built-in functions, but I wanted to understand each part of the process
        if (num == 0)
            return 1;
        else
            return toPow * localPow(toPow, num - 1);
    }

    int localFac(int num) { // recursively calculates the factorial of a number. Again, built-in functions might be faster, but I wanted control of the process
        if (num == 0)
            return 1;
        else
            return (num * localFac(num - 1));
    }
}
