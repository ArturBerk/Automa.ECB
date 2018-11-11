using System;

namespace Automa.Tasks
{
    public class ActionTask : Task
    {
        public Action Action { get; set; }

        public ActionTask(Action action)
        {
            Action = action;
        }

        public override void Execute()
        {
            Action();
        }
    }
}