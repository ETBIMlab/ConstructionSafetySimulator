namespace TaskTriggers
{
    public interface ITriggerTasks
    {
        bool RequirementsMet { get; set; }
        void Interact();
    }

    public abstract class TaskTriggerBase : ITriggerTasks
    {
        public bool RequirementsMet { get; set; }
        public TaskTriggerBase()
        {
            this.RequirementsMet = false;
        }

        public abstract void Interact();
    }
}