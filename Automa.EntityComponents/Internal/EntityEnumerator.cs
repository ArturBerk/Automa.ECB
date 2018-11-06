using System;
using System.Collections.Generic;
using System.Reflection;

namespace Automa.EntityComponents.Internal
{
    public class EntityIterator
    {
        internal Group group;
        private int currentChunkLength;

        internal int CurrentChunkIndex;
        internal int CurrentIndex;

        internal Value[] values;

        public EntityIterator()
        {
            List<Value> values = new List<Value>();
            foreach (var fieldInfo in GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                var fieldType = fieldInfo.FieldType;
                if (fieldType.IsGenericType)
                {
                    var genericType = fieldType.GetGenericTypeDefinition();
                    if (genericType == typeof(IValue<>))
                    {
                        var cType = fieldType.GetGenericArguments()[0];
                        var componentValue =
                            (Value)Activator.CreateInstance(typeof(Value<>).MakeGenericType(cType));
                        componentValue.Iterator = this;
                        values.Add(componentValue);
                        fieldInfo.SetValue(this, componentValue);
                    }
                }
            }
            this.values = values.ToArray();
        }

        internal void ApplyGroup(Group group)
        {
            this.group = group;
            Reset();
        }

        public bool MoveNext()
        {
            if (@group == null) throw new EntitiesException("Entity enumerator not binded");
            if (CurrentChunkIndex < 0) return false;
            ++CurrentIndex;
            while (CurrentIndex >= currentChunkLength)
            {
                ++CurrentChunkIndex;
                if (CurrentChunkIndex >= group.Chunks.Count)
                {
                    CurrentChunkIndex = -1;
                    for (var index = 0; index < values.Length; index++)
                    {
                        var value = values[index];
                        value.ChangeChunk(null);
                    }
                    return false;
                }
                var chunk = @group.Chunks[CurrentChunkIndex];
                for (var index = 0; index < values.Length; index++)
                {
                    var value = values[index];
                    value.ChangeChunk(chunk);
                }
                currentChunkLength = group.Chunks.Buffer[CurrentChunkIndex].EntityCount;
                CurrentIndex = 0;
            }
            return true;
        }

        public void Reset()
        {
            currentChunkLength = group.Chunks.Count > 0 ? group.Chunks[0].EntityCount : 0;
            CurrentChunkIndex = group.Chunks.Count > 0 ? 0 : -1;
            if (CurrentChunkIndex >= 0)
            {
                var chunk = @group.Chunks[CurrentChunkIndex];
                for (var index = 0; index < values.Length; index++)
                {
                    var value = values[index];
                    value.ChangeChunk(chunk);
                }
            }
            else
            {
                for (var index = 0; index < values.Length; index++)
                {
                    var value = values[index];
                    value.ChangeChunk(null);
                }
            }
            CurrentIndex = -1;
        }
    }
}