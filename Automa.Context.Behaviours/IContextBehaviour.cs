using Automa.Behaviours;

namespace Automa.Context.Behaviours
{
    public interface IContextBehaviour : IBehaviour
    {
        void OnAttach(IContext context);
        void OnDetach(IContext context);
    }
}
