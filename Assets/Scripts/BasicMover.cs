using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @TODO: Add functionality beyond box collider
// @TODO: Allow weighting to bezier points, for a 'tighter' curve (one that passes closer to the points)
public class BasicMover : MonoBehaviour   // BEWARE: Currently only works with objects that use Box Colliders
{
    public GameObject objectToMove;
    public GameObject[] destinations;

    Transform objTransform;

    Vector3 destNoY;
    Vector3[] destPositions;

    public float speedOfMotion;

    public bool useBezierCurve;

    RaycastHit hitInfo;

    bool moving;
    float halfHeight;
    
    int currDest;
    int last;

    float t;

    private void Start() {
        moving = false;
        t = 0;

        objTransform = objectToMove.transform;

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
            if (!useBezierCurve) {
                Ray ray = new Ray(objTransform.position, -objTransform.up);

                if (Physics.Raycast(ray, out hitInfo)) {
                    objTransform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
                }


                if (Vector3.Distance(objTransform.position, destPositions[last]) <= 0.15) {
                    moving = false;
                    return;
                }

                if (Vector3.Distance(objTransform.position, destPositions[currDest]) <= 0.15) {
                    currDest++;
                    t = 0;
                }

                destNoY = Vector3.Lerp(destPositions[currDest - 1], destPositions[currDest], t);  // Lerp interpolates between two positions at a constant rate

                t += speedOfMotion * Time.deltaTime;

                destNoY.y = hitInfo.point.y + halfHeight;
                objTransform.LookAt(destNoY);
                objTransform.position = destNoY;
            } else {

                destNoY = getBezierPoint(t, destPositions);
                destNoY.y = hitInfo.point.y + halfHeight;
                objTransform.position = destNoY;

                t += speedOfMotion/4.5f * Time.deltaTime * destinations.Length;

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

    Vector3 getBezierPoint(float t, Vector3[] contPoints) {
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


    int binCoef(int degree, int num) {
        if (degree == 0 && num == 0) {
            return 1;
        }

        return localFac(degree) / (localFac(num) * localFac(degree - num));
    }

    float localPow(float toPow, int num) {
        if (num == 0)
            return 1;
        else
            return toPow * localPow(toPow, num - 1);
    }

    int localFac(int num) {
        if (num == 0)
            return 1;
        else
            return (num * localFac(num - 1));
    }
}
