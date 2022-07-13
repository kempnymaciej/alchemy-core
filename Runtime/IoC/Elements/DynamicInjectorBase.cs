using System;

namespace AlchemyBow.Core.IoC.Elements
{
    /// <summary>
    /// A base class for dynamic injectors.
    /// </summary>
    public abstract class DynamicInjectorBase 
    {
        /// <summary>
        /// The type of the injection target class. 
        /// </summary>
        public readonly Type injectionTargetType;

        /// <summary>
        /// A field setter that can be used to inject the fields of injection target class instances.
        /// </summary>
        /// <remarks>The value of this field is set internally by the <c>Container</c> when the injector is resolved.</remarks>
        protected internal ReflectionFieldsSetter FieldSetter { get; internal set; }

        /// <summary>
        /// Creates an instance of the class.
        /// </summary>
        /// <param name="injectionTargetType">The type of the injection target class.</param>
        protected DynamicInjectorBase(Type injectionTargetType)
        {
            this.injectionTargetType = injectionTargetType ?? throw new ArgumentNullException(nameof(injectionTargetType));
        }
    }
}
