using System;
using System.Collections.Generic;
using Automa.Common;
using Automa.Entities.Internal;

namespace Automa.Entities
{
    public class EntityGroup : IEntityGroup
    {
        private Dictionary<Type, IEntityCollection> entityCollections = new Dictionary<Type, IEntityCollection>(20);
        private Dictionary<Type, IEntityDataCollection> entityDataCollections = new Dictionary<Type, IEntityDataCollection>(20);
        private Dictionary<Type, IEntityDataCollection> dataCollections = new Dictionary<Type, IEntityDataCollection>(20);

        public IEnumerable<Type> RegisteredEntityTypes => entityCollections.Keys;

        public IEnumerable<Type> RegisteredEntityDataTypes => entityDataCollections.Keys;

        public IEnumerable<Type> RegisteredDataTypes => dataCollections.Keys;

        public IEntityCollection Entities(Type type)
        {
            if (!entityCollections.TryGetValue(type, out var entityCollection))
            {
                entityCollection =
                    (IEntityCollection) Activator.CreateInstance(typeof(EntityCollection<>).MakeGenericType(type));
                entityCollections.Add(type, entityCollection);
            }
            return entityCollection;
        }

        public IEntityCollection<T> Entities<T>() where T : class
        {
            if (!entityCollections.TryGetValue(TypeOf<T>.Type, out var entityCollection))
            {
                entityCollection = new EntityCollection<T>();
                entityCollections.Add(TypeOf<T>.Type, entityCollection);
            }
            return (IEntityCollection<T>) entityCollection;
        }

        public IEntityDataCollection<T> EntityDatas<T>() where T : struct
        {
            if (!entityDataCollections.TryGetValue(TypeOf<T>.Type, out var entityCollection))
            {
                entityCollection = new EntityDataCollection<T>();
                entityDataCollections.Add(TypeOf<T>.Type, entityCollection);
            }
            return (IEntityDataCollection<T>)entityCollection;
        }

        public IDataCollection<T> Datas<T>() where T : struct
        {
            if (!dataCollections.TryGetValue(TypeOf<T>.Type, out var entityCollection))
            {
                entityCollection = new DataCollection<T>();
                dataCollections.Add(TypeOf<T>.Type, entityCollection);
            }
            return (IDataCollection<T>)entityCollection;
        }

        public void Clear(bool clearEntityTypes = false)
        {
            foreach (var entityList in entityCollections.Values)
            {
                entityList.Clear();
            }
            foreach (var entityList in entityDataCollections.Values)
            {
                entityList.Clear();
            }
            foreach (var entityList in dataCollections.Values)
            {
                entityList.Clear();
            }

            if (clearEntityTypes)
            {
                entityCollections.Clear();
                entityDataCollections.Clear();
                dataCollections.Clear();
            }
        }
    }
}