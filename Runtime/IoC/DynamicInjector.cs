using AlchemyBow.Core.IoC.Elements;

namespace AlchemyBow.Core.IoC
{
    /// <summary>
    /// Provides a mechanism for automated dynamic injection.
    /// </summary>
    /// <typeparam name="T">The type of the injection target class.</typeparam>
    public sealed class DynamicInjector<T> : DynamicInjectorBase 
        where T : class
    {
        /// <summary>
        /// Creates an instance of the class.
        /// </summary>
        public DynamicInjector() : base(typeof(T)) { }

        /// <summary>
        /// Injects values into the target object.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <remarks>Only works if the injector has been bound and resolved in the container.</remarks>
        /// <returns>The object passed as a parameter (<c>target</c>).</returns>
        public T Inject(T target)
        {
            FieldSetter.SetFields(target);
            return target;
        }
    } 
}
