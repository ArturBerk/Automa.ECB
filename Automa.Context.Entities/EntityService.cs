using Automa.Entities;

namespace Automa.Context.Entities
{
    public class EntityService : EntityGroup, IService
    {
        public virtual void OnAttach(IContext context)
        {
        }

        public virtual void OnDetach(IContext context)
        {
        }
    }
}