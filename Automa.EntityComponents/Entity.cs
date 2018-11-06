using System.Runtime.CompilerServices;
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

        public void SetType(EntityType newType)
        {
            if (Chunk.EntityType == newType) return;
            Chunk.entityManager.ChangeEntityType(this, newType);
        }

        public bool HasNotNullComponent<T>() where T : class
        {
            var chunkComponentData = Chunk.ComponentDatas[ComponentTypeManager.GetTypeIndex<T>()];
            return ((ComponentData<T>) chunkComponentData)?[IndexInChunk] != null;
        }

        public bool HasComponent<T>()
        {
            return Chunk.ComponentDatas[ComponentType.Create<T>().TypeIndex] != null;
        }

        public void SetComponent<T>(T data)
        {
            var t = (ComponentData<T>)Chunk.ComponentDatas[ComponentTypeManager.GetTypeIndex<T>()];
            if (t == null)
            {
                throw new EntitiesException($"Entity doesn't have component of type \"{typeof(T)}\"");
            }
            t[IndexInChunk] = data;
        }

        public void SetComponent<T>(ref T data)
        {
            var t = (ComponentData<T>)Chunk.ComponentDatas[ComponentTypeManager.GetTypeIndex<T>()];
            if (t == null)
            {
                throw new EntitiesException($"Entity doesn't have component of type \"{typeof(T)}\"");
            }
            t[IndexInChunk] = data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetComponent<T>()
        {
            var t = (ComponentData<T>)Chunk.ComponentDatas[ComponentTypeManager.GetTypeIndex<T>()];
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