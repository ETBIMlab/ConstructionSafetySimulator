using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Collections;

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
    [Tooltip("This is an optional Animation to play during each transition. If the first parameter is a bool, that bool will be updated to reflect if the scene is loaded.")]
    public Animator TransitionAnimator;
    [Tooltip("This is an optional delay before trying to transition scenes. Set to a negative number to calculate the longest clip length.")]
    public float SceneChangeDelay;

    private void OnValidate()
    {
        if(SceneChangeDelay < 0)
        {
            SceneChangeDelay = 0;
            if(TransitionAnimator)
            {
                foreach(AnimationClip ac in TransitionAnimator.runtimeAnimatorController.animationClips)
                {
                    if (ac.length > SceneChangeDelay)
                        SceneChangeDelay = ac.length;
                }
            }
        }
    }
    
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
        if(!mutex)
        {
            Debug.Log("Cannot transition scenes while in the middle of another scene transition");
        }
        else
        {
            mutex = false;
            if(TransitionAnimator)
            {
                TransitionAnimator.enabled = true;
                TransitionAnimator.Play(0, -1, 0);
                if (TransitionAnimator.parameters.Length > 0)
                    if (TransitionAnimator.parameters[0].type == AnimatorControllerParameterType.Bool)
                        TransitionAnimator.SetBool(TransitionAnimator.parameters[0].name, false);
            }
            yield return new WaitForSeconds(SceneChangeDelay);

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

            if (TransitionAnimator)
            {
                if(TransitionAnimator.parameters.Length > 0)
                    if(TransitionAnimator.parameters[0].type == AnimatorControllerParameterType.Bool)
                        TransitionAnimator.SetBool(TransitionAnimator.parameters[0].name, true);
            }
            yield return new WaitForSeconds(SceneChangeDelay);
            mutex = true;
        }
    }
}
