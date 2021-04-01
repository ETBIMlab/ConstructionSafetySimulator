using UnityEngine;
using UnityEditor;
using UnityEngine.Animations;

public class EditModeFunctions : EditorWindow
{
    GameObject go;
    float arg_float;
    int arg_int;

    
    [MenuItem("Window/Edit Mode Functions")]
    public static void ShowWindow()
    {
        GetWindow<EditModeFunctions>("Edit Mode Functions");
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("go ");
        go = (GameObject) EditorGUILayout.ObjectField(go, typeof(GameObject), true);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("arg_float ");
        arg_float = EditorGUILayout.FloatField(arg_float);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("arg_int ");
        arg_int = EditorGUILayout.IntField(arg_int);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        GUILayout.Label("ScaleColliders( go , arg_float ); Scales all colliders that are on any children of this object.");
        if (GUILayout.Button("Scale Colliders"))
        {
            ScaleColliders(go.transform, arg_float);
        }

        EditorGUILayout.Space();
        GUILayout.Label("CreateBasicRope( go , arg_int ); Creates a rope with n sections using 'go' as the particle prefab.");
        if (GUILayout.Button("Create Basic Rope"))
        {
            CreateBasicRope(go, arg_int);
        }
    }

    private void ScaleColliders(Transform trans, float scale)
    {
        Debug.Log("The function ran on: " + trans.name);
        BoxCollider box = null;
        CapsuleCollider cap = null;
        if((box = trans.GetComponent<BoxCollider>()) != null)
        {
            box.size *= scale;
        } else if ((cap = trans.GetComponent<CapsuleCollider>()) != null)
        {
            cap.radius *= scale;
            cap.height *= scale;
        }

        foreach (Transform t in trans)
            ScaleColliders(t, scale);
    }
    
    private void CreateBasicRope(GameObject prefab, int sections)
    {
        if(sections < 2 || sections > 100)
        {
            Debug.Log("Usage: sections must be less than 100 but greater than 2");
        }
        GameObject rope = new GameObject();
        rope.transform.position = Vector3.zero;
        rope.name = "BasicRope";

        GameObject temp;
        temp = Instantiate<GameObject>(prefab, Vector3.zero, Quaternion.Euler(0,0,0), rope.transform);
        temp.name = "start";
        
        for (int i = 0; i < sections - 2; i++)
        {

            temp = Instantiate<GameObject>(prefab, Vector3.zero, Quaternion.Euler(0, 0, 0), rope.transform);
            temp.name = "mid (" + i + ")";
        }

        temp = Instantiate<GameObject>(prefab, Vector3.zero, Quaternion.Euler(0, 0, 0), rope.transform);
        temp.name = "end";

        for(int i = 1; i < sections - 1; i++)
        {
            ParentConstraint pc = rope.transform.GetChild(i).gameObject.AddComponent<ParentConstraint>();
            //int connectDistance = (int) Mathf.Floor(Mathf.Log((    ((i % (sections / 2)) != i) ? sections - i : i)    + 1, 2));
            

            if (pc)
            {
                pc.AddSource(new ConstraintSource
                {
                    sourceTransform = rope.transform.GetChild(0),
                    weight = 1 - (((float) i) / ((float)(sections - 1)))
            });
                pc.AddSource(new ConstraintSource
                {
                    sourceTransform = rope.transform.GetChild(sections - 1),
                    weight = (((float)i) / ((float)(sections - 1)))
                });
                pc.constraintActive = true;
                pc.locked = true;
            }
        }
        

    }
}