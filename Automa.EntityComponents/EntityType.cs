using System.Collections.Generic;
using Automa.EntityComponents.Internal;

namespace Automa.EntityComponents
{
    public class EntityType
    {
        public readonly uint TypeId;
        public readonly int TypeIndex;

        public readonly ComponentType[] ComponentTypes;

        internal EntityType(ComponentType[] componentTypes, uint typeId, int typeIndex)
        {
            ComponentTypes = componentTypes;
            TypeId = typeId;
            TypeIndex = typeIndex;
        }
    }

    public static class EntityTypeManager
    {
        private static readonly Dictionary<uint, EntityType> typesById = new Dictionary<uint, EntityType>();

        public static EntityType FromComponentTypes(params ComponentType[] types)
        {
            ComponentTypeManager.Normalize(types);
            var typeId = HashUtility.Fletcher32(types, types.Length);
            if (!typesById.TryGetValue(typeId, out var type))
            {
                type = new EntityType(types, typeId, typesById.Count);
                typesById.Add(typeId, type);
            }
            return type;
        }
    }
}
