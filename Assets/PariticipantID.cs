using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//OnGUI isn't performant. It's best to remove after use

public class PariticipantID : MonoBehaviour
{
    public bool deleteOnComplete = true;
    public string id;
    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        id = GUILayout.TextField(id, GUILayout.Width(200));

        //ok button
        if (GUILayout.Button("OK"))
        {
            Debug.Log("Set Id from GUI");
            CognitiveVR.Core.SetParticipantId(id);
            if (deleteOnComplete) { Destroy(gameObject); }
        }
        GUILayout.EndHorizontal();
    }
}
