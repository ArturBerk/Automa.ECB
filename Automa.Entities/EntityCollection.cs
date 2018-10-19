using System;
using Automa.Common;

namespace Automa.Entities
{
    public class EntityCollection
    {
    }

    public class EntityCollection<TEntity> : EntityCollection
    {
        public delegate void EntityAddedHandler(ref TEntity entity);

        public delegate void EntityRemovedHandler(ref TEntity entity);

        private static ArrayList<StructReference> structReferencePool;
        private static ArrayList<ClassReference> classReferencePool = new ArrayList<ClassReference>();
        private ArrayList<int> availableLocations;
        private ArrayList<TEntity> entities = new ArrayList<TEntity>(4);
        private ArrayList<int> entityLocations;

        public int Count => entities.Count;

        public ref TEntity this[int index] => ref entities.Buffer[index];

        public event EntityAddedHandler EntityAdded;
        public event EntityRemovedHandler EntityRemoved;

        public void AddEntityData<T>(T entity) where T : struct, TEntity, IEntity<TEntity>
        {
            if (entity.Reference != null)
                throw new ApplicationException("Entity already added to entity group");
            var reference = GetStructReference();
            if (availableLocations.Count > 0)
            {
                reference.locationIndex = availableLocations[availableLocations.Count - 1];
                --availableLocations.Count;
            }
            else
            {
                reference.locationIndex = entityLocations.Count;
            }
            var entityIndex = entities.Count;
            entityLocations[reference.locationIndex] = entityIndex;
            entities.ExpandCount(entityIndex + 1);
            entities[entityIndex] = entity;
            EntityAdded?.Invoke(ref entities.Buffer[entityIndex]);
        }

        public void AddEntity<T>(T entity) where T : class, TEntity, IEntity<TEntity>
        {
            if (entity.Reference != null)
                throw new ApplicationException("Entity already added to entity group");
            var reference = GetClassReference();
            if (availableLocations.Count > 0)
            {
                reference.locationIndex = availableLocations[availableLocations.Count - 1];
                --availableLocations.Count;
            }
            else
            {
                reference.locationIndex = entityLocations.Count;
            }
            reference.entity = entity;
            var entityIndex = entities.Count;
            entityLocations[reference.locationIndex] = entityIndex;
            entities.ExpandCount(entityIndex + 1);
            entities[entityIndex] = entity;
            EntityAdded?.Invoke(ref entities.Buffer[entityIndex]);
        }

        public void AddEntityReferenced<T>(IEntity<T> entity) where T : class, IEntity
        {
            if (entity.Reference == null)
                throw new ApplicationException("Entity not added to entity group");

            // Add entity
            var reference = GetClassReference();
            if (availableLocations.Count > 0)
            {
                reference.locationIndex = availableLocations[availableLocations.Count - 1];
                --availableLocations.Count;
            }
            else
            {
                reference.locationIndex = entityLocations.Count;
            }
            reference.entity = (TEntity) entity;
            var entityIndex = entities.Count;
            entityLocations[reference.locationIndex] = entityIndex;
            entities.ExpandCount(entityIndex + 1);
            entities[entityIndex] = (TEntity) entity;
            EntityAdded?.Invoke(ref entities.Buffer[entityIndex]);

            // Connect to parent
            reference.IsChild = true;
            var linkedReference = (LinkedReference) entity.Reference;
            while (linkedReference.Child != null)
            {
                linkedReference = linkedReference.Child;
            }
            linkedReference.Child = reference;
        }

        public void RemoveEntity<T>(T entity) where T : class, TEntity, IEntity<TEntity>
        {
            if (entity.Reference == null)
                throw new ApplicationException("Entity not added to entity group");
            var classReference = (ClassReference) entity.Reference;
            if (classReference.IsChild)
                throw new ApplicationException("Can't remove referenced entity, remove root entity");
            RemoveEntity(classReference);
        }

        private void RemoveEntity(ClassReference classReference)
        {
            var entityLocation = entityLocations[classReference.locationIndex];
            EntityRemoved?.Invoke(ref entities.Buffer[entityLocation]);
            entities.UnorderedRemoveAt(entityLocation);
            RemoveChildEntity(classReference);
            classReference.Clear();
            classReferencePool.Add(classReference);
        }

        private void RemoveChildEntity(ClassReference parent)
        {
            if (parent.Child == null) return;
            var classReference = (ClassReference) parent.Child;
            classReference.collection.RemoveEntity(classReference);
        }

        public void RemoveEntityData<T>(T entity) where T : struct, TEntity, IEntity<TEntity>
        {
            if (entity.Reference == null)
                throw new ApplicationException("Entity not added to entity group");
            var structReference = (StructReference) entity.Reference;
            var entityLocation = entityLocations[structReference.locationIndex];
            entities.UnorderedRemoveAt(entityLocation);
            structReference.locationIndex = -1;
            structReferencePool.Add(structReference);
        }

        private StructReference GetStructReference()
        {
            StructReference reference = null;
            if (structReferencePool.Count > 0)
            {
                reference = structReferencePool.Buffer[structReferencePool.Count - 1];
                structReferencePool.Buffer[structReferencePool.Count - 1] = null;
                --structReferencePool.Count;
            }
            else
            {
                reference = new StructReference
                {
                    collection = this
                };
            }
            return reference;
        }

        private ClassReference GetClassReference()
        {
            ClassReference reference = null;
            if (classReferencePool.Count > 0)
            {
                reference = classReferencePool.Buffer[structReferencePool.Count - 1];
                structReferencePool.Buffer[structReferencePool.Count - 1] = null;
                --structReferencePool.Count;
            }
            else
            {
                reference = new ClassReference
                {
                    collection = this
                };
            }
            return reference;
        }

        private class StructReference : IEntityReference<TEntity>
        {
            public EntityCollection<TEntity> collection;
            public int locationIndex;
            public ref TEntity Entity => ref collection.entities[collection.entityLocations[locationIndex]];
        }

        private class ClassReference : LinkedReference, IEntityReference<TEntity>
        {
            public EntityCollection<TEntity> collection;
            public TEntity entity;
            public int locationIndex;
            public ref TEntity Entity => ref entity;

            public override void Clear()
            {
                locationIndex = -1;
                entity = default(TEntity);
                IsChild = false;
                Child = null;
            }
        }

        private abstract class LinkedReference
        {
            public LinkedReference Child;
            public bool IsChild;
            public abstract void Clear();
        }
    }
}