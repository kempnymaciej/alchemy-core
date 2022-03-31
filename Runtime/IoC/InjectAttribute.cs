using System;

namespace AlchemyBow.Core.IoC
{
    /// <summary>
    /// Indicates that a field should be injected.
    /// </summary>
    /// <remarks>
    /// Only works for classes decorated with the <c>[InjectionTarget]</c> attribute.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field)]
    public class InjectAttribute : Attribute
    {

    } 
}
