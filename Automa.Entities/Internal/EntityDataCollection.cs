using System.Diagnostics;
using Automa.Common;

namespace Automa.Entities.Internal
{
    internal class EntityDataCollection<TEntity> : IEntityDataCollection<TEntity> where TEntity : struct
    {
        private static ArrayList<StructReference> referencePool = new ArrayList<StructReference>(4);

        private ArrayList<TEntity> entities = new ArrayList<TEntity>(4);
        private ArrayList<StructReference> references = new ArrayList<StructReference>(4);

        public int Count => entities.Count;

        public ref TEntity this[int index] => ref entities.Buffer[index];

        public event EntityDataAddedHandler<TEntity> Added;
        public event EntityDataRemovedHandler<TEntity> Removed;

        public IEntityReference<TEntity> GetReference(int index)
        {
            return references[index];
        }

        public IEntityReference<TEntity> Add(ref TEntity entity)
        {
            var reference = GetReference();
            reference.Index = entities.Count;
            references.SetAt(reference.Index, reference);
            entities.SetAt(reference.Index, entity);
            Added?.Invoke(ref entities.Buffer[reference.Index]);
            return reference;
        }

        public void Remove(IEntityReference<TEntity> reference)
        {
            Remove((StructReference)reference);
        }

        private void Remove(StructReference reference)
        {
            var removeEntity = entities.Buffer[reference.Index];
            if (references.UnorderedRemoveAt(reference.Index))
            {
                references[reference.Index].Index = reference.Index;
            }
            entities.UnorderedRemoveAt(reference.Index);
            reference.Clear();
            referencePool.Add(reference);
            Removed?.Invoke(ref removeEntity);
        }

        public void Clear()
        {
            for (var index = 0; index < entities.Buffer.Length; index++)
            {
                var entity = entities.Buffer[index];
                Removed?.Invoke(ref entity);
            }

            for (int i = 0; i < references.Count; i++)
            {
                references[i].Clear();
            }
            referencePool.AddRange(references.Buffer, references.Count);
            references.Clear();
            entities.Clear();
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

            public void Clear()
            {
                Index = -1;
            }

            public override string ToString()
            {
                return $"{Index} at {typeof(TEntity).Name}";
            }

            public void Dispose()
            {
                if (Index == -1) return;
                ((IEntity<TEntity>)Entity).Reference = null;
                Collection.Remove(this);
            }
        }
    }

}