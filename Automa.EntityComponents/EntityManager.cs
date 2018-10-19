using System;
using Automa.Common;
using Automa.EntityComponents.Internal;

namespace Automa.EntityComponents
{
    public partial class EntityManager
    {
        private EntityTypeChunk[] chunks = Array.Empty<EntityTypeChunk>();
        private ArrayList<Group> groups = new ArrayList<Group>(4);

        public Entity CreateEntity(EntityType entityType)
        {
            return GetChunk(entityType).AddEntity(new Entity(), null);
        }

        public void RemoveEntity(Entity entity)
        {
            entity.Chunk.RemoveEntity(entity, null);
        }

        private EntityTypeChunk GetChunk(EntityType entityType)
        {
            if (entityType.TypeIndex >= chunks.Length)
            {
                ExpandChunks(entityType.TypeIndex + 1);
            }
            var chunk = chunks[entityType.TypeIndex];
            if (chunk == null)
            {
                chunk = new EntityTypeChunk(this, entityType);
                chunks[entityType.TypeIndex] = chunk;
                UpdateGroups();
            }
            return chunk;
        }

        public void ChangeEntityType(Entity entity, EntityType newType)
        {
            var sourceChunk = entity.Chunk;
            var targetChunk = GetChunk(newType);
            sourceChunk.RemoveEntity(entity, targetChunk);
            targetChunk.AddEntity(entity, sourceChunk);
        }

        private void ExpandChunks(int count)
        {
            Array.Resize(ref chunks, count);
        }
    }
}