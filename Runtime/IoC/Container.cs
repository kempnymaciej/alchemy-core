using System;
using System.Collections.Generic;
using AlchemyBow.Core.IoC.Elements;

namespace AlchemyBow.Core.IoC
{
    /// <summary>
    /// The dependency injection container.
    /// </summary>
    public sealed class Container : IBindOnlyContainer
    {
        private readonly Dictionary<Type, object> bindings;
        private readonly HashSet<object> inaccessibleBindings;
        private readonly HashSet<object> resolved;
        private readonly HashSet<Type> dynamicCollections;

        /// <summary>
        /// Creates an instance of the dependency injection container.
        /// </summary>
        public Container()
        {
            this.bindings = new Dictionary<Type, object>();
            this.inaccessibleBindings = new HashSet<object>();
            this.resolved = new HashSet<object>();
            this.dynamicCollections = new HashSet<Type>();
        }

        /// <summary>
        /// Declares a new key-value pair and informs the container that it should inject dependencies to the instance of the value. 
        /// </summary>
        /// <typeparam name="T">The key.</typeparam>
        /// <param name="value">The instance of the value.</param>
        public void Bind<TKey>(TKey value)
        {
            bindings.Add(typeof(TKey), value);
        }

        /// <summary>
        /// Declares a new key-value pair and informs the container that it should inject dependencies to the instance of the value. 
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The instance of the value.</param>
        /// <remarks>Only use it if the generic overload of the method cannot be used.</remarks>
        public void Bind(Type key, object value)
        {
            if (!key.IsAssignableFrom(value.GetType()))
            {
                throw new Exception($"Unable to bind a key-value pair. The key({key}) must be assignable from the value({value.GetType()})");
            }
            bindings.Add(key, value);
        }

        /// <summary>
        /// Informs the container that it should inject dependencies to the instance.
        /// </summary>
        /// <param name="instance">The instance to inform about.</param>
        public void BindInaccessible(object instance)
        {
            inaccessibleBindings.Add(instance);
        }

        /// <summary>
        /// Ensures that there is a key-value pair for the dynamic collection.
        /// </summary>
        /// <typeparam name="TCollection">The key of the dynamic collection.</typeparam>
        public void EnsureDynamicCollectionBinding<TCollection>()
        {
            GetDynamicCollectionBinding<TCollection>();
        }

        /// <summary>
        /// Ensures that there is a key-value pair for the dynamic collection and adds the specified item.
        /// </summary>
        /// <typeparam name="TCollection">The key of the dynamic collection.</typeparam>
        /// <typeparam name="TItem">The type of items in the collection.</typeparam>
        /// <param name="item">The item to add.</param>
        /// <remarks>
        /// Informs the container that it should inject dependencies to the instance of the collection but not into the item.
        /// </remarks>
        public void AddToDynamicCollectionBinding<TCollection, TItem>(TItem item)
            where TCollection : ICollection<TItem>
        {
            var collection = GetDynamicCollectionBinding<TCollection>();
            if (item != null)
            {
                collection.Add(item);
            }
        }

        /// <summary>
        /// Ensures that there is a key-value pair for the dynamic collection and adds specified items.
        /// </summary>
        /// <typeparam name="TCollection">The key of the dynamic collection.</typeparam>
        /// <typeparam name="TItem">The type of items in the collection.</typeparam>
        /// <param name="items">The items to add.</param>
        /// <remarks>
        /// Informs the container that it should inject dependencies to the instance of the collection but not into the items.
        /// </remarks>
        public void AddRangeToDynamicCollectionBinding<TCollection, TItem>(IEnumerable<TItem> items)
            where TCollection : ICollection<TItem>
        {
            var collection = GetDynamicCollectionBinding<TCollection>();
            if (items != null)
            {
                foreach (var item in items)
                {
                    collection.Add(item);
                }
            }
        }

        private TCollection GetDynamicCollectionBinding<TCollection>()
        {
            var collectionType = typeof(TCollection);
            TCollection collection;
            if (bindings.TryGetValue(collectionType, out var instance))
            {
                if (!dynamicCollections.Contains(collectionType))
                {
                    throw new Exception($"Unable to create a dynamic collection. The type({collectionType}) is already used as a standard binding key.");
                }
                collection = (TCollection)instance;
            }
            else
            {
                collection = (TCollection)Activator.CreateInstance(collectionType);
                Bind(collection);
                dynamicCollections.Add(collectionType);
            }
            return collection;
        }

        /// <summary>
        /// Resolves the specified key.
        /// </summary>
        /// <typeparam name="T">The key to resolve.</typeparam>
        /// <returns>The resolved value that was associated to the key.</returns>
        public T Resolve<T>()
        {
            T result;
            try
            {
                result = (T)Resolve(typeof(T));
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to resolve the key({typeof(T)}).\n{e.Message}");
            }
            return result;
        }

        private object Resolve(Type key)
        {
            if (!bindings.TryGetValue(key, out var instance))
            {
                throw new Exception($"Cannot relove the key({key}). It has no binding.");
            }
            ResolveAndInjectBinding(instance);
            return instance;
        }

        /// <summary>
        /// Resolves all bindings.
        /// </summary>
        public void ResolveAllBindings()
        {
            foreach (var instance in bindings.Values)
            {
                ResolveAndInjectBinding(instance);
            }
            foreach (var instance in inaccessibleBindings)
            {
                ResolveAndInjectBinding(instance);
            }
        }

        private void ResolveAndInjectBinding(object instance)
        {
            if (resolved.Add(instance))
            {
                var injectionInfo = InjectionInfo.GetInjectionInfo(instance.GetType());
                while (injectionInfo != null)
                {
                    if (injectionInfo.declaredInjectFields != null)
                    {
                        foreach (var item in injectionInfo.declaredInjectFields)
                        {
                            item.SetValue(instance, Resolve(item.FieldType));
                        }
                    }
                    injectionInfo = injectionInfo.parentInfo;
                }
            }
        }

        /// <summary>
        /// Copies the content of the source container to the target container.
        /// </summary>
        /// <param name="source">The source container.</param>
        /// <param name="target">The target container.</param>
        /// <param name="sealDynamicCollections">If <c>true</c>, converts dynamic collection bindings into standard binding in the target container.</param>
        public static void CopyContent(Container source, Container target, bool sealDynamicCollections)
        {
            foreach (var item in source.bindings)
            {
                target.bindings.Add(item.Key, item.Value);
            }
            foreach (var item in source.inaccessibleBindings)
            {
                target.inaccessibleBindings.Add(item);
            }
            foreach (var item in source.resolved)
            {
                target.resolved.Add(item);
            }

            if (!sealDynamicCollections)
            {
                foreach (var item in source.dynamicCollections)
                {
                    target.dynamicCollections.Add(item);
                } 
            }
        }
    } 
}
