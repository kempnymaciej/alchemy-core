using System;

namespace AlchemyBow.Core.IoC
{
    /// <summary>
    /// Indicates that a class should be analysed for fields decorated with the <c>[Inject]</c> attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class InjectionTargetAttribute : Attribute
    {

    } 
}
