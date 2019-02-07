using System;
using Automa.Common;

namespace Automa.Entities.Internal
{
    internal interface IClassEntityCollection
    {
        void RemoveBlind(object reference);
    }

    internal class EntityCollection<TEntity> : IClassEntityCollection, IEntityCollection<TEntity> where TEntity : class
    {
        private static ArrayList<ClassReference> referencePool = new ArrayList<ClassReference>(4);

        private ArrayList<TEntity> entities = new ArrayList<TEntity>(4);
        private ArrayList<ClassReference> references = new ArrayList<ClassReference>(4);

        public int Count => entities.Count;

        object IEntityCollection.this[int index] => entities.Buffer[index];
        public TEntity this[int index] => entities.Buffer[index];

        public TEntity[] ToArray()
        {
            var result = new TEntity[entities.Count];
            Array.Copy(entities.Buffer, result, entities.Count);
            return result;
        }

        public event EntityAddedHandler<TEntity> Added;
        public event EntityRemovedHandler<TEntity> Removed;

        public void Add(object entity)
        {
            if (!(entity is TEntity typeEntity) || !(entity is IEntity<TEntity> entityWithReference))
            {
                throw new EntitiesException($"Entity must implement \"{typeof(TEntity)}\" and \"{typeof(IEntity<TEntity>)}\"");
            }
            if (entityWithReference.Reference != null)
                throw new EntitiesException("Entity already added to entity group");
            var reference = GetReference();
            reference.Index = entities.Count;
            references.SetAt(reference.Index, reference);
            entities.SetAt(reference.Index, typeEntity);
            entityWithReference.Reference = reference;
            Added?.Invoke(entities.Buffer[reference.Index]);
        }

        public void Remove(object entity)
        {
            if (!(entity is TEntity) || !(entity is IEntity<TEntity> entityWithReference))
            {
                throw new EntitiesException($"Entity must implement \"{typeof(TEntity)}\" and \"{typeof(IEntity<TEntity>)}\"");
            }
            if (entityWithReference.Reference == null)
                throw new EntitiesException("Entity not added to entity group");
            var classReference = (ClassReference)entityWithReference.Reference;
            if (classReference.IsChild)
                throw new EntitiesException("Can't remove referenced entity, remove root entity");
            Remove(classReference);
            entityWithReference.Reference = null;
        }

        public void Add<T>(T entity) where T : TEntity, IEntity<TEntity>
        {
            if (entity.Reference != null)
                throw new EntitiesException("Entity already added to entity group");
            var reference = GetReference();
            reference.Index = entities.Count;
            references.SetAt(reference.Index, reference);
            entities.SetAt(reference.Index, entity);
            entity.Reference = reference;
            Added?.Invoke(entities.Buffer[reference.Index]);
        }

        public void AddReferenced<TReference>(IEntity<TReference> referenced, object entity) where TReference : IEntity<TReference>
        {
            AddReferenced(referenced, (TEntity)entity);
        }

        public void AddReferenced<TReferenced>(IEntity<TReferenced> referenced, TEntity entity) where TReferenced : IEntity<TReferenced>
        {
            if (referenced.Reference == null)
                throw new EntitiesException("Entity not added to entity group");

            // Add entity
            var reference = GetReference();
            reference.Index = entities.Count;
            references.SetAt(reference.Index, reference);
            entities.SetAt(reference.Index, entity);
            Added?.Invoke(entities.Buffer[reference.Index]);

            // Connect to parent
            reference.IsChild = true;
            var linkedReference = (LinkedReference)referenced.Reference;
            while (linkedReference.Child != null)
            {
                linkedReference = linkedReference.Child;
            }
            linkedReference.Child = reference;
        }

        public void AddReferenced<TRefernces>(TRefernces entity) where TRefernces : TEntity, IEntity<TRefernces>
        {
            if (entity.Reference == null)
                throw new EntitiesException("Entity not added to entity group");

            // Add entity
            var reference = GetReference();
            reference.Index = entities.Count;
            references.SetAt(reference.Index, reference);
            entities.SetAt(reference.Index, entity);
            Added?.Invoke(entities.Buffer[reference.Index]);

            // Connect to parent
            reference.IsChild = true;
            var linkedReference = (LinkedReference)entity.Reference;
            while (linkedReference.Child != null)
            {
                linkedReference = linkedReference.Child;
            }
            linkedReference.Child = reference;
        }

        public void Remove<T>(T entity) where T : TEntity, IEntity<TEntity>
        {
            if (entity.Reference == null)
                throw new EntitiesException("Entity not added to entity group");
            var classReference = (ClassReference)entity.Reference;
            if (classReference.IsChild)
                throw new EntitiesException("Can't remove referenced entity, remove root entity");
            Remove(classReference);
            entity.Reference = null;
        }

        private void Remove(ClassReference classReference)
        {
            var entity = entities.Buffer[classReference.Index];
            if (references.UnorderedRemoveAt(classReference.Index))
            {
                references[classReference.Index].Index = classReference.Index;
            }
            entities.UnorderedRemoveAt(classReference.Index);
            classReference.Child?.ClassEntityCollection.RemoveBlind(classReference.Child);
            classReference.Clear();
            referencePool.Add(classReference);
            Removed?.Invoke(entity);
        }

        public void RemoveBlind(object reference)
        {
            Remove((ClassReference)reference);
        }

        private ClassReference GetReference()
        {
            ClassReference reference = null;
            if (referencePool.Count > 0)
            {
                reference = referencePool.Buffer[referencePool.Count - 1];
                referencePool.Buffer[referencePool.Count - 1] = null;
                --referencePool.Count;
            }
            else
            {
                reference = new ClassReference();
            }
            reference.Collection = this;
            return reference;
        }

        //[DebuggerDisplay("{" + nameof(Index) + "} at {typeof(TEntity).Name}")]
        private sealed class ClassReference : LinkedReference, IEntityReference<TEntity>
        {
            public EntityCollection<TEntity> Collection;
            public int Index;
            public ref TEntity Entity => ref Collection.entities[Index];

            public override void Clear()
            {
                Index = -1;
                IsChild = false;
                Child = null;
            }

            public override IClassEntityCollection ClassEntityCollection => Collection;

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

    internal abstract class LinkedReference
    {
        public LinkedReference Child;
        public bool IsChild;
        public abstract void Clear();
        public abstract IClassEntityCollection ClassEntityCollection { get; }
    }
}