using UnityEngine;
using Valve.VR;

/// <summary>
/// Allows SteamVR_LoadLevel component to reference a scene instead of just a string. This has the sole purpose of allowing scenes to be renamed without having to keep track of any name strings that are referencing that scene.
/// </summary>
[RequireComponent(typeof(SteamVR_LoadLevel))]
public class LoadLevelManager : MonoBehaviour
{
#if UNITY_EDITOR
    [Tooltip("Scene Asset reference to the scene that should be loaded through the SteamVR_LoadLevel component.")]
    public UnityEditor.SceneAsset scene;
    [Tooltip("Set to true for the scene name to also be applied to the gameobject")]
    public bool updateGameObjectName = false;
    private void OnValidate()
    {
        if (scene != null)
        {
            GetComponent<SteamVR_LoadLevel>().levelName = scene.name;
            if (updateGameObjectName)
                gameObject.name = "LoadLevel [" + scene.name + "]";
        }
        else if (updateGameObjectName)
            gameObject.name = "LoadLevel [NULL]";
    }
#endif
}
