using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

[ExecuteInEditMode]
public class SceneControl : MonoBehaviour
{
    //ResetVector: holds data needed to reset a given sub-scene
    [System.Serializable]
    public class ResetVector
    {
        [Tooltip("The root path of the scene.")]
        public string sceneRoot;
        [Tooltip("Name of the scene to reset to (without '.unity' suffix).")]
        public string sceneName;
        [Tooltip("The keyboard key that will reset this scene.")]
        public KeyCode keyCode;
        [Tooltip("The number you wish to identify this scene with.")]
        public int sceneNumber;
        [Tooltip("GameObject transform used as an initial position for the player in the scene.")]
        public Transform InitialTransform;
    }

    [Tooltip("This is the Player GameObject.")]
    public GameObject Player;
    [Tooltip("This is a reference to the GameObject that holds all of the VR related subsystems.")]
    public GameObject SteamVR_Object;
    [Tooltip("This is a referenct to the GameObject that is used when no VR systems are present.")]
    public GameObject NoVR_Object;
    
    GameObject activeFrame; //used generically for resetting the player to an initial position
    Vector3 initPos;        //stores the game start initial position for the activeFrame
    Vector3 initAngles;     //stores the game start initial rotation for the activeFrame

    [SerializeField]
    public List<ResetVector> ResetVectors;  //list containing all of the reset vectors for the main scene

    //Setup: performs initialization of the activeFrame and initial positioning data on game start
    void Setup()
    {
        if (SteamVR_Object.activeInHierarchy)
        {
            Debug.Log("SceneReset.Setup(): activeFrame set to SteamVR_Object");
            activeFrame = SteamVR_Object;
        }
        else
        {
            Debug.Log("SceneReset.Setup(): activeFrame set to NoVR_Object");
            activeFrame = NoVR_Object;
        }
        initPos = activeFrame.transform.localPosition;
        initAngles = activeFrame.transform.localEulerAngles;
        Debug.Log("SceneReset.Setup(): initial position: " + initPos.ToString());
        Debug.Log("SceneReset.Setup(): initial euler angles: " + initAngles.ToString());
    }

    //START: loads the sub-scenes when the main scene is loaded
    void Start()
    {
        Setup();

        foreach (ResetVector resetVector in ResetVectors)
        {
            Debug.Log("Loading scene: " + resetVector.sceneName);
#if UNITY_EDITOR
            EditorSceneManager.OpenScene(resetVector.sceneRoot + resetVector.sceneName + ".unity", OpenSceneMode.Additive);
#else
            SceneManager.LoadScene(resetVector.scene, LoadSceneMode.Additive);
#endif
        }
    }


    // Update is called once per frame
    void Update()
    {
        foreach (ResetVector resetVector in ResetVectors)
        {
            if (Input.GetKey(resetVector.keyCode))
            {
                SceneControlVars.currentScene = resetVector.sceneNumber;
                ResetScene(resetVector);
                break;
            }
        }
    }

    void ResetScene(ResetVector resetVector)
    {
        Debug.Log("SceneReset.ResetScene(): destroying scene " + resetVector.sceneName);
        bool sceneDestroyed = SceneManager.UnloadScene(SceneManager.GetSceneByName(resetVector.sceneName));
        if (sceneDestroyed)
        {
            Debug.Log("SceneReset.ResetScene(): loading scene" + resetVector.sceneName);
            SceneManager.LoadScene(resetVector.sceneName, LoadSceneMode.Additive);

            Player.transform.position = resetVector.InitialTransform.position;
            Player.transform.eulerAngles = resetVector.InitialTransform.eulerAngles;

            activeFrame.transform.localPosition = initPos;
            activeFrame.transform.localEulerAngles = initAngles;
        }
    }
}
