using System.Collections.Generic;
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
            behaviours.Add(behaviour);
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

        public void Apply()
        {
            for (var i = 0; i < behaviours.Count; i++)
            {
                behaviours[i].Apply();
            }
        }
    }
}