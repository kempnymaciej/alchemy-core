using System;
using System.Collections.Generic;
using AlchemyBow.Core.IoC;

namespace AlchemyBow.Core.Extras.FluentBindings
{
    /// <summary>
    /// Represents a binding that can be configured with method chaining.
    /// </summary>
    public class FluentBinding
    {
        private readonly IBindOnlyContainer container;
		private readonly object instance;

        /// <summary>
        /// Creates a new instance of the <see cref="FluentBinding"/> class.
        /// </summary>
        /// <param name="container">The container used for binding.</param>
        /// <param name="instance">The instance to bind.</param>
        public FluentBinding(IBindOnlyContainer container, object instance)
        {
            this.container = container;
            this.instance = instance;
        }

        /// <summary>
        /// Binds the instance to its own type.
        /// </summary>
        public FluentBinding BindToSelf()
        {
            container.Bind(instance.GetType(), instance);
            return this;
        }
        
        /// <summary>
        /// Binds the instance to the specified type.
        /// </summary>
        public FluentBinding BindTo<TKey>()
        {
            container.Bind(typeof(TKey), instance);
            return this;
        }
        
        /// <summary>
        /// Binds the instance to the specified type.
        /// </summary>
        public FluentBinding BindTo(Type key)
        {
            container.Bind(key, instance);
            return this;
        }
        
        /// <summary>
        /// Binds the instance as inaccessible.
        /// </summary>
        public FluentBinding BindInaccessible()
        {
            container.BindInaccessible(instance);
            return this;
        }

        /// <summary>
        /// Adds the instance to the dynamic collection binding.
        /// </summary>
        public FluentBinding AddToDynamicCollectionBinding<TCollection, TItem>()
            where TCollection : ICollection<TItem>
        {
            container.AddToDynamicCollectionBinding<TCollection, TItem>((TItem)instance);
            return this;
        }
        
        /// <summary>
        /// Adds the instance to the dynamic list of core loading callbacks handlers.
        /// </summary>
        public void AddToCoreLoadingCallbacksHandlers()
        {
            if (instance is ICoreLoadingCallbacksHandler callbacksHandler)
            {
                container.AddToDynamicListBinding(callbacksHandler);
            }
            else
            {
                throw new InvalidOperationException($"The instance of the type {instance.GetType()} does not implement the {nameof(ICoreLoadingCallbacksHandler)} interface.");
            }
        }
    }
}