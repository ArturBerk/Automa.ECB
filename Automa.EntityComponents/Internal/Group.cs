using System.Linq;
using Automa.Common;

namespace Automa.EntityComponents.Internal
{
    internal class Group
    {
        public IComponentArray[] Arrays;
        public ArrayList<EntityTypeChunk> Chunks = new ArrayList<EntityTypeChunk>(4);
        public ComponentType[] ExcludeTypes;
        public ComponentType[] IncludeTypes;
        private ArrayList<IEntityAddedListener> addedListeners = new ArrayList<IEntityAddedListener>(1);
        private ArrayList<IEntityRemovingListener> removingListeners = new ArrayList<IEntityRemovingListener>(1);

        public void Add(IEntityAddedListener addedListener)
        {
            addedListeners.Add(addedListener);
            if (addedListeners.Count == 1)
            {
                foreach (var entityTypeChunk in Chunks)
                {
                    entityTypeChunk.EntityAdded += OnEntityAdded;
                }
            }
        }

        public void Remove(IEntityAddedListener addedListener)
        {
            addedListeners.Remove(addedListener);
            if (addedListeners.Count == 0)
            {
                foreach (var entityTypeChunk in Chunks)
                {
                    entityTypeChunk.EntityAdded -= OnEntityAdded;
                }
            }
        }

        public void Add(IEntityRemovingListener removingListener)
        {
            removingListeners.Add(removingListener);
            if (addedListeners.Count == 1)
            {
                foreach (var entityTypeChunk in Chunks)
                {
                    entityTypeChunk.EntityRemoving += OnEntityRemoving;
                }
            }
        }

        public void Remove(IEntityRemovingListener removingListener)
        {
            removingListeners.Remove(removingListener);
            if (addedListeners.Count == 0)
            {
                foreach (var entityTypeChunk in Chunks)
                {
                    entityTypeChunk.EntityRemoving -= OnEntityRemoving;
                }
            }
        }

        public EntityIterator GetEnumerator()
        {
            var entityEnumerator = new EntityIterator();
            entityEnumerator.ApplyGroup(this);
            return entityEnumerator;
        }
        
        public void Update(EntityTypeChunk[] chunks)
        {
            if (addedListeners.Count > 0)
            {
                foreach (var entityTypeChunk in Chunks)
                {
                    entityTypeChunk.EntityAdded -= OnEntityAdded;
                }
            }
            if (removingListeners.Count > 0)
            {
                foreach (var entityTypeChunk in Chunks)
                {
                    entityTypeChunk.EntityRemoving -= OnEntityRemoving;
                }
            }
            Chunks.Clear();
            for (var i = 0; i < chunks.Length; i++)
            {
                var chunk = chunks[i];
                if (chunk != null && IsSuitable(chunk))
                {
                    Chunks.Add(chunk);
                }
            }
            foreach (var componentIterator in Arrays)
            {
                componentIterator.UpdateChunks(ref Chunks);
            }
            if (addedListeners.Count > 0)
            {
                foreach (var entityTypeChunk in Chunks)
                {
                    entityTypeChunk.EntityAdded += OnEntityAdded;
                }
            }
            if (removingListeners.Count > 0)
            {
                foreach (var entityTypeChunk in Chunks)
                {
                    entityTypeChunk.EntityRemoving += OnEntityRemoving;
                }
            }
        }

        private void OnEntityAdded(EntityTypeChunk movedFrom, Entity entity)
        {
            if (movedFrom != null)
            {
                for (int i = 0; i < Chunks.Count; i++)
                {
                    if (ReferenceEquals(Chunks.Buffer[i], movedFrom)) return;
                }
            }
            for (int i = 0; i < addedListeners.Count; i++)
            {
                addedListeners[i].EntityAdded(entity);
            }
        }

        private void OnEntityRemoving(EntityTypeChunk movingTo, Entity entity)
        {
            if (movingTo != null)
            {
                for (int i = 0; i < Chunks.Count; i++)
                {
                    if (ReferenceEquals(Chunks.Buffer[i], movingTo)) return;
                }
            }
            for (int i = 0; i < addedListeners.Count; i++)
            {
                removingListeners[i].EntityRemoving(entity);
            }
        }

        private bool IsSuitable(EntityTypeChunk chunk)
        {
            for (var i = 0; i < IncludeTypes.Length; i++)
            {
                var t = IncludeTypes[i].TypeId;
                if (t == 0) continue;
                if (chunk.EntityType.ComponentTypes.All(type => type.TypeId != t)) return false;
            }
            for (var i = 0; i < ExcludeTypes.Length; i++)
            {
                var t = ExcludeTypes[i].TypeId;
                if (t == 0) continue;
                if (chunk.EntityType.ComponentTypes.Any(type => type.TypeId == t)) return false;
            }
            return true;
        }
    }
}