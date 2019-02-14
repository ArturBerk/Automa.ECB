namespace Automa.Entities
{
    public interface IEntity
    {
        bool IsRegistered { get; }
        void Register(IEntityGroup entityGroup);
        void Unregister();
    }

    public interface IEntity<TEntity> : IEntity
    {
        IEntityReference<TEntity> Reference { get; set; }
    }
}