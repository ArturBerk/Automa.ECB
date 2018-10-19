using System;
using Automa.Behaviours;

namespace Automa.Context.Behaviours
{
    public class BehavioursService : IService
    {
        public readonly ContextBehaviourGroup Root = new ContextBehaviourGroup("Root");

        public void OnAttach(IContext context)
        {
            throw new NotImplementedException();
        }

        public void OnDetach(IContext context)
        {
            throw new NotImplementedException();
        }
    }
}
