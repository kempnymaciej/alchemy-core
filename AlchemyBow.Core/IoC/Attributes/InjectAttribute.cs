using System;

namespace AlchemyBow.Core.IoC
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property)]
    public class InjectAttribute : Attribute
    {

    } 
}
