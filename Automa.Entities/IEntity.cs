namespace Automa.Entities
{
    public interface IEntity<TEntity>
    {
        IEntityReference<TEntity> Reference { get; set; }
    }
}