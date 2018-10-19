namespace Automa.EntityComponents.Internal
{
    public class EntityEnumerator
    {
        internal Group group;
        private int currentChunkLength;

        internal int CurrentChunkIndex;
        internal int CurrentIndex;

        internal void ApplyGroup(Group group)
        {
            this.group = group;
            Reset();
        }

        public bool MoveNext()
        {
            if (@group == null) throw new EntitiesException("Entity enumerator not binded");
            if (CurrentChunkIndex < 0) return false;
            ++CurrentIndex;
            while (CurrentIndex >= currentChunkLength)
            {
                ++CurrentChunkIndex;
                if (CurrentChunkIndex >= group.Chunks.Count)
                {
                    CurrentChunkIndex = -1;
                    return false;
                }
                currentChunkLength = group.Chunks.Buffer[CurrentChunkIndex].EntityCount;
                CurrentIndex = 0;
            }
            return true;
        }

        public void Reset()
        {
            currentChunkLength = group.Chunks.Count > 0 ? group.Chunks[0].EntityCount : 0;
            CurrentChunkIndex = group.Chunks.Count > 0 ? 0 : -1;
            CurrentIndex = -1;
        }
    }
}