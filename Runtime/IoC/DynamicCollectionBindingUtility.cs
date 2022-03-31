using System.Collections.Generic;

namespace AlchemyBow.Core.IoC
{
    /// <summary>
    /// A utility class that simplifies adding items to dynamic collections of common types.
    /// </summary>
    public static class DynamicCollectionBindingUtility
    {
        /// <summary>
        /// A <c>List</c> shortcut for <c>IBindOnlyContainer.AddToDynamicCollectionBinding</c>
        /// </summary>
        /// <typeparam name="TItem">The type of items in the collection.</typeparam>
        /// <param name="container">The dependency injection container.</param>
        /// <param name="item">The item to add.</param>
        public static void AddToDynamicListBinding<TItem>(this IBindOnlyContainer container, TItem item)
        {
            container.AddToDynamicCollectionBinding<List<TItem>, TItem>(item);
        }
        /// <summary>
        /// A <c>List</c> shortcut for <c>IBindOnlyContainer.AddRangeToDynamicCollectionBinding</c>
        /// </summary>
        /// <typeparam name="TItem">The type of items in the collection.</typeparam>
        /// <param name="container">The dependency injection container.</param>
        /// <param name="items">The items to add.</param>
        public static void AddRangeToDynamicListBinding<TItem>(this IBindOnlyContainer container, IEnumerable<TItem> items)
        {
            container.AddRangeToDynamicCollectionBinding<List<TItem>, TItem>(items);
        }

        /// <summary>
        /// A <c>HashSet</c> shortcut for <c>IBindOnlyContainer.AddToDynamicCollectionBinding</c> 
        /// </summary>
        /// <typeparam name="TItem">The type of items in the collection.</typeparam>
        /// <param name="container">The dependency injection container.</param>
        /// <param name="item">The item to add.</param>
        public static void AddToDynamicHashSetBinding<TItem>(this IBindOnlyContainer container, TItem item)
        {
            container.AddToDynamicCollectionBinding<HashSet<TItem>, TItem>(item);
        }
        /// <summary>
        /// A <c>HashSet</c> shortcut for <c>IBindOnlyContainer.AddRangeToDynamicCollectionBinding</c> 
        /// </summary>
        /// <typeparam name="TItem">The type of items in the collection.</typeparam>
        /// <param name="container">The dependency injection container.</param>
        /// <param name="items">The items to add.</param>
        public static void AddRangeToDynamicHashSetBinding<TItem>(this IBindOnlyContainer container, IEnumerable<TItem> items)
        {
            container.AddRangeToDynamicCollectionBinding<HashSet<TItem>, TItem>(items);
        }

        /// <summary>
        /// A <c>Dictionary</c> shortcut for <c>IBindOnlyContainer.AddToDynamicCollectionBinding</c>  
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="container">The dependency injection container.</param>
        /// <param name="key">The key of the item to add.</param>
        /// <param name="value">The value of the item to add.</param>
        public static void AddToDynamicDictionaryBinding<TKey, TValue>(this IBindOnlyContainer container, TKey key, TValue value)
        {
            container.AddToDynamicCollectionBinding<Dictionary<TKey, TValue>, KeyValuePair<TKey, TValue>>
                (new KeyValuePair<TKey, TValue>(key, value));
        }
        /// <summary>
        /// A <c>Dictionary</c> shortcut for <c>IBindOnlyContainer.AddToDynamicCollectionBinding</c>
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="container">The dependency injection container.</param>
        /// <param name="item">The key-value pair to add.</param>
        public static void AddToDynamicDictionaryBinding<TKey, TValue>(this IBindOnlyContainer container, KeyValuePair<TKey, TValue> item)
        {
            container.AddToDynamicCollectionBinding<Dictionary<TKey, TValue>, KeyValuePair<TKey, TValue>>(item);
        }
        /// <summary>
        /// A <c>Dictionary</c> shortcut for <c>IBindOnlyContainer.AddRangeToDynamicCollectionBinding</c>
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="container">The dependency injection container.</param>
        /// <param name="items">The key-value pairs to add.</param>
        public static void AddRangeToDynamicDictionaryBinding<TKey, TValue>(this IBindOnlyContainer container, IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            container.AddRangeToDynamicCollectionBinding<Dictionary<TKey, TValue>, KeyValuePair<TKey, TValue>>(items);
        }
    } 
}
