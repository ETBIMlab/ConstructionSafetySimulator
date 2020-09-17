using UnityEngine;

namespace Scenes
{
    public class SampleSceneTrigger : MonoBehaviour
    {
        public static ISelectScenes SampleSceneTransitionSingleton = new SceneOneToSceneTwo("SampleScene2");

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            SampleSceneTransitionSingleton.ChangeScene();
        }

        public class SceneOneToSceneTwo : SceneTriggerBase
        {
            public SceneOneToSceneTwo(string sceneName)
                : base(sceneName)
            { }
        }
    }
}

