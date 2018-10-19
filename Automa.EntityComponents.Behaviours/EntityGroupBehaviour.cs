using Automa.Context;

namespace Automa.EntityComponents.Behaviours
{
    public abstract class EntityGroupBehaviour<TEntityGroup> : EntityComponentBehaviour 
        where TEntityGroup : EntityGroup, new()
    {
        private readonly TEntityGroup entityGroup;

        protected EntityGroupBehaviour()
        {
            entityGroup = CreateGroup();
        }

        protected virtual TEntityGroup CreateGroup()
        {
            return new TEntityGroup();
        }

        public override void OnAttach(IContext context)
        {
            base.OnAttach(context);
            EntityManager.BindGroup(entityGroup);
        }

        public override void Apply()
        {
            ApplyToEntities(entityGroup, entityGroup.Entity.EvaluateCount);
        }

        protected abstract void ApplyToEntities(TEntityGroup entity, int count);
    }

    public class EntityGroup : IGroup
    {
        public IArray<Entity> Entity;
    }
}
