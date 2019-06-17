using System.Collections.Generic;
using System.Reflection;
using Automa.Common;

namespace Automa.Behaviours
{
    public class BehaviourGroup : IBehaviourGroup
    {
        private ArrayList<IBehaviour> behaviours = new ArrayList<IBehaviour>(4);

        public BehaviourGroup(string name)
        {
            Name = name;
        }

        public IEnumerable<IBehaviour> Behaviours => behaviours;

        public string Name { get; }

        public virtual void Add(IBehaviour behaviour)
        {
            var behaviourType = behaviour.GetType();
            for (var i = 0; i < behaviours.Count; i++)
            {
                var behaviour1 = behaviours[i];
                foreach (var afterAttribute in behaviour1.GetType().GetCustomAttributes<AfterAttribute>())
                {
                    if (afterAttribute.Type.IsAssignableFrom(behaviourType))
                    {
                        // insert here
                        behaviours.Insert(i, behaviour);
                        return;
                    }
                }
            }
            behaviours.Add(behaviour);
        }

        public virtual void AddRange(IEnumerable<IBehaviour> newBehaviours)
        {
            foreach (var behaviour in newBehaviours)
            {
                Add(behaviour);
            }
        }

        public virtual bool Remove(IBehaviour slot)
        {
            for (var i = 0; i < behaviours.Count; i++)
            {
                var behaviourSlot = behaviours[i];
                if (!ReferenceEquals(behaviourSlot, slot)) continue;
                behaviours.RemoveAt(i);
                return true;
            }
            return false;
        }

        public bool IsEnabled { get; set; } = true;

        public void Apply()
        {
            if (!IsEnabled) return;
            for (var i = 0; i < behaviours.Count; i++)
            {
                if (behaviours[i].IsEnabled) behaviours[i].Apply();
            }
        }
    }
}