using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Automa.Tasks
{
    public class TaskManager : IDisposable
    {
        private readonly ManualResetEventSlim taskCompleted = new ManualResetEventSlim(false);
        private readonly WorkerThread[] threadPool;
        private readonly BlockingCollection<Task> tasks = new BlockingCollection<Task>();
        private long activeTasks;

        public TaskManager(int count = 0)
        {
            if (count == 0)
            {
                count = Environment.ProcessorCount - 1;
                if (count <= 0) count = 1;
            }
            threadPool = new WorkerThread[count];
            for (var i = 0; i < count; i++)
            {
                threadPool[i] = new WorkerThread(this);
            }
        }

        public void Dispose()
        {
            foreach (var workerThread in threadPool)
            {
                workerThread.Dispose();
            }
        }

        public void Schedule(Task task)
        {
            //while (true)
            //{
                task.currentTaskManager = this;
                task.Completed.Reset();
                Interlocked.Increment(ref activeTasks);
                tasks.Add(task);
                //break;
            //}
        }

        public void WaitAll()
        {
            if (Interlocked.Read(ref activeTasks) == 0) return;
            while (true)
            {
                taskCompleted.Wait();
                if (Interlocked.Read(ref activeTasks) == 0) break;
            }
        }

        public event Action<Exception> UnhandledException;

        private void RaiseUnhandledException(Exception e)
        {
            UnhandledException?.Invoke(e);
        }

        private class WorkerThread : IDisposable
        {
            private readonly TaskManager tasksManager;
            private readonly Thread thread;

            public WorkerThread(TaskManager tasksManager)
            {
                this.tasksManager = tasksManager;
                thread = new Thread(Run);
                thread.Start();
            }

            public void Dispose()
            {
                thread.Interrupt();
            }

            private void Run()
            {
                while (true)
                {
                    var task = tasksManager.tasks.Take();
                    if (task != null)
                    {
                        try
                        {
                            task.Execute();
                            task.Completed.Set();
                        }
                        catch (Exception e)
                        {
                            tasksManager.RaiseUnhandledException(e);
                        }
                    }
                    Interlocked.Decrement(ref tasksManager.activeTasks);
                    tasksManager.taskCompleted.Set();
                }
            }
        }
    }
}