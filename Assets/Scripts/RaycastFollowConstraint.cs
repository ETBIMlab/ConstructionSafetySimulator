using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RaycastFollowConstraint : MonoBehaviour
{

    [SerializeField]
    private Transform castPositionAndDirection;
    [SerializeField]
    private float range;
    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private bool active;

    private RaycastHit hitdata;

    private void Start()
    {
        if (active)
        {
            if (castPositionAndDirection == null)
            {
                Debug.LogError("Cannot activate raycast follow constraint without a transform. Disabling constraint...");
                active = false;
            }
            if (range < 0.0f)
            {
                Debug.LogError("Cannot activate raycast follow constraint without a transform. Disabling constraint...");
                active = false;
            }
        }
    }


    void Update()
    {
        if (active)
            if (Physics.Raycast(castPositionAndDirection.position, castPositionAndDirection.forward, out hitdata, range, layerMask))
            {
                transform.position = hitdata.point;
                transform.eulerAngles = hitdata.normal;
            }
            else
            {
                transform.position = castPositionAndDirection.position + castPositionAndDirection.forward * range;
                transform.eulerAngles = Vector3.RotateTowards(transform.eulerAngles, castPositionAndDirection.forward * -1, 5 * Time.deltaTime, 0.0f);
            }
    }
}
