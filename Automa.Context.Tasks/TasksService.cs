using System;
using Automa.Tasks;

namespace Automa.Context.Tasks
{
    public class TasksService : TaskManager, IService
    {
        public void OnAttach(IContext context)
        {
        }

        public void OnDetach(IContext context)
        {
            Dispose();
        }
    }
}
