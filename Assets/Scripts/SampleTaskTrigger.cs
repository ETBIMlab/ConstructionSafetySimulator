using Scenes;
using UnityEngine;

namespace TaskTriggers
{
    public class SampleTaskTrigger : MonoBehaviour
    {
        private TaskTriggerBase TaskTrigger { get; set; }
        
        // Start is called before the first frame update
        void Start()
        {
            this.TaskTrigger = new CollideWithBoxTrigger();
            SampleSceneTrigger.SampleSceneTransitionSingleton.TaskList.Add(this.TaskTrigger);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            this.TaskTrigger.Interact();
        }

        private class CollideWithBoxTrigger : TaskTriggerBase
        {
            public CollideWithBoxTrigger()
                : base () 
            { }

            public override void Interact()
            {
                this.RequirementsMet = true;
            }
        }
    }
}

