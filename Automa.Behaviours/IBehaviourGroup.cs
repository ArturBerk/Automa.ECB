using System.Collections.Generic;

namespace Automa.Behaviours
{
    public interface IBehaviourGroup : IBehaviour
    {
        string Name { get; }
        IEnumerable<IBehaviour> Behaviours { get; }
        void Add(IBehaviour slot);
        void AddRange(IEnumerable<IBehaviour> newBehaviours);
        bool Remove(IBehaviour slot);
    }
}