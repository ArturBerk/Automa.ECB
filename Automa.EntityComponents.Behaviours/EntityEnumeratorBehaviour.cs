using Automa.Context;
using Automa.EntityComponents.Internal;

namespace Automa.EntityComponents.Behaviours
{
    public abstract class EntityEnumeratorBehaviour<TEntityEnumerator> : EntityComponentBehaviour 
        where TEntityEnumerator : EntityEnumerator, new()
    {
        private readonly TEntityEnumerator entityEnumerator;

        protected EntityEnumeratorBehaviour()
        {
            entityEnumerator = CreateEnumerator();
        }

        protected virtual TEntityEnumerator CreateEnumerator()
        {
            return new TEntityEnumerator();
        }

        public override void OnAttach(IContext context)
        {
            base.OnAttach(context);
            EntityManager.BindEnumerator(entityEnumerator);
        }

        public override void Apply()
        {
            entityEnumerator.Reset();
            ApplyToEntities(entityEnumerator);
        }

        protected abstract void ApplyToEntities(TEntityEnumerator entity);
    }
}
