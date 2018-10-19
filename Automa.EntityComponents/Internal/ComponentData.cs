using Automa.Common;

namespace Automa.EntityComponents.Internal
{
    internal abstract class ComponentData
    {
        public int Count;
        public abstract void Add();
        public abstract bool Remove(int index);
    }

    internal sealed class ComponentData<T> : ComponentData
    {
        private ArrayList<T> data = new ArrayList<T>(4);

        public ref T this[int index] => ref data.Buffer[index];

        public override void Add()
        {
            ++Count;
            data.ExpandCount(Count);
        }

        public override bool Remove(int index)
        {
            --Count;
            return data.UnorderedRemoveAt(index);
        }

        public void Set(int index, ref T value)
        {
            data[index] = value;
        }
    }
}