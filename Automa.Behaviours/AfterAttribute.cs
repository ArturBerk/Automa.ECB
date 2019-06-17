using System;

namespace Automa.Behaviours
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public sealed class AfterAttribute : Attribute
    {
        public readonly Type Type;

        public AfterAttribute(Type type)
        {
            Type = type;
        }
    }
}