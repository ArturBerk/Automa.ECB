using Automa.EntityComponents;

namespace Automa.Context.Entities
{
    public class EntityService : EntityManager, IService
    {
        public virtual void OnAttach(IContext context)
        {
        }

        public virtual void OnDetach(IContext context)
        {
        }
    }
}