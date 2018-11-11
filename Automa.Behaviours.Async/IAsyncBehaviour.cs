using System.Collections.Generic;

namespace Automa.Behaviours.Async
{
    public interface IAsyncBehaviour
    {
        IEnumerable<IDependency> Dependencies { get; }
        void ApplyAsync();
    }
}
