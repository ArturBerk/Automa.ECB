using System.Collections.Generic;

namespace Automa.Behaviours
{
    public interface IBehaviourGroup : IBehaviour
    {
        string Name { get; }
        IEnumerable<IBehaviour> Behaviours { get; }
        void Add(IBehaviour slot);
        bool Remove(IBehaviour slot);
    }
}