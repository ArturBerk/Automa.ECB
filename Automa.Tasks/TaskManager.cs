using System;
using System.Collections.Generic;
using System.Threading;

namespace Automa.Tasks
{
    public class TaskManager : IDisposable
    {
        private readonly ManualResetEventSlim taskCompleted = new ManualResetEventSlim(false);
        private readonly WorkerThread[] threadPool;
        private long activeTasks;
        private int currentIndex;

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

        public void Schedule(ITask task)
        {
            while (true)
            {
                task.Completed.Reset();
                var index = Interlocked.Increment(ref currentIndex);
                Interlocked.Increment(ref activeTasks);
                threadPool[index].Tasks.Enqueue(task);
                break;
            }
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

        public void WaitAll(IEnumerable<ITask> tasks)
        {
            foreach (var task in tasks)
            {
                if (!task.Completed.IsSet)
                {
                    task.Completed.Wait();
                }
            }
        }

        private class WorkerThread : IDisposable
        {
            public readonly BlockingQueue<ITask> Tasks = new BlockingQueue<ITask>();
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
                    var task = Tasks.Dequeue();
                    task.Execute();
                    task.Completed.Set();
                    Interlocked.Decrement(ref tasksManager.activeTasks);
                    tasksManager.taskCompleted.Set();
                }
            }
        }
    }
}