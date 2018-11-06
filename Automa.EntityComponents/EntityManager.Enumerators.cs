using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Automa.EntityComponents.Internal;

namespace Automa.EntityComponents
{
    public partial class EntityManager
    {
        public void BindEnumerator(EntityIterator entityIterator)
        {
            var includedTypesTmp = new List<ComponentType>();
            var excludedTypesTmp = new List<ComponentType>();
            foreach (var fieldInfo in entityIterator.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var fieldType = fieldInfo.FieldType;
                if (fieldType.IsGenericType)
                {
                    var genericType = fieldType.GetGenericTypeDefinition();
                    if (genericType == typeof(IValue<>))
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
            BindEnumerator(entityIterator, internalGroup);
        }

        public void UnbindEnumerator(EntityIterator entityIterator)
        {
            UnbindEnumerator(entityIterator, entityIterator.group);
        }

        private void UnbindEnumerator(EntityIterator entityIterator, Group group)
        {
            if (group == null) return;
            entityIterator.group = null;
            foreach (var fieldInfo in entityIterator.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var fieldType = fieldInfo.FieldType;
                if (fieldType.IsGenericType)
                {
                    var genericType = fieldType.GetGenericTypeDefinition();
                    if (genericType == typeof(IValue<>))
                    {
                        fieldInfo.SetValue(entityIterator, null);
                    }
                }
            }
            if (entityIterator is IEntityAddedListener addedListener)
            {
                group.Remove(addedListener);
            }
            if (entityIterator is IEntityRemovingListener removingListener)
            {
                group.Remove(removingListener);
            }
        }

        private void BindEnumerator(EntityIterator entityIterator, Group internalGroup)
        {
            entityIterator.ApplyGroup(internalGroup);
            if (entityIterator is IEntityAddedListener addedListener)
            {
                internalGroup.Add(addedListener);
            }
            if (entityIterator is IEntityRemovingListener removingListener)
            {
                internalGroup.Add(removingListener);
            }
        }
    }
}