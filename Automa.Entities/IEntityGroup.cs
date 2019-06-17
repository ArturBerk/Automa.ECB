using System;
using System.Collections.Generic;
using System.Data;

namespace Automa.Entities
{
    public interface IEntityGroup
    {
        IEnumerable<Type> RegisteredEntityTypes { get; }
        IEnumerable<Type> RegisteredEntityDataTypes { get; }
        IEnumerable<Type> RegisteredDataTypes { get; }

        IEntityCollection Entities(Type type);
        IEntityCollection<T> Entities<T>() where T : class;
        IEntityDataCollection<T> EntityDatas<T>() where T : struct;
        IDataCollection<T> Datas<T>() where T : struct;
        void Clear();
    }
}