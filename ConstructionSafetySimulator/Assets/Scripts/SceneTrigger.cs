using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TaskTriggers;
using System.Linq;

namespace Scenes
{
    public interface ISelectScenes
    {
        List<ITriggerTasks> TaskList { get; }
        string SceneName { get; }
        void ChangeScene();
    }

    public abstract class SceneTriggerBase : ISelectScenes
    {
        public List<ITriggerTasks> TaskList { get; }
        public string SceneName { get; }

        public SceneTriggerBase(string sceneName)
        {
            this.TaskList = new List<ITriggerTasks>();
            this.SceneName = sceneName;
        }

        public void ChangeScene()
        {
            bool ok = TaskList.All(t => t.RequirementsMet);
            if (ok)
            {
                SceneManager.LoadScene(sceneName: $"{this.SceneName}");
            }
        }
    }
}

