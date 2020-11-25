using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneRestore : MonoBehaviour // One Scene Restore object restores one scene
{
    public GameObject[] objectsToSave;  // the array of objects whose positions we will save
    public Transform respawnPoint;      // links with PlayerDeathHandler, where the player will be teleported when they die

    List<Vector3> positions;            // the list of positions of the objects we're storing
    List<Quaternion> rotations;         // the list of rotations of the objects we're storing
    List<Rigidbody> rigidbodies;        // the list of rigidbodies of the objects we're storing (if they have one)

    void Start()    // the positions, rotations, and rigidbodies are saved at the start of runtime
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
        plD.AddStateToPlayer(this); // automatically adds the state to the player
    }

    void resetVelocity(Rigidbody obj) { // resets the velocity of the rigidbody component of the object
        obj.velocity = Vector3.zero;
        obj.angularVelocity = Vector3.zero;
    }

    public void ResetScene() {  // this is the main function, resets all the objects.
        int i = 0;
        foreach (GameObject obj in objectsToSave) {
            obj.transform.position = positions[i];
            obj.transform.rotation = rotations[i];
            i++;
        }

        rigidbodies.ForEach(resetVelocity);
    }

    
    public void SceneCompleted() {  // if we complete a task and no longer want to be able to reset it, you can call this function to remove this from the PlayerDeathHandler
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerDeathHandler plD = player.GetComponent<PlayerDeathHandler>();
        plD.RemoveStateFromPlayer(this);
    }
}
