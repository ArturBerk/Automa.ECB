namespace Automa.Entities
{
    public interface IEntity
    {
    }

    public interface IEntity<TEntity>
    {
        IEntityReference<TEntity> Reference { get; }
    }
}