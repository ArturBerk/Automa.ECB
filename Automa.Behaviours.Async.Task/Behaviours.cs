using System;
using System.Collections.Generic;
using System.Threading;

namespace Automa.Behaviours.Async.Task
{
    internal static class ExecutionOrder
    {
        public static List<IBehaviour> Executed = new List<IBehaviour>();
    }

    internal class MainThreadBehaviour2 : TestBehaviour, IMainThreadBehaviour
    {
    }

    internal class MainThreadBehaviour1 : TestBehaviour, IMainThreadBehaviour
    {
    }

    internal class SyncBehaviour2 : TestBehaviour
    {
    }

    internal class SyncBehaviour1 : TestBehaviour
    {
    }

    internal class TestBehaviour : IBehaviour
    {
        public bool Executed;
        public int ExecutedThread;

        public virtual void Apply()
        {
            Executed = true;
            ExecutedThread = Thread.CurrentThread.ManagedThreadId;
            lock (ExecutionOrder.Executed)
            {
                ExecutionOrder.Executed.Add(this);
            }
        }
    }

    internal abstract class TestAsyncBehaviour : TestBehaviour, IAsyncBehaviour
    {
        public abstract IEnumerable<IDependency> Dependencies { get; }

        public override void Apply()
        {
            double count = 0;
            for (int i = 0; i < 100000; i++)
            {
                count = Math.Sin(i);
            }
            base.Apply();
        }

        public void ApplyAsync()
        {
            Apply();
        }
    }

    internal class AsyncBehaviourModify1 : TestAsyncBehaviour
    {
        public override IEnumerable<IDependency> Dependencies
        {
            get { yield return Dependency<Dependency1>.Modify; }
        }
    }

    internal class AsyncBehaviourRead1 : TestAsyncBehaviour
    {
        public override IEnumerable<IDependency> Dependencies
        {
            get { yield return Dependency<Dependency1>.ReadOnly; }
        }
    }

    internal class AsyncBehaviourModify2 : TestAsyncBehaviour
    {
        public override IEnumerable<IDependency> Dependencies
        {
            get { yield return Dependency<Dependency2>.Modify; }
        }
    }

    internal class AsyncBehaviourWithoutDependencies : TestBehaviour, IAsyncBehaviour
    {
        public IEnumerable<IDependency> Dependencies { get; }

        public void ApplyAsync()
        {
            Apply();
        }
    }

    internal class Dependency1
    {
    }

    internal class Dependency2
    {
    }
}