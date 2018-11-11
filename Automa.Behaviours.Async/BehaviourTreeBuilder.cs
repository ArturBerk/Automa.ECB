using System.Collections.Generic;
using System.Linq;

namespace Automa.Behaviours.Async
{
    public class BehaviourTreeBuilder
    {
        public BehaviourTree Build(IBehaviourGroup group)
        {
            List<IAsyncBehaviour> asyncBehaviours = new List<IAsyncBehaviour>();
            List<IBehaviour> syncBehaviours = new List<IBehaviour>();
            List<IMainThreadBehaviour> mainThreadBehaviours = new List<IMainThreadBehaviour>();
            GatherBehaviours(group, asyncBehaviours, syncBehaviours, mainThreadBehaviours);

            List< BehaviourTreeGroupBuilder > groups = new List<BehaviourTreeGroupBuilder>();
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
                    groups.Add(new BehaviourTreeGroupBuilder()
                    {
                        Dependencies = new List<IDependency>(asyncBehaviourDependencies),
                        Behaviours = new List<IAsyncBehaviour>()
                        {
                            asyncBehaviour
                        }
                    });
                }
            }

            return new BehaviourTree(groups.Select(builder => new BehaviourTree.BehaviourTreeGroup()
            {
                Dependencies = builder.Dependencies.ToArray(),
                Behaviours = builder.Behaviours.ToArray()
            }).ToArray(), syncBehaviours.ToArray(), mainThreadBehaviours.ToArray());
        }

        private class BehaviourTreeGroupBuilder
        {
            public List<IDependency> Dependencies;
            public List<IAsyncBehaviour> Behaviours;

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

        private static void GatherBehaviours(IBehaviourGroup group,
            List<IAsyncBehaviour> asyncBehaviours,
            List<IBehaviour> syncBehaviours,
            List<IMainThreadBehaviour> mainThreadBehaviours)
        {
            foreach (var groupBehaviour in group.Behaviours)
            {
                if (groupBehaviour is IAsyncBehaviour asyncBehaviour)
                {
                    asyncBehaviours.Add(asyncBehaviour);
                    continue;
                }
                if (groupBehaviour is IMainThreadBehaviour mainThreadBehaviour)
                {
                    mainThreadBehaviours.Add(mainThreadBehaviour);
                    continue;
                }
                if (groupBehaviour is IBehaviourGroup)
                {
                    GatherBehaviours(group, asyncBehaviours, syncBehaviours, mainThreadBehaviours);
                }
                else
                {
                    syncBehaviours.Add(groupBehaviour);
                }
            }
        }
    }
}