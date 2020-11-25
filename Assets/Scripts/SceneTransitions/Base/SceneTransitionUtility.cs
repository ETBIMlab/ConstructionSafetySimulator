using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SceneTransitions
{
    public enum SceneTransitioner
    {
        SceneOneToSceneTwo = 0
    }

    public static class SceneTransitionerExtensions
    {
        private static SceneTransitionSingletons Singletons = new SceneTransitionSingletons();

        public static ISelectScenes GetSceneTransitioner(this SceneTransitioner sceneTransitioner)
        {
            ISelectScenes res = null;
            switch (sceneTransitioner)
            {
                case SceneTransitioner.SceneOneToSceneTwo:
                    res = Singletons.SceneOneToSceneTwo;
                    break;
            }
            return res;
        }
    }

    public class SceneTransitionSingletons
    {
        private ISelectScenes sceneOneToSceneTwo = null;
        public ISelectScenes SceneOneToSceneTwo
        {
            get
            {
                if (sceneOneToSceneTwo == null)
                {
                    sceneOneToSceneTwo = new DefaultSceneTransition("SampleScene2");
                }
                return sceneOneToSceneTwo;
            }
        }
    }
}
