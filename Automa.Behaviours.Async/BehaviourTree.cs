using System;
using Automa.Tasks;

namespace Automa.Behaviours.Async
{
    public class BehaviourTree
    {
        public static IBehaviourTreeBuilder Builder => new BehaviourTreeBuilder();

        private readonly ApplyingTask applyingTask;

        internal BehaviourTreeGroup[] Groups;
        internal IMainThreadBehaviour[] MainThreadBehaviours;
        internal IBehaviour[] SyncBehaviours;

        internal BehaviourTree(BehaviourTreeGroup[] groups, IBehaviour[] syncBehaviours,
            IMainThreadBehaviour[] mainThreadBehaviours)
        {
            Groups = groups;
            SyncBehaviours = syncBehaviours;
            MainThreadBehaviours = mainThreadBehaviours;
            foreach (var behaviourTreeGroup in groups)
            {
                behaviourTreeGroup.Tasks = new BehaviourTask[behaviourTreeGroup.Behaviours.Length];
                for (var i = 0; i < behaviourTreeGroup.Tasks.Length; i++)
                {
                    behaviourTreeGroup.Tasks[i] = new BehaviourTask(behaviourTreeGroup.Behaviours[i]);
                }
            }
            applyingTask = new ApplyingTask(this);
        }

        public IApplyingHandler Start(TaskManager tasks)
        {
            applyingTask.tasks = tasks;
            tasks.Schedule(applyingTask);
            return applyingTask;
        }

        internal class BehaviourTreeGroup
        {
            public IAsyncBehaviour[] Behaviours;
            public IDependency[] Dependencies;
            public BehaviourTask[] Tasks;
        }

        private class ApplyingTask : Task, IApplyingHandler
        {
            private BehaviourTree behaviourTree;
            public TaskManager tasks;

            public ApplyingTask(BehaviourTree behaviourTree)
            {
                this.behaviourTree = behaviourTree;
            }

            public void Complete()
            {
                Wait();
                foreach (var behaviourTreeMainThreadBehaviour in behaviourTree.MainThreadBehaviours)
                {
                    behaviourTreeMainThreadBehaviour.Apply();
                }
            }

            public void Complete(TimeSpan timeSpan)
            {
                Wait(timeSpan);
                foreach (var behaviourTreeMainThreadBehaviour in behaviourTree.MainThreadBehaviours)
                {
                    behaviourTreeMainThreadBehaviour.Apply();
                }
            }

            public override void Execute()
            {
                foreach (var behaviourTreeGroup in behaviourTree.Groups)
                {
                    foreach (var behaviourTask in behaviourTreeGroup.Tasks)
                    {
                        tasks.Schedule(behaviourTask);
                    }
                    behaviourTreeGroup.Tasks.WaitAll();
                }
                foreach (var behaviourTreeSyncBehaviour in behaviourTree.SyncBehaviours)
                {
                    behaviourTreeSyncBehaviour.Apply();
                }
            }
        }

        internal class BehaviourTask : Task
        {
            private readonly IAsyncBehaviour behaviour;

            public BehaviourTask(IAsyncBehaviour behaviour)
            {
                this.behaviour = behaviour;
            }

            public override void Execute()
            {
                behaviour.ApplyAsync();
            }
        }

        public interface IApplyingHandler
        {
            void Complete();
            void Complete(TimeSpan timeSpan);
        }
    }
}