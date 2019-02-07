namespace Automa.Entities
{
    public interface IEntity { }

    public interface IEntity<TEntity> : IEntity
    {
        IEntityReference<TEntity> Reference { get; set; }
    }
}