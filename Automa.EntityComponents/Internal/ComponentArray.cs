using System;
using Automa.Common;

namespace Automa.EntityComponents.Internal
{
    internal interface IComponentArray
    {
        int TypeIndex { get; }
        void UpdateChunks(ref ArrayList<EntityTypeChunk> chunks);
    }

    internal class ComponentArray<T> : IArray<T>, IComponentArray
    {
        private readonly int index;
        internal ComponentData<T>[] componentDatas = Array.Empty<ComponentData<T>>();

        public int EvaluateCount
        {
            get
            {
                var result = 0;
                for (int i = 0; i < componentDatas.Length; ++i)
                {
                    result += componentDatas[i].Count;
                }
                return result;
            }
        }

        public ComponentArray()
        {
            index = ComponentType.Create<T>().TypeIndex;
        }

        public ref T Get(EntityIterator f)
        {
            return ref componentDatas[f.CurrentChunkIndex][f.CurrentIndex];
        }

        public ref T this[int index]
        {
            get
            {
                for (var i = 0; i < componentDatas.Length; i++)
                {
                    var r = componentDatas[i];
                    if (r.Count <= index)
                    {
                        index -= r.Count;
                        continue;
                    }
                    return ref r[index];
                }
                throw new ArgumentOutOfRangeException("Index out off range");
            }
        }

        public int TypeIndex => index;

        public void UpdateChunks(ref ArrayList<EntityTypeChunk> chunks)
        {
            if (componentDatas.Length != chunks.Count)
            {
                componentDatas = new ComponentData<T>[chunks.Count];
            }
            for (var i = 0; i < chunks.Count; i++)
            {
                componentDatas[i] = (ComponentData<T>) chunks[i].ComponentDatas[index];
            }
        }
    }
}