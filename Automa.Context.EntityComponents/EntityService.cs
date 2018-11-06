using Automa.EntityComponents;

namespace Automa.Context.EntityComponents
{
    public class EntityService : EntityManager, IService, IUpdateService
    {
        public virtual void OnAttach(IContext context)
        {
        }

        public virtual void OnDetach(IContext context)
        {
        }

        public void Update()
        {
            Dispatch();
        }
    }
}