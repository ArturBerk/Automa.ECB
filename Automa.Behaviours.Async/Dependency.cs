using System;
using Automa.Common;

namespace Automa.Behaviours.Async
{
    public class Dependency<T> : IDependency
    {
        private Dependency(DependencyMode type)
        {
            Mode = type;
        }

        public static Dependency<T> ReadOnly => new Dependency<T>(DependencyMode.ReadOnly);

        public static Dependency<T> Modify => new Dependency<T>(DependencyMode.Modify);
        public Type Type => TypeOf<T>.Type;
        public DependencyMode Mode { get; }
    }
}