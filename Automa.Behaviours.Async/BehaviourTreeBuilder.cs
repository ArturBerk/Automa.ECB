using System;
using System.Collections.Generic;
using System.Linq;

namespace Automa.Behaviours.Async
{
    public interface IBehaviourTreeBuilder
    {
        IBehaviourTreeBuilder With<T>(T[] behaviours) where T : IBehaviour;
        IBehaviourTreeBuilder With<T>(IEnumerable<T> behaviours) where T : IBehaviour;
        IBehaviourTreeBuilder With<T>(T groupBehaviour) where T : IBehaviour;
        IBehaviourTreeBuilder With(IBehaviourGroup groupBehaviour);
        BehaviourTree Build();
    }

    internal class BehaviourTreeBuilder : IBehaviourTreeBuilder
    {
        public static BehaviourTreeBuilder New => new BehaviourTreeBuilder();

        private readonly List<IAsyncBehaviour> asyncBehaviours = new List<IAsyncBehaviour>();
        private readonly List<IMainThreadBehaviour> mainThreadBehaviours = new List<IMainThreadBehaviour>();
        private readonly List<IBehaviour> syncBehaviours = new List<IBehaviour>();

        public IBehaviourTreeBuilder With<T>(params T[] behaviours) where T : IBehaviour
        {
            for (var index = 0; index < behaviours.Length; index++)
            {
                var behaviour = behaviours[index];
                AddBehaviour(behaviour);
            }
            return this;
        }

        public IBehaviourTreeBuilder With<T>(IEnumerable<T> behaviours) where T : IBehaviour
        {
            foreach (var behaviour in behaviours)
            {
                AddBehaviour(behaviour);
            }
            return this;
        }

        public IBehaviourTreeBuilder With<T>(T groupBehaviour) where T : IBehaviour
        {
            AddBehaviour(groupBehaviour);
            return this;
        }

        private void AddBehaviour(IBehaviour groupBehaviour)
        {
            if (groupBehaviour is IAsyncBehaviour asyncBehaviour)
            {
                asyncBehaviours.Add(asyncBehaviour);
                return;
            }
            if (groupBehaviour is IMainThreadBehaviour mainThreadBehaviour)
            {
                mainThreadBehaviours.Add(mainThreadBehaviour);
                return;
            }
            if (groupBehaviour is IBehaviourGroup group)
            {
                GatherBehaviours(@group);
            }
            else
            {
                syncBehaviours.Add(groupBehaviour);
            }
        }

        public IBehaviourTreeBuilder With(IBehaviourGroup group)
        {
            GatherBehaviours(group);
            return this;
        }

        public BehaviourTree Build()
        {
            var groups = new List<BehaviourTreeGroupBuilder>();

            foreach (var asyncBehaviour in asyncBehaviours)
            {
                var placedInGroup = false;
                var asyncBehaviourDependencies = asyncBehaviour.Dependencies.ToArray();
                foreach (var behaviourTreeGroupBuilder in groups)
                {
                    if (asyncBehaviourDependencies.All(dependency =>
                        !behaviourTreeGroupBuilder.HasDependencyConflict(dependency)))
                    {
                        behaviourTreeGroupBuilder.Behaviours.Add(asyncBehaviour);
                        placedInGroup = true;
                        break;
                    }
                }
                if (!placedInGroup)
                {
                    groups.Add(new BehaviourTreeGroupBuilder
                    {
                        Dependencies = new List<IDependency>(asyncBehaviourDependencies),
                        Behaviours = new List<IAsyncBehaviour>
                        {
                            asyncBehaviour
                        }
                    });
                }
            }

            return new BehaviourTree(groups.Select(builder => new BehaviourTree.BehaviourTreeGroup
            {
                Dependencies = builder.Dependencies.ToArray(),
                Behaviours = builder.Behaviours.ToArray()
            }).ToArray(), syncBehaviours.ToArray(), mainThreadBehaviours.ToArray());
        }

        private void GatherBehaviours(IBehaviourGroup group)
        {
            foreach (var groupBehaviour in group.Behaviours)
            {
                AddBehaviour(groupBehaviour);
            }
        }

        private class BehaviourTreeGroupBuilder
        {
            public List<IAsyncBehaviour> Behaviours;
            public List<IDependency> Dependencies;

            public bool HasDependencyConflict(IDependency dependency)
            {
                for (var index = 0; index < Dependencies.Count; index++)
                {
                    var localDependency = Dependencies[index];
                    if (localDependency.Type == dependency.Type &&
                        (localDependency.Mode == DependencyMode.Modify || dependency.Mode == DependencyMode.Modify))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}