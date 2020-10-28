using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;


// @TODO: Add a "Look At" point so the player is always facing the correct direction when they respawn
public class PlayerDeathHandler : MonoBehaviour
{
    public Material PlayerVisionFade;
    public int timeToDie; // 85 is a good value for this.
    public int timeToRespawn; // 100 is a good value for this.

    // these have to be public for some reason, or else we get an error in "AddStateToPlayer"
    public List<GameObject> stateSavers;
    public List<SceneRestore> respawnPoints;

    int listSize;
    public bool callDeathCycleDirectly;

    Color VisionFadeColor;

    private void Update() {
        if (callDeathCycleDirectly) {
            Debug.Log("CALLING DEATH CYCLE DIRECTLY");
            callDeathCycleDirectly = false;
            StartDeathCycle();
        }
    }

    private void Start() {
        VisionFadeColor = new Color(1, 1, 1, 1);
        Shader.SetGlobalColor("_VisionFadeColor", VisionFadeColor);

    }

    public void AddStateToPlayer(SceneRestore resPoint) {
        stateSavers.Add(resPoint.gameObject);
        respawnPoints.Add(resPoint);
        listSize++;
    }

    public void RemoveStateFromPlayer(SceneRestore resPoint) {
        stateSavers.Remove(resPoint.gameObject);
        respawnPoints.Remove(resPoint);
        listSize--;
    }

    public void StartDeathCycle() {
        Camera.main.GetComponent<CameraOverride>().EnableCameraOverride(PlayerVisionFade);

        if (PlayerVisionFade == null) {
            Debug.LogError("Can't run death cycle on Player Object, PlayerVisionFade material no attached!");
            return;
        }

        float minDist = 99999;
        int currIndex = 0;
        int minDistIndex = 0;


        if (listSize > 0) {
            foreach (SceneRestore resPoint in respawnPoints) {
                float dist = 0;
                
                if(Camera.main.GetComponent<FallbackCameraController>() != null) {
                    dist = Vector3.Distance(resPoint.respawnPoint.position, Camera.main.transform.position);
                } else {
                    dist = Vector3.Distance(resPoint.respawnPoint.position, this.transform.position);
                }


                if (dist < minDist) {
                    minDist = dist;
                    minDistIndex = currIndex;
                }

                currIndex++;
            }
        }
        

        StartCoroutine("DeathCycle", minDistIndex);
    }


    IEnumerator DeathCycle(int minDistIndex) {
        float fadeColor = timeToDie;

        for (int i = 0; i < timeToDie; i++) {
            fadeColor = fadeColor / timeToDie;
            //Debug.Log("COLOR: " + fadeColor);
            

            VisionFadeColor.r = fadeColor; VisionFadeColor.g = fadeColor; VisionFadeColor.b = fadeColor;
            Debug.Log(VisionFadeColor.ToString());
            Shader.SetGlobalColor("_VisionFadeColor", VisionFadeColor);

            Debug.Log("DYING");
            fadeColor = fadeColor * timeToDie;
            fadeColor = fadeColor - 1;
            yield return new WaitForSeconds(.025f);
        }


        if(listSize > 0) {
            if (Camera.main.GetComponent<FallbackCameraController>() != null) {
                Camera.main.transform.position = respawnPoints[minDistIndex].respawnPoint.position;
            } else {
                this.transform.position = respawnPoints[minDistIndex].respawnPoint.position;
            }

            respawnPoints[minDistIndex].ResetScene();
        }

        VisionFadeColor.r = 0; VisionFadeColor.g = 0; VisionFadeColor.b = 0;
        Shader.SetGlobalColor("_VisionFadeColor", VisionFadeColor);

        StartCoroutine("FixVision");
    }

    IEnumerator FixVision() {
        StopCoroutine("DeathCycle");
        float fadeColor = 0;

        yield return new WaitForSeconds(1.5f);


        for (int i = 0; i < timeToRespawn; i++) {
            fadeColor = fadeColor / timeToRespawn;
            //Debug.Log("COLOR: " + fadeColor);

            VisionFadeColor.r = fadeColor; VisionFadeColor.g = fadeColor; VisionFadeColor.b = fadeColor;
            Shader.SetGlobalColor("_VisionFadeColor", VisionFadeColor);

            Debug.Log("RESPAWNING");
            fadeColor = fadeColor * timeToRespawn;
            fadeColor = fadeColor + 1;
            yield return new WaitForSeconds(.025f);
        }

        Camera.main.GetComponent<CameraOverride>().DisableCameraOverride();

    }
}
