using System;
using System.Globalization;
using System.Reflection;
using Automa.Common;
using Automa.Entities.Internal;

namespace Automa.Entities
{
    public class EntityGroup : IEntityGroup
    {
        private ArrayList<object> entityLists = new ArrayList<object>(4);

        public IEntityCollection Entities(Type type)
        {
            IEntityCollection entityCollection;
            var entityType = EntityTypeManager.GetTypeIndex(type);
            if (entityLists.Count <= entityType)
            {
                entityCollection = (IEntityCollection)Activator.CreateInstance(typeof(EntityCollection<>).MakeGenericType(type));
                entityLists.SetAt(entityType, entityCollection);
            }
            else
            {
                entityCollection = (IEntityCollection)entityLists[entityType];
            }
            return entityCollection;
        }

        public IEntityCollection<T> Entities<T>() where T : class
        {
            EntityCollection<T> entityCollection;
            var entityType = EntityTypeManager.GetTypeIndex<T>();
            if (entityLists.Count <= entityType)
            {
                entityCollection = new EntityCollection<T>();
                entityLists.SetAt(entityType, entityCollection);
            }
            else
            {
                entityCollection = (EntityCollection<T>) entityLists[entityType];
            }
            return entityCollection;
        }

        public IEntityDataCollection<T> EntityDatas<T>() where T : struct
        {
            EntityDataCollection<T> entityCollection;
            var entityType = EntityTypeManager.GetTypeIndex<T>();
            if (entityLists.Count <= entityType)
            {
                entityCollection = new EntityDataCollection<T>();
                entityLists.SetAt(entityType, entityCollection);
            }
            else
            {
                entityCollection = (EntityDataCollection<T>) entityLists[entityType];
            }
            return entityCollection;
        }
    }
}