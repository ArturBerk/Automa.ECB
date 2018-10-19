using System;
using System.Collections.Generic;
using Automa.Common;

namespace Automa.EntityComponents
{
    public struct ComponentType
    {
        public bool Equals(ComponentType other)
        {
            return TypeId == other.TypeId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ComponentType && Equals((ComponentType) obj);
        }

        public override int GetHashCode()
        {
            return TypeId.GetHashCode();
        }

        public ushort TypeId;
        public int TypeIndex;

        private ComponentType(ushort typeId)
        {
            TypeId = typeId;
            TypeIndex = TypeId;
        }

        public static ComponentType Create<T>()
        {
            return new ComponentType(ComponentTypeManager.GetTypeIndex<T>());
        }

        public static ComponentType Create(Type type)
        {
            return new ComponentType(ComponentTypeManager.GetTypeIndex(type));
        }

        public static implicit operator Type(ComponentType type)
        {
            return ComponentTypeManager.GetTypeFromIndex(type.TypeId);
        }

        public static bool operator ==(ComponentType c1, ComponentType c2)
        {
            return c1.TypeId == c2.TypeId;
        }

        public static bool operator !=(ComponentType c1, ComponentType c2)
        {
            return c1.TypeId != c2.TypeId;
        }

        public override string ToString()
        {
            return $"{TypeId} ({ComponentTypeManager.GetTypeFromIndex(TypeId).Name})";
        }
    }

    internal static class ComponentTypeManager
    {
        private static readonly Dictionary<Type, ushort> indicesByTypes = new Dictionary<Type, ushort>();
        private static ArrayList<Type> types = new ArrayList<Type>(4);

        static ComponentTypeManager()
        {
            RegisterType(TypeOf<Entity>.Type);
        }

        public static int TypeCount => types.Count;

        public static ushort GetTypeIndex<T>()
        {
            var result = StaticComponentTypeIndex<T>.typeIndex;
            if (result != 0) return result;
            result = GetTypeIndex(typeof(T));
            StaticComponentTypeIndex<T>.typeIndex = result;
            return result;
        }

        public static ushort GetTypeIndex(Type type)
        {
            if (indicesByTypes.TryGetValue(type, out var p))
            {
                return p;
            }
            return RegisterType(type);
        }

        private static ushort RegisterType(Type type)
        {
            var newId = (ushort) indicesByTypes.Count;
            indicesByTypes.Add(type, newId);
            types.SetAt(newId, type);
            return newId;
        }

        public static Type GetTypeFromIndex(ushort typeTypeId)
        {
            if (types.Count > typeTypeId)
            {
                return types[typeTypeId];
            }
            throw new ArgumentException($"Type with id={typeTypeId} not registred");
        }

        public static void Normalize(ComponentType[] types)
        {
            if (types == null || types.Length == 0) throw new EntitiesException("Entity type must contain al least one component type");
            Array.Sort(types, (type1, type2) =>
            {
                var result = type1.TypeIndex - type2.TypeIndex;
                if (result == 0) throw new EntitiesException("Component types in entity types must be different");
                return result;
            });
        }
    }

    internal static class StaticComponentTypeIndex<T>
    {
        public static ushort typeIndex;
    }
}