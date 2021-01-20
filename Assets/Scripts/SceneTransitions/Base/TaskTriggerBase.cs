namespace SceneTransitions
{
    public interface ITriggerTasks
    {
        bool RequirementsMet { get; }
        void SetRequirementsMet(bool met = true);
    }

    public abstract class TaskTriggerBase : ITriggerTasks
    {
        public bool RequirementsMet { get; set; }
        protected TaskTriggerBase()
        {
            this.RequirementsMet = false;
        }

        public void SetRequirementsMet(bool met = true)
        {
            this.RequirementsMet = met;
        }
    }
}