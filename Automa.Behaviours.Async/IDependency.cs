using System;

namespace Automa.Behaviours.Async
{
    public interface IDependency
    {
        Type Type { get; }
        DependencyMode Mode { get; }
    }
}