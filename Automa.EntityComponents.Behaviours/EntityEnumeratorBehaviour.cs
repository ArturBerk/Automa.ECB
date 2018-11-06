using Automa.Context;
using Automa.EntityComponents.Internal;

namespace Automa.EntityComponents.Behaviours
{
    public abstract class EntityIteratorBehaviour<TEntityIterator> : EntityComponentBehaviour 
        where TEntityIterator : EntityIterator, new()
    {
        private readonly TEntityIterator entityEnumerator;

        protected EntityIteratorBehaviour()
        {
            entityEnumerator = CreateIterator();
        }

        protected virtual TEntityIterator CreateIterator()
        {
            return new TEntityIterator();
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

        protected abstract void ApplyToEntities(TEntityIterator entity);
    }
}
