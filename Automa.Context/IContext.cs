namespace Automa.Context
{
    public interface IContext
    {
        T GetService<T>();
        void AttachService(object service);
        void DetachService(object service);
    }
}