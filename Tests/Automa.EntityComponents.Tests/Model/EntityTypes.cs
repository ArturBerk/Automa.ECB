namespace Automa.EntityComponents.Model
{
    public static class EntityTypes
    {
        public static EntityType Type1 = EntityTypeManager.FromComponentTypes(
            ComponentType.Create<Component1>(),
            ComponentType.Create<Component2>());

        public static EntityType Type2 = EntityTypeManager.FromComponentTypes(
            ComponentType.Create<Component1>(),
            ComponentType.Create<Component2>(),
            ComponentType.Create<Component3>());

        public static EntityType Type3 = EntityTypeManager.FromComponentTypes(
            ComponentType.Create<Component2>(),
            ComponentType.Create<Component3>());
    }
}
