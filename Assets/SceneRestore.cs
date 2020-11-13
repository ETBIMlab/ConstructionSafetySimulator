using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneRestore : MonoBehaviour
{
    public GameObject[] objectsToSave;
    public Transform respawnPoint;

    List<Vector3> positions;
    List<Quaternion> rotations;
    List<Rigidbody> rigidbodies;

    void Start()
    {
        positions = new List<Vector3>();
        rotations = new List<Quaternion>();
        rigidbodies = new List<Rigidbody>();


        foreach (GameObject obj in objectsToSave) {
            positions.Add(obj.transform.position);
            rotations.Add(obj.transform.rotation);

            if(obj.GetComponent<Rigidbody>() != null) {
                rigidbodies.Add(obj.GetComponent<Rigidbody>());
            }
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if(player == null) {
            Debug.LogWarning("Can not find an object with the tag \"Player\"");
            return;
        } else if (respawnPoint == null) {
            Debug.LogWarning("A respawn has not been set for this SceneRestore on gameobject: " + this.gameObject);
            return;
        }
        PlayerDeathHandler plD = player.GetComponent<PlayerDeathHandler>();

        if(plD == null) {
            Debug.LogWarning("Can not find PlayerDeathHandler on the object with the tag \"Player\"");
            return;
        }
        plD.AddStateToPlayer(this);
    }

    void resetVelocity(Rigidbody obj) {
        obj.velocity = Vector3.zero;
        obj.angularVelocity = Vector3.zero;
    }

    public void ResetScene() {
        int i = 0;
        foreach (GameObject obj in objectsToSave) {
            obj.transform.position = positions[i];
            obj.transform.rotation = rotations[i];
            i++;
        }

        rigidbodies.ForEach(resetVelocity);
    }

    public void SceneCompleted() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerDeathHandler plD = player.GetComponent<PlayerDeathHandler>();
        plD.RemoveStateFromPlayer(this);
    }
}
