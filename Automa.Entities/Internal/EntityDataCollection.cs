using System.Diagnostics;
using Automa.Common;

namespace Automa.Entities.Internal
{
    internal class EntityDataCollection<TEntity> : IEntityDataCollection<TEntity>
    {
        private static ArrayList<StructReference> referencePool = new ArrayList<StructReference>(4);

        private ArrayList<TEntity> entities = new ArrayList<TEntity>(4);
        private ArrayList<StructReference> references = new ArrayList<StructReference>(4);

        public int Count => entities.Count;

        public ref TEntity this[int index] => ref entities.Buffer[index];

        public event EntityDataAddedHandler<TEntity> Added;
        public event EntityDataRemovedHandler<TEntity> Removed;

        public void Add<T>(ref T entity) where T : struct, TEntity, IEntity<TEntity>
        {
            if (entity.Reference != null)
                throw new EntitiesException("Entity already added to entity group");
            var reference = GetReference();
            reference.Index = entities.Count;
            references.SetAt(reference.Index, reference);
            entity.Reference = reference;
            entities.SetAt(reference.Index, entity);
            Added?.Invoke(ref entities.Buffer[reference.Index]);
        }

        public void Remove<T>(ref T entity) where T : struct, TEntity, IEntity<TEntity>
        {
            if (entity.Reference == null)
                throw new EntitiesException("Entity not added to entity group");
            var reference = (StructReference)entity.Reference;
            Remove(reference);
            entity.Reference = null;
        }

        private void Remove(StructReference reference)
        {
            var removeEntity = entities.Buffer[reference.Index];
            if (references.UnorderedRemoveAt(reference.Index))
            {
                references[reference.Index].Index = reference.Index;
            }
            entities.UnorderedRemoveAt(reference.Index);
            reference.Index = -1;
            referencePool.Add(reference);
            Removed?.Invoke(ref removeEntity);
        }

        private StructReference GetReference()
        {
            StructReference reference = null;
            if (referencePool.Count > 0)
            {
                reference = referencePool.Buffer[referencePool.Count - 1];
                referencePool.Buffer[referencePool.Count - 1] = null;
                --referencePool.Count;
            }
            else
            {
                reference = new StructReference();
            }
            reference.Collection = this;
            return reference;
        }

        [DebuggerDisplay("{" + nameof(Index) + "} at {typeof(TEntity).Name}")]
        private class StructReference : IEntityReference<TEntity>
        {
            public EntityDataCollection<TEntity> Collection;
            public int Index;
            public ref TEntity Entity => ref Collection.entities[Index];

            public override string ToString()
            {
                return $"{Index} at {typeof(TEntity).Name}";
            }

            public void Dispose()
            {
                ((IEntity<TEntity>)Entity).Reference = null;
                Collection.Remove(this);
            }
        }
    }

}