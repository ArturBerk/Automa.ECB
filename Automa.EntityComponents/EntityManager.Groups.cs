using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Automa.EntityComponents.Internal;

namespace Automa.EntityComponents
{
    public partial class EntityManager
    {
        public void BindGroupsAndEnumerators(object groupHolder)
        {
            foreach (var fieldInfo in groupHolder.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var fieldType = fieldInfo.FieldType;
                if (typeof(IGroup).IsAssignableFrom(fieldType))
                {
                    var value = fieldInfo.GetValue(groupHolder);
                    if (value == null)
                    {
                        value = Activator.CreateInstance(fieldType);
                        fieldInfo.SetValue(groupHolder, value);
                    }
                    BindGroup((IGroup) value);
                }
                else if (typeof(EntityIterator).IsAssignableFrom(fieldType))
                {
                    var value = fieldInfo.GetValue(groupHolder);
                    if (value == null)
                    {
                        value = Activator.CreateInstance(fieldType);
                        fieldInfo.SetValue(groupHolder, value);
                    }
                    BindEnumerator((EntityIterator) value);
                }
            }
        }

        public void UnbindGroupAndEnumerators(object groupHolder)
        {
            foreach (var fieldInfo in groupHolder.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var fieldType = fieldInfo.FieldType;
                if (typeof(IGroup).IsAssignableFrom(fieldType))
                {
                    var value = fieldInfo.GetValue(groupHolder);
                    if (value != null)
                    {
                        UnbindGroup((IGroup) value);
                    }
                }
                else if (typeof(EntityIterator).IsAssignableFrom(fieldType))
                {
                    var value = fieldInfo.GetValue(groupHolder);
                    if (value == null)
                    {
                        UnbindEnumerator((EntityIterator) value);
                    }
                }
            }
        }

        public void BindGroup(IGroup group)
        {
            var includedTypesTmp = new List<ComponentType>();
            var excludedTypesTmp = new List<ComponentType>();
            foreach (var fieldInfo in group.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var fieldType = fieldInfo.FieldType;
                if (fieldType.IsGenericType)
                {
                    var genericType = fieldType.GetGenericTypeDefinition();
                    if (genericType == typeof(IArray<>))
                    {
                        var componentType = ComponentType.Create(fieldType.GetGenericArguments()[0]);
                        includedTypesTmp.Add(componentType);
                    }
                }
            }
            foreach (var excludeComponentAttribute in GetType().GetCustomAttributes<ExcludeComponentAttribute>())
            {
                var componentType = ComponentType.Create(excludeComponentAttribute.ComponentType);
                excludedTypesTmp.Add(componentType);
            }

            var internalGroup = GetGroup(includedTypesTmp.ToArray(), excludedTypesTmp.ToArray());
            BindGroup(group, internalGroup);
        }

        public void UnbindGroup(IGroup group)
        {
            var includedTypesTmp = new List<ComponentType>();
            var excludedTypesTmp = new List<ComponentType>();
            foreach (var fieldInfo in group.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var fieldType = fieldInfo.FieldType;
                if (fieldType.IsGenericType)
                {
                    var genericType = fieldType.GetGenericTypeDefinition();
                    if (genericType == typeof(IArray<>))
                    {
                        var componentType = ComponentType.Create(fieldType.GetGenericArguments()[0]);
                        includedTypesTmp.Add(componentType);
                    }
                }
            }
            foreach (var excludeComponentAttribute in GetType().GetCustomAttributes<ExcludeComponentAttribute>())
            {
                var componentType = ComponentType.Create(excludeComponentAttribute.ComponentType);
                excludedTypesTmp.Add(componentType);
            }

            var internalGroup = GetGroup(includedTypesTmp.ToArray(), excludedTypesTmp.ToArray());
            UnbindGroup(group, internalGroup);
        }

        private void UnbindGroup(object group, Group internalGroup)
        {
            foreach (var fieldInfo in group.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                var fieldType = fieldInfo.FieldType;
                if (fieldType.IsGenericType)
                {
                    var genericType = fieldType.GetGenericTypeDefinition();
                    if (genericType == typeof(IArray<>))
                    {
                        fieldInfo.SetValue(group, null);
                    }
                }
            }
            if (group is IEntityAddedListener addedListener)
            {
                internalGroup.Remove(addedListener);
            }
            if (group is IEntityRemovingListener removingListener)
            {
                internalGroup.Remove(removingListener);
            }
        }

        private void BindGroup(object group, Group internalGroup)
        {
            foreach (var fieldInfo in group.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                var fieldType = fieldInfo.FieldType;
                if (fieldType.IsGenericType)
                {
                    var genericType = fieldType.GetGenericTypeDefinition();
                    if (genericType == typeof(IArray<>))
                    {
                        var componentType = ComponentType.Create(fieldType.GetGenericArguments()[0]);
                        var value = internalGroup.Arrays.First(
                            iterator => iterator.TypeIndex == componentType.TypeIndex);
                        fieldInfo.SetValue(group, value);
                    }
                }
            }
            if (group is IEntityAddedListener addedListener)
            {
                internalGroup.Add(addedListener);
            }
            if (group is IEntityRemovingListener removingListener)
            {
                internalGroup.Add(removingListener);
            }
        }

        internal Group GetGroup(ComponentType[] included, ComponentType[] excluded)
        {
            ComponentTypeManager.Normalize(included);
            if (excluded.Length > 0)
            {
                ComponentTypeManager.Normalize(excluded);
            }
            foreach (var group in groups)
            {
                if (group.IncludeTypes.Length != included.Length) goto nextGroup;
                if (group.ExcludeTypes.Length != excluded.Length) goto nextGroup;
                for (var i = 0; i < included.Length; i++)
                {
                    if (group.IncludeTypes[i].TypeIndex != included[i].TypeIndex) goto nextGroup;
                }
                for (var i = 0; i < excluded.Length; i++)
                {
                    if (group.ExcludeTypes[i].TypeIndex != excluded[i].TypeIndex) goto nextGroup;
                }
                return group;
                nextGroup:
                ;
            }
            var newGroup = new Group
            {
                IncludeTypes = included,
                ExcludeTypes = excluded
            };
            groups.Add(newGroup);
            newGroup.Arrays = new IComponentArray[included.Length];
            for (var i = 0; i < included.Length; i++)
            {
                newGroup.Arrays[i] =
                    (IComponentArray) Activator.CreateInstance(
                        typeof(ComponentArray<>).MakeGenericType(included[i]));
            }
            newGroup.Update(chunks);
            return newGroup;
        }

        private void UpdateGroups()
        {
            for (var i = 0; i < groups.Count; i++)
            {
                groups[i].Update(chunks);
            }
        }
    }
}