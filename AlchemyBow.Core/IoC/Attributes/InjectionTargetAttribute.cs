using System;

namespace AlchemyBow.Core.IoC
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class InjectionTargetAttribute : Attribute
    {
        public readonly InjectionType injectionType;

        public InjectionTargetAttribute()
        {
            this.injectionType = InjectionType.Fields;
        }

        public InjectionTargetAttribute(InjectionType injectionType)
        {
            this.injectionType = injectionType;
        }
    } 
}
