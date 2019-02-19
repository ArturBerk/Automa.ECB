namespace Automa.Entities
{
    public delegate void EntityDataAddedHandler<TEntity>(ref TEntity entity);
    public delegate void EntityDataRemovedHandler<TEntity>(ref TEntity entity);

    public interface IEntityDataCollection
    {
        int Count { get; }
        void Clear();
    }

    public interface IEntityDataCollection<TEntity> : IEntityDataCollection where TEntity : struct
    {
        ref TEntity this[int index] { get; }
        IEntityReference<TEntity> GetReference(int index);
        IEntityReference<TEntity> Add(ref TEntity entity);
        void Remove(IEntityReference<TEntity> entity);
        event EntityDataAddedHandler<TEntity> Added;
        event EntityDataRemovedHandler<TEntity> Removed;
    }
}