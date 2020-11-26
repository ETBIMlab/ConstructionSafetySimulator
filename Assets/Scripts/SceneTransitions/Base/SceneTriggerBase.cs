using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

namespace SceneTransitions
{
    public interface ISelectScenes
    {
        string SceneName { get; }
        void Transition();
        void AddToTaskList(ITriggerTasks task);
        void RemoveFromTaskList(ITriggerTasks task);
    }

    public abstract class SceneTriggerBase : ISelectScenes
    {
        private List<ITriggerTasks> TaskList { get; }
        public string SceneName { get; }

        protected SceneTriggerBase(string sceneName)
        {
            this.TaskList = new List<ITriggerTasks>();
            this.SceneName = sceneName;
        }

        public void AddToTaskList(ITriggerTasks task)
        {
            this.TaskList.Add(task);
        }

        public void RemoveFromTaskList(ITriggerTasks task)
        {
            this.TaskList.Remove(task);
        }

        public void Transition()
        {
            bool ok = TaskList.All(t => t.RequirementsMet);
            if (ok)
            {
                SceneManager.LoadScene(sceneName: $"{this.SceneName}");
            }
        }

        public virtual bool SceneTransitionMeetsRequirements()
        {
            return TaskList.All(t => t.RequirementsMet);
        }
    }
}

