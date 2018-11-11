using System;
using Automa.Tasks;

namespace Automa.Behaviours.Async
{
    public class BehaviourTree
    {
        internal class BehaviourTreeGroup
        {
            public IDependency[] Dependencies;
            public IAsyncBehaviour[] Behaviours;
            public BehaviourTask[] Tasks;
        }

        internal BehaviourTreeGroup[] Groups;
        internal IBehaviour[] SyncBehaviours;
        internal IMainThreadBehaviour[] MainThreadBehaviours;
        private ApplyingTask applyingTask;

        internal BehaviourTree(BehaviourTreeGroup[] groups, IBehaviour[] syncBehaviours, IMainThreadBehaviour[] mainThreadBehaviours)
        {
            Groups = groups;
            SyncBehaviours = syncBehaviours;
            MainThreadBehaviours = mainThreadBehaviours;
            foreach (var behaviourTreeGroup in groups)
            {
                behaviourTreeGroup.Tasks = new BehaviourTask[behaviourTreeGroup.Behaviours.Length];
                for (int i = 0; i < behaviourTreeGroup.Tasks.Length; i++)
                {
                    behaviourTreeGroup.Tasks[i] = new BehaviourTask(behaviourTreeGroup.Behaviours[i]);
                }
            }
            applyingTask = new ApplyingTask(this);
        }

        private class ApplyingTask : Task, IApplyingHandler
        {
            private BehaviourTree behaviourTree;
            public TaskManager tasks;

            public ApplyingTask(BehaviourTree behaviourTree)
            {
                this.behaviourTree = behaviourTree;
            }

            public override void Execute()
            {
                foreach (var behaviourTreeGroup in behaviourTree.Groups)
                {
                    foreach (var behaviourTask in behaviourTreeGroup.Tasks)
                    {
                        tasks.Schedule(behaviourTask);
                    }
                    tasks.WaitAll(behaviourTreeGroup.Tasks);
                }
                foreach (var behaviourTreeSyncBehaviour in behaviourTree.SyncBehaviours)
                {
                    behaviourTreeSyncBehaviour.Apply();
                }
            }

            public void Complete()
            {
                Completed.Wait();
                foreach (var behaviourTreeMainThreadBehaviour in behaviourTree.MainThreadBehaviours)
                {
                    behaviourTreeMainThreadBehaviour.Apply();
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

        public IApplyingHandler Start(TaskManager tasks)
        {
            applyingTask.tasks = tasks;
            tasks.Schedule(applyingTask);
            return applyingTask;
        }

        public interface IApplyingHandler
        {
            void Complete();
        }
    }
}
