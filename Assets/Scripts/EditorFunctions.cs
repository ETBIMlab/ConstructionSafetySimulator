using UnityEngine;
using UnityEditor;

public class EditModeFunctions : EditorWindow
{
    GameObject source;
    [MenuItem("Window/Edit Mode Functions")]
    public static void ShowWindow()
    {
        GetWindow<EditModeFunctions>("Edit Mode Functions");
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        source = (GameObject) EditorGUILayout.ObjectField(source, typeof(GameObject), true);
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("Shrink Colliders"))
        {
            ShinkColliders(source.transform);
        }
    }

    private void ShinkColliders(Transform trans)
    {
        Debug.Log("The function ran on: " + trans.name);
        BoxCollider box = null;
        CapsuleCollider cap = null;
        if((box = trans.GetComponent<BoxCollider>()) != null)
        {
            box.size *=  0.94f;
        } else if ((cap = trans.GetComponent<CapsuleCollider>()) != null)
        {
            cap.radius *= 0.94f;
            cap.height *= 0.94f;
        }

        foreach (Transform t in trans)
            ShinkColliders(t);
    }
}