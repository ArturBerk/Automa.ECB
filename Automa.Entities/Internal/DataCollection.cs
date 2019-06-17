using System;
using Automa.Common;

namespace Automa.Entities.Internal
{
    internal class DataCollection<TEntity> : IDataCollection<TEntity> where TEntity : struct
    {
        private ArrayList<TEntity> entities = new ArrayList<TEntity>(4);

        public event DataAddedHandler<TEntity> Added;
        public event DataRemovedHandler<TEntity> Removed;

        public Type Type => TypeOf<TEntity>.Type;
        public int Count => entities.Count;

        public ref TEntity this[int index] => ref entities.Buffer[index];

        public void Add(TEntity entity)
        {
            entities.Add(entity);
            Added?.Invoke(ref entity);
        }

        public void Add(ref TEntity entity)
        {
            entities.Add(entity);
            Added?.Invoke(ref entity);
        }

        public void Clear()
        {
            if (Removed != null)
            {
                for (int i = 0; i < entities.Count; i++)
                {
                    Removed(ref entities.Buffer[i]);
                }
            }
            entities.Clear();
        }

        public void Remove(int i)
        {
            Removed?.Invoke(ref entities.Buffer[i]);
            entities.UnorderedRemoveAt(i);
        }
    }

}