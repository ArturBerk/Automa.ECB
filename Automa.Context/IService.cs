namespace Automa.Context
{
    public interface IService
    {
        void OnAttach(IContext context);
        void OnDetach(IContext context);
    }
}