using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Automa.EntityComponents.Internal;

namespace Automa.EntityComponents
{
    public partial class EntityManager
    {
        public void BindEnumerator(EntityEnumerator entityEnumerator)
        {
            var includedTypesTmp = new List<ComponentType>();
            var excludedTypesTmp = new List<ComponentType>();
            foreach (var fieldInfo in entityEnumerator.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var fieldType = fieldInfo.FieldType;
                if (fieldType.IsGenericType)
                {
                    var genericType = fieldType.GetGenericTypeDefinition();
                    if (genericType == typeof(IComponentValue<>))
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
            BindEnumerator(entityEnumerator, internalGroup);
        }

        public void UnbindEnumerator(EntityEnumerator entityEnumerator)
        {
            UnbindEnumerator(entityEnumerator, entityEnumerator.group);
        }

        private void UnbindEnumerator(EntityEnumerator entityEnumerator, Group group)
        {
            if (group == null) return;
            entityEnumerator.group = null;
            foreach (var fieldInfo in entityEnumerator.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var fieldType = fieldInfo.FieldType;
                if (fieldType.IsGenericType)
                {
                    var genericType = fieldType.GetGenericTypeDefinition();
                    if (genericType == typeof(IComponentValue<>))
                    {
                        fieldInfo.SetValue(entityEnumerator, null);
                    }
                }
            }
            if (entityEnumerator is IEntityAddedListener addedListener)
            {
                group.Remove(addedListener);
            }
            if (entityEnumerator is IEntityRemovingListener removingListener)
            {
                group.Remove(removingListener);
            }
        }

        private void BindEnumerator(EntityEnumerator entityEnumerator, Group internalGroup)
        {
            entityEnumerator.ApplyGroup(internalGroup);
            foreach (var fieldInfo in entityEnumerator.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var fieldType = fieldInfo.FieldType;
                if (fieldType.IsGenericType)
                {
                    var genericType = fieldType.GetGenericTypeDefinition();
                    if (genericType == typeof(IComponentValue<>))
                    {
                        var cType = fieldType.GetGenericArguments()[0];
                        var componentType = ComponentType.Create(cType);
                        var componentValue =
                            (ComponentValue) Activator.CreateInstance(typeof(ComponentValue<>).MakeGenericType(cType));
                        componentValue.enumerator = entityEnumerator;
                        componentValue.SetArray(internalGroup.Arrays.First(iterator =>
                            iterator.TypeIndex == componentType.TypeIndex));
                        fieldInfo.SetValue(entityEnumerator, componentValue);
                    }
                }
            }
            if (entityEnumerator is IEntityAddedListener addedListener)
            {
                internalGroup.Add(addedListener);
            }
            if (entityEnumerator is IEntityRemovingListener removingListener)
            {
                internalGroup.Add(removingListener);
            }
        }
    }
}