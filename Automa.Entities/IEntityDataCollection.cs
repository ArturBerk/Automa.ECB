namespace Automa.Entities
{
    public delegate void EntityDataAddedHandler<TEntity>(ref TEntity entity);
    public delegate void EntityDataRemovedHandler<TEntity>(ref TEntity entity);

    public interface IEntityDataCollection
    {
        int Count { get; }
        void Clear();
    }

    public interface IEntityDataCollection<TEntity> : IEntityDataCollection
    {
        ref TEntity this[int index] { get; }
        void Add<T>(ref T entity) where T : struct, TEntity, IEntity<TEntity>;
        void Remove<T>(ref T entity) where T : struct, TEntity, IEntity<TEntity>;
        event EntityDataAddedHandler<TEntity> Added;
        event EntityDataRemovedHandler<TEntity> Removed;
    }
}