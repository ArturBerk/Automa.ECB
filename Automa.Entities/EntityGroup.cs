using System;
using System.Collections.Generic;
using Automa.Common;
using Automa.Entities.Internal;

namespace Automa.Entities
{
    public class EntityGroup : IEntityGroup
    {
        private ArrayList<IBaseEntityCollection> entityCollections = new ArrayList<IBaseEntityCollection>(4);
        private ArrayList<IBaseEntityCollection> dataCollections = new ArrayList<IBaseEntityCollection>(4);

        public IEnumerable<Type> RegisteredEntityTypes
        {
            get
            {
                for (int i = 0; i < entityCollections.Count; i++)
                {
                    if (entityCollections[i] is IEntityCollection collection)
                        yield return collection.Type;
                }
            }
        }

        public IEnumerable<Type> RegisteredEntityDataTypes
        {
            get
            {
                for (int i = 0; i < entityCollections.Count; i++)
                {
                    if (entityCollections[i] is IEntityDataCollection collection)
                        yield return collection.Type;
                }
            }
        }

        public IEnumerable<Type> RegisteredDataTypes
        {
            get
            {
                for (int i = 0; i < entityCollections.Count; i++)
                {
                    var collection = dataCollections[i];
                    if (collection != null) yield return collection.Type;
                }
            }
        }

        public IEntityCollection Entities(Type type)
        {
            IEntityCollection entityCollection;
            var entityType = EntityTypeManager.GetTypeIndex(type);
            if (entityCollections.Count <= entityType || entityCollections[entityType] == null)
            {
                entityCollection = (IEntityCollection)Activator.CreateInstance(typeof(EntityCollection<>).MakeGenericType(type));
                entityCollections.SetAt(entityType, entityCollection);
            }
            else
            {
                entityCollection = (IEntityCollection)entityCollections[entityType];
            }
            return entityCollection;
        }

        public IEntityCollection<T> Entities<T>() where T : class
        {
            EntityCollection<T> entityCollection;
            var entityType = EntityTypeManager.GetTypeIndex<T>();
            if (entityCollections.Count <= entityType || entityCollections[entityType] == null)
            {
                entityCollection = new EntityCollection<T>();
                entityCollections.SetAt(entityType, entityCollection);
            }
            else
            {
                entityCollection = (EntityCollection<T>) entityCollections[entityType];
            }
            return entityCollection;
        }

        public IEntityDataCollection<T> EntityDatas<T>() where T : struct
        {
            EntityDataCollection<T> entityCollection;
            var entityType = EntityTypeManager.GetTypeIndex<T>();
            if (entityCollections.Count <= entityType || entityCollections[entityType] == null)
            {
                entityCollection = new EntityDataCollection<T>();
                entityCollections.SetAt(entityType, entityCollection);
            }
            else
            {
                entityCollection = (EntityDataCollection<T>) entityCollections[entityType];
            }
            return entityCollection;
        }

        public IDataCollection<T> Datas<T>() where T : struct
        {
            DataCollection<T> entityCollection;
            var entityType = EntityTypeManager.GetTypeIndex<T>();
            if (entityCollections.Count <= entityType || entityCollections[entityType] == null)
            {
                entityCollection = new DataCollection<T>();
                entityCollections.SetAt(entityType, entityCollection);
                dataCollections.Add(entityCollection);
            }
            else
            {
                entityCollection = (DataCollection<T>)entityCollections[entityType];
            }
            return entityCollection;
        }

        public void Clear()
        {
            foreach (var entityList in entityCollections)
            {
                entityList?.Clear();
            }
            foreach (var baseEntityCollection in dataCollections)
            {
                baseEntityCollection?.Clear();
            }
        }
    }
}