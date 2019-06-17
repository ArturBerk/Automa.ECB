using System;
using System.Collections.Generic;
using System.Text;

namespace Automa.Entities
{
    public interface IBaseEntityCollection
    {
        Type Type { get; }
        int Count { get; }
        void Remove(int i);
        void Clear();
    }
}
