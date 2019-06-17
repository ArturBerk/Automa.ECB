namespace Automa.Entities
{
    public delegate void DataAddedHandler<TEntity>(ref TEntity entity);
    public delegate void DataRemovedHandler<TEntity>(ref TEntity entity);

    public interface IDataCollection<TEntity> : IEntityDataCollection where TEntity : struct
    {
        ref TEntity this[int index] { get; }
        void Add(TEntity entity);
        void Add(ref TEntity entity);
        event DataAddedHandler<TEntity> Added;
        event DataRemovedHandler<TEntity> Removed;
    }
}