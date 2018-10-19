namespace Automa.EntityComponents
{
    public interface IArray<T>
    {
        ref T this[int index] { get; }
        int EvaluateCount { get; }
    }
}
