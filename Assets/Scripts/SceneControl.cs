using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif
using System.Collections;
using Valve.VR;


[RequireComponent(typeof(SteamVR_LoadLevel))]
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


    // RequiredComponent forces this to always be attached to the same object
    private SteamVR_LoadLevel VRLoader;

    GameObject activeFrame; //used generically for resetting the player to an initial position
    Vector3 initPos;        //stores the game start initial position for the activeFrame
    Vector3 initAngles;     //stores the game start initial rotation for the activeFrame

    [SerializeField]
    public List<ResetVector> ResetVectors;  //list containing all of the reset vectors for the main scene

    //Setup: performs initialization of the activeFrame and initial positioning data on game start
    
    void Start()
    {
        VRLoader = GetComponent<SteamVR_LoadLevel>();
        if (SteamVR_Object.activeInHierarchy)
        {
            //Debug.Log("SceneReset.Setup(): activeFrame set to SteamVR_Object");
            activeFrame = SteamVR_Object;
        }
        else
        {
            //Debug.Log("SceneReset.Setup(): activeFrame set to NoVR_Object");
            activeFrame = NoVR_Object;
        }
        initPos = activeFrame.transform.localPosition;
        initAngles = activeFrame.transform.localEulerAngles;
        //Debug.Log("SceneReset.Setup(): initial position: " + initPos.ToString());
        //Debug.Log("SceneReset.Setup(): initial euler angles: " + initAngles.ToString());

        foreach (ResetVector resetVector in ResetVectors)
        {
            if(SceneManager.GetSceneByPath(resetVector.sceneRoot + resetVector.sceneName + ".unity").name == null)
            {
                SceneManager.LoadScene(resetVector.sceneName, LoadSceneMode.Additive);
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        foreach (ResetVector resetVector in ResetVectors)
        {
            if (Input.GetKeyDown(resetVector.keyCode))
            {
                SceneControlVars.currentScene = resetVector.sceneNumber;

                StartCoroutine(ResetScene(resetVector));
                break;
            }
        }
    }

    private bool mutex = true;

    private IEnumerator ResetScene(ResetVector resetVector)
    {
        if (!mutex)
        {
            Debug.Log("Cannot transition scenes while in the middle of another scene transition");
        }
        else
        {
            mutex = false;

            /*
            Debug.Log("SceneReset.ResetScene(): destroying scene " + resetVector.sceneName);
            AsyncOperation asyncLoad;
            asyncLoad = SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(resetVector.sceneName));
            while (!asyncLoad.isDone)
                yield return null;

            Debug.Log("SceneReset.ResetScene(): loading scene" + resetVector.sceneName);
            asyncLoad = SceneManager.LoadSceneAsync(resetVector.sceneName, LoadSceneMode.Additive);
            Player.transform.position = resetVector.InitialTransform.position;
            Player.transform.eulerAngles = resetVector.InitialTransform.eulerAngles;
            activeFrame.transform.localPosition = initPos;
            activeFrame.transform.localEulerAngles = initAngles;
            
            while (!asyncLoad.isDone)
                yield return null;
            
            yield return new WaitForSeconds(SceneChangeDelay);
            */

            //Debug.Log("SceneReset.ResetScene(): loading scene" + resetVector.sceneName);
            VRLoader.levelName = resetVector.sceneName;
            VRLoader.Trigger();
            

            while (SteamVR_LoadLevel.fading)
                yield return null;

            SceneControlVars.currentScene = resetVector.sceneNumber;
            Player.transform.position = resetVector.InitialTransform.position;
            Player.transform.eulerAngles = resetVector.InitialTransform.eulerAngles;
            activeFrame.transform.localPosition = initPos;
            activeFrame.transform.localEulerAngles = initAngles;

            while (SteamVR_LoadLevel.loading)
                yield return null;

            //Debug.Log("Finished.");
            mutex = true;
        }
    }

    private void OnValidate()
    {
        if(EditorSceneManager.sceneCount != ResetVectors.Count + 1)
        {
            //Debug.Log("There is a scene that is not loaded. Trying to load...");
            // Trys to load in subsence if not loaded
            foreach (ResetVector resetVector in ResetVectors)
            {
                if(SceneManager.GetSceneByPath(resetVector.sceneRoot + resetVector.sceneName + ".unity").name == null)
                {
                    // @TODO:: Load scene automatically.
                    Debug.LogWarning("SceneControl:: Scene is not loaded '" + resetVector.sceneRoot + resetVector.sceneName + "'");
                }
            }
        }
    }
}
