using Automa.EntityComponents.Internal;

namespace Automa.EntityComponents
{
    public class Entity
    {
        internal EntityTypeChunk Chunk;
        internal int IndexInChunk;

        public void Remove()
        {
            Chunk.RemoveEntity(this, null);
        }

        public void ChangeType(EntityType newType)
        {
            Chunk.entityManager.ChangeEntityType(this, newType);
        }

        public bool HasNotNullComponent<T>() where T : class
        {
            var chunkComponentData = Chunk.ComponentDatas[ComponentType.Create<T>().TypeIndex];
            return ((ComponentData<T>) chunkComponentData)?[IndexInChunk] != null;
        }

        public bool HasComponent<T>()
        {
            return Chunk.ComponentDatas[ComponentType.Create<T>().TypeIndex] != null;
        }

        public void SetComponent<T>(T data) where T : class
        {
            var t = (ComponentData<T>)Chunk.ComponentDatas[ComponentType.Create<T>().TypeIndex];
            if (t == null)
            {
                throw new EntitiesException($"Entity doesn't have component of type \"{typeof(T)}\"");
            }
            t[IndexInChunk] = data;
        }

        public void SetComponent<T>(ref T data) where T : struct
        {
            var t = (ComponentData<T>)Chunk.ComponentDatas[ComponentType.Create<T>().TypeIndex];
            if (t == null)
            {
                throw new EntitiesException($"Entity doesn't have component of type \"{typeof(T)}\"");
            }
            t[IndexInChunk] = data;
        }

        public ref T GetComponent<T>()
        {
            var t = (ComponentData<T>)Chunk.ComponentDatas[ComponentType.Create<T>().TypeIndex];
            if (t == null)
            {
                throw new EntitiesException($"Entity doesn't have component of type \"{typeof(T)}\"");
            }
            return ref t[IndexInChunk];
        }

        public bool Equals(Entity other)
        {
            return Equals(Chunk, other.Chunk) && IndexInChunk == other.IndexInChunk;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Entity && Equals((Entity) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Chunk != null ? Chunk.GetHashCode() : 0) * 397) ^ IndexInChunk;
            }
        }
    }
}