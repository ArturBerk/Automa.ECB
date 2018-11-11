namespace Automa.Behaviours.Async
{
    public class Dependency<T>
    {
        public DependencyMode Type { get; }

        private Dependency(DependencyMode type)
        {
            Type = type;
        }

        public static Dependency<T> ReadOnly => new Dependency<T>(DependencyMode.ReadOnly);

        public static Dependency<T> Modify => new Dependency<T>(DependencyMode.Modify);
    }
}