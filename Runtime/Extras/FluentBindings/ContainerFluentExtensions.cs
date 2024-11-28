using AlchemyBow.Core.IoC;

namespace AlchemyBow.Core.Extras.FluentBindings
{
    /// <summary>
    /// This class contains extension methods that provide fluent binding operations for the <c>IBindOnlyContainer</c> interface.
    /// </summary>
    public static class ContainerFluentExtensions
    {
        /// <summary>
        /// Starts the fluent binding process, which allows for chaining additional binding steps.
        /// </summary>
        /// <param name="container">The container used for binding.</param>
        /// <param name="instance">The instance to bind.</param>
        /// <returns>A <see cref="FluentBinding"/> object that enables further fluent configuration.</returns>
        public static FluentBinding StartFluentBinding(this IBindOnlyContainer container, object instance)
        {
            return new FluentBinding(container, instance);
        }
    }
}