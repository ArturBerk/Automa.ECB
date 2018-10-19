using Automa.Context;
using Automa.Context.Behaviours;

namespace Automa.EntityComponents.Behaviours
{
    public abstract class EntityComponentBehaviour : IContextBehaviour
    {
        public EntityManager EntityManager { get; private set; }

        public virtual void OnAttach(IContext context)
        {
            EntityManager = context.GetService<EntityManager>();
        }

        public virtual void OnDetach(IContext context)
        {
        }

        public abstract void Apply();
    }
}