using UnityEngine;

namespace SceneTransitions
{
    public class SampleSceneTrigger : MonoBehaviour
    {
        public SceneTransitioner SceneToTransitionTo;

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
            SceneToTransitionTo.GetSceneTransitioner().Transition();
        }
    }
}

