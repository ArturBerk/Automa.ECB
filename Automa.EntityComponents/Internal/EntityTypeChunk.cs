using System;
using Automa.Common;

namespace Automa.EntityComponents.Internal
{
    internal class EntityTypeChunk
    {
        public readonly ComponentData[] ComponentDatas;
        public readonly int ComponentTypeCount;
        public readonly ComponentData<Entity> EntityData;
        public readonly EntityManager entityManager;
        public readonly EntityType EntityType;
        public int EntityCount;
        public event Action<EntityTypeChunk, Entity> EntityAdded;
        public event Action<EntityTypeChunk, Entity> EntityRemoving;
        private ArrayList<(EntityTypeChunk, Entity)> addedEntities = new ArrayList<(EntityTypeChunk, Entity)>(4);

        public EntityTypeChunk(EntityManager entityManager, EntityType entityType)
        {
            this.entityManager = entityManager;
            EntityType = entityType;
            ComponentDatas = new ComponentData[ComponentTypeManager.TypeCount];
            ComponentTypeCount = entityType.ComponentTypes.Length;
            for (var i = 0; i < ComponentTypeCount; i++)
            {
                var componentType = entityType.ComponentTypes[i];
                var componentArrayType = typeof(ComponentData<>).MakeGenericType(componentType);
                ComponentDatas[componentType.TypeIndex] = (ComponentData) Activator.CreateInstance(componentArrayType);
            }
            EntityData = new ComponentData<Entity>();
            ComponentDatas[0] = EntityData;
        }

        public Entity AddEntity(Entity entity, EntityTypeChunk movedFrom)
        {
            entity.Chunk = this;
            for (var i = 0; i < ComponentTypeCount; i++)
            {
                var ctype = EntityType.ComponentTypes[i];
                ComponentDatas[ctype.TypeIndex].Add();
            }
            EntityData.Add();
            EntityData.Set(EntityCount, ref entity);
            entity.IndexInChunk = EntityCount++;
            if (EntityAdded != null)
            {
                addedEntities.Add((movedFrom, entity));
            }
            EntityAdded?.Invoke(movedFrom, entity);
            return entity;
        }

        public void RemoveEntity(Entity entity, EntityTypeChunk movingTo)
        {
            EntityRemoving?.Invoke(movingTo, entity);
            for (var i = 0; i < ComponentTypeCount; i++)
            {
                var ctype = EntityType.ComponentTypes[i];
                ComponentDatas[ctype.TypeIndex].Remove(entity.IndexInChunk);
            }
            if (EntityData.Remove(entity.IndexInChunk))
            {
                EntityData[entity.IndexInChunk].IndexInChunk = entity.IndexInChunk;
            }
            entity.Chunk = null;
            entity.IndexInChunk = -1;
            --EntityCount;
        }

        public void Dispatch()
        {
            if (EntityAdded != null && addedEntities.Count > 0)
            {
                for (var index = 0; index < addedEntities.Buffer.Length; index++)
                {
                    var addedEntity = addedEntities.Buffer[index];
                    EntityAdded(addedEntity.Item1, addedEntity.Item2);
                }
                addedEntities.Clear();
            }
        }
    }
}