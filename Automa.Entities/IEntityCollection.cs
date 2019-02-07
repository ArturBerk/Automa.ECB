namespace Automa.Entities
{
    public delegate void EntityAddedHandler<in TEntity>(TEntity entity);
    public delegate void EntityRemovedHandler<in TEntity>(TEntity entity);

    public interface IEntityCollection
    {
        int Count { get; }
        object this[int index] { get; }
        void Add(object entity);
        void Remove(object entity);
        void AddReferenced<TReference>(IEntity<TReference> referenced, object entity) where TReference : IEntity<TReference>;
    }

    public interface IEntityCollection<TEntity> : IEntityCollection where TEntity : class
    {
        new TEntity this[int index] { get; }
        void Add<T>(T entity) where T : TEntity, IEntity<TEntity>;
        void Remove<T>(T entity) where T : TEntity, IEntity<TEntity>;
        void AddReferenced<TReference>(IEntity<TReference> referenced, TEntity entity) where TReference : IEntity<TReference>;
        void AddReferenced<TReference>(TReference entity) where TReference : TEntity, IEntity<TReference>;
        TEntity[] ToArray();
        event EntityAddedHandler<TEntity> Added;
        event EntityRemovedHandler<TEntity> Removed;
    }
}