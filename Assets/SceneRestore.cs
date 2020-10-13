using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneRestore : MonoBehaviour
{
    public GameObject[] objectsToSave;
    public bool userDied;

    List<Vector3> positions;
    List<Quaternion> rotations;
    List<Rigidbody> rigidbodies;

    void Start()
    {
        positions = new List<Vector3>();
        rotations = new List<Quaternion>();
        rigidbodies = new List<Rigidbody>();


        userDied = false;

        foreach (GameObject obj in objectsToSave) {
            positions.Add(obj.transform.position);
            rotations.Add(obj.transform.rotation);

            if(obj.GetComponent<Rigidbody>() != null) {
                rigidbodies.Add(obj.GetComponent<Rigidbody>());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (userDied) {

            int i = 0;
            foreach (GameObject obj in objectsToSave) {
                obj.transform.position = positions[i];
                obj.transform.rotation = rotations[i];
                i++;
            }

            rigidbodies.ForEach(resetVelocity);

            userDied = false;
        }

    }

    void resetVelocity(Rigidbody obj) {
        obj.velocity = Vector3.zero;
        obj.angularVelocity = Vector3.zero;
    }

}
