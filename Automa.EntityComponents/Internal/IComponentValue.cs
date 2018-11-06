using System.Runtime.CompilerServices;

namespace Automa.EntityComponents.Internal
{
    public interface IValue<T>
    {
        ref T Value { get; }
    }

    internal abstract class Value
    {
        public EntityIterator Iterator;
        public abstract void ChangeChunk(EntityTypeChunk chunk);
    }

    internal class Value<T> : Value, IValue<T>
    {
        private readonly int componentTypeIndex;
        private ComponentData<T> data;
        ref T IValue<T>.Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return ref data[Iterator.CurrentIndex]; }
        }

        public Value()
        {
            componentTypeIndex = ComponentTypeManager.GetTypeIndex<T>();
        }

        public override void ChangeChunk(EntityTypeChunk chunk)
        {
            data = (ComponentData<T>) chunk?.ComponentDatas[componentTypeIndex];
        }
    }
}