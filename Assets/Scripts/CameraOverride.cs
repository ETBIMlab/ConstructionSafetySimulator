using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraOverride : MonoBehaviour
{
    bool camOverride = false;
    Material localMat;

    public void EnableCameraOverride(Material mat) {
        camOverride = true;
        localMat = mat;
    }

    public void DisableCameraOverride() {
        camOverride = false;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (!camOverride) {
            Graphics.Blit(source, destination);
            return;
        }

        Graphics.Blit(source, destination, localMat);
    }

}
