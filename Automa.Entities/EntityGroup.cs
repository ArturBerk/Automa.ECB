using Automa.Common;

namespace Automa.Entities
{
    public interface IEntityGroup
    {
        EntityCollection<T> Entities<T>();
    }

    public class EntityGroup : IEntityGroup
    {
        private ArrayList<EntityCollection> entityLists = new ArrayList<EntityCollection>(4);

        public EntityCollection<T> Entities<T>()
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
    }
}