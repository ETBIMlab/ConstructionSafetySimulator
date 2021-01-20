using System.Collections.Generic;
using UnityEngine;

namespace SceneTransitions
{
    public class SampleTaskTrigger : MonoBehaviour
    {
        private ITriggerTasks TaskTrigger { get; set; }

        public SceneTransitioner SceneToTransitionTo;

        // Start is called before the first frame update
        void Start()
        {
            this.TaskTrigger = new DefaultTaskTrigger();
            SceneToTransitionTo.GetSceneTransitioner().AddToTaskList(this.TaskTrigger);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            this.TaskTrigger.SetRequirementsMet();
        }
    }
}

