namespace Automa.Behaviours
{
    public interface IBehaviour
    {
        bool IsEnabled { get; set; }
        void Apply();
    }
}