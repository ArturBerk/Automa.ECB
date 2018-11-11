using System.Threading;

namespace Automa.Tasks
{
    public abstract class Task
    {
        internal TaskManager currentTaskManager;
        internal ManualResetEventSlim Completed { get; } = new ManualResetEventSlim(false);
        protected TaskManager CurrentTaskManager => currentTaskManager;

        public abstract void Execute();

        public void Wait()
        {
            Completed.Wait();
        }
    }
}