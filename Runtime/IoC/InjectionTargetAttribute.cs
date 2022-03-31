using System;

namespace AlchemyBow.Core.IoC
{
    /// <summary>
    /// Indicates that a class should be analysed for <c>[Inject]</c> fields.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class InjectionTargetAttribute : Attribute
    {

    } 
}
