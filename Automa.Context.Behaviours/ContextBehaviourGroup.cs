using System.Collections.Generic;
using Automa.Behaviours;

namespace Automa.Context.Behaviours
{
    public class ContextBehaviourGroup : BehaviourGroup, IContextBehaviour
    {
        private IContext context;

        public ContextBehaviourGroup(string name) : base(name)
        {
        }

        public override void Add(IBehaviour behaviour)
        {
            base.Add(behaviour);
            if (context != null && behaviour is IContextBehaviour contextBehaviour)
            {
                contextBehaviour.OnAttach(context);
            }
        }

        public override void AddRange(IEnumerable<IBehaviour> newBehaviours)
        {
            base.AddRange(newBehaviours);
            foreach (var newBehaviour in newBehaviours)
            {
                if (context != null && newBehaviour is IContextBehaviour contextBehaviour)
                {
                    contextBehaviour.OnAttach(context);
                }
            }
        }

        public override bool Remove(IBehaviour behaviour)
        {
            var remove = base.Remove(behaviour);
            if (remove && context != null && behaviour is IContextBehaviour contextBehaviour)
            {
                contextBehaviour.OnAttach(context);
                return true;
            }
            return remove;
        }

        public void OnAttach(IContext context)
        {
            this.context = context;
            foreach (var behaviour in Behaviours)
            {
                if (behaviour is IContextBehaviour contextBehaviour)
                {
                    contextBehaviour.OnAttach(context);
                }
            }
        }

        public void OnDetach(IContext context)
        {
            foreach (var behaviour in Behaviours)
            {
                if (behaviour is IContextBehaviour contextBehaviour)
                {
                    contextBehaviour.OnDetach(context);
                }
            }
            this.context = null;
        }
    }
}