using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerHeightManager))]
[CanEditMultipleObjects]
public class PlayerHeightManagerEditor : Editor
{
    SerializedProperty playerHeightManager;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Set Height"))
        {
            (target as PlayerHeightManager).SetPlayerHeight();
        }

        if (GUILayout.Button("Set Arm Length"))
        {
            (target as PlayerHeightManager).SetPlayerArmLength();
        }
    }
}
