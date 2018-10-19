namespace Automa.Entities
{
    public interface IEntityReference<TEntity>
    {
        ref TEntity Entity { get; }
    }
}