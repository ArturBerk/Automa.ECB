using System;
using System.Data;

namespace Automa.Entities
{
    public interface IEntityGroup
    {
        IEntityCollection Entities(Type type);
        IEntityCollection<T> Entities<T>() where T : class;
        IEntityDataCollection<T> EntityDatas<T>() where T : struct;
        void Clear();
    }
}