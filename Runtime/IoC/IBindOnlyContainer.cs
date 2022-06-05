using System;
using System.Collections.Generic;

namespace AlchemyBow.Core.IoC
{
    /// <summary>
    /// Provides a dependency injection container interface that allows only binding operations.
    /// </summary>
    public interface IBindOnlyContainer
    {
        /// <summary>
        /// Declares a new key-value pair and informs the container that it should inject dependencies to the instance of the value. 
        /// </summary>
        /// <typeparam name="T">The key.</typeparam>
        /// <param name="value">The instance of the value.</param>
        /// <remarks>The dependencies are injected during the resolve process.</remarks>
        void Bind<T>(T value);

        /// <summary>
        /// Declares a new key-value pair and informs the container that it should inject dependencies to the instance of the value. 
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The instance of the value.</param>
        /// <remarks>The dependencies are injected during the resolve process.</remarks>
        void Bind(Type key, object value);

        /// <summary>
        /// Informs the container that it should inject dependencies to the instance.
        /// </summary>
        /// <param name="instance">The instance to inform about.</param>
        /// <remarks>The dependencies are injected during the resolve process.</remarks>
        void BindInaccessible(object instance);

        /// <summary>
        /// Ensures that there is a key-value pair for the dynamic collection.
        /// </summary>
        /// <typeparam name="TCollection">The key of the dynamic collection.</typeparam>
        void EnsureDynamicCollectionBinding<TCollection>();

        /// <summary>
        /// Ensures that there is a key-value pair for the dynamic collection and adds the specified item.
        /// </summary>
        /// <typeparam name="TCollection">The key of the dynamic collection.</typeparam>
        /// <typeparam name="TItem">The type of items in the collection.</typeparam>
        /// <param name="item">The item to add.</param>
        /// <remarks>
        /// Informs the container that it should inject dependencies to the instance of the collection but not into the item. (The dependencies are injected during the resolve process.)
        /// </remarks>
        void AddToDynamicCollectionBinding<TCollection, TItem>(TItem item)
            where TCollection : ICollection<TItem>;

        /// <summary>
        /// Ensures that there is a key-value pair for the dynamic collection and adds specified items.
        /// </summary>
        /// <typeparam name="TCollection">The key of the dynamic collection.</typeparam>
        /// <typeparam name="TItem">The type of items in the collection.</typeparam>
        /// <param name="items">The items to add.</param>
        /// <remarks>
        /// Informs the container that it should inject dependencies to the instance of the collection but not into the items. (The dependencies are injected during the resolve process.)
        /// </remarks>
        void AddRangeToDynamicCollectionBinding<TCollection, TItem>(IEnumerable<TItem> items)
            where TCollection : ICollection<TItem>;
    } 
}
