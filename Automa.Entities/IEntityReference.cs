using System;

namespace Automa.Entities
{
    public interface IEntityReference<TEntity> : IDisposable
    {
        ref TEntity Entity { get; }
    }
}