using System;

namespace AlchemyBow.Core.IoC
{
    public sealed class Binding
    {
        public readonly Type type;
        public readonly object instance;

        public Binding(Type type, object instance)
        {
            this.type = type;
            this.instance = instance;
        }
    } 
}
