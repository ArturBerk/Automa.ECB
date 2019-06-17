using System;

namespace Automa.Entities
{
    public interface IEntityReference : IDisposable
    {
        bool IsValid { get; }
    }

    public interface IEntityReference<TEntity> : IEntityReference
    {
        ref TEntity Entity { get; }
    }
}