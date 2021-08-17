using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
[ExecuteInEditMode]
public class SetTextToSceneName : MonoBehaviour
{
    void Start()
    {
        GetComponent<Text>().text = SceneManager.GetActiveScene().name;
    }
}
