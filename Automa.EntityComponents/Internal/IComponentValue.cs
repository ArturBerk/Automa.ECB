namespace Automa.EntityComponents.Internal
{
    public interface IComponentValue<T>
    {
        ref T Value { get; }
    }

    internal abstract class ComponentValue
    {
        internal EntityEnumerator enumerator;

        public abstract void SetArray(IComponentArray data);
    }

    internal class ComponentValue<T> : ComponentValue, IComponentValue<T>
    {
        internal ComponentArray<T> array;

        public ref T Value => ref array.Get(enumerator);

        public override void SetArray(IComponentArray data)
        {
            array = (ComponentArray<T>) data;
        }
    }
}