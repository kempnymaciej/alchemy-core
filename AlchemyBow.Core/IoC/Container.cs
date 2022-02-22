using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using AlchemyBow.Core.IoC.Exceptions;

namespace AlchemyBow.Core.IoC
{
    public sealed class Container : IBindOnlyContainer
    {
        private readonly Dictionary<Type, Binding> bindings;
        private readonly Dictionary<object, Binding> inaccessibleBindings;
        private readonly HashSet<object> resolved;

        public Container()
        {
            this.bindings = new Dictionary<Type, Binding>();
            this.inaccessibleBindings = new Dictionary<object, Binding>();
            this.resolved = new HashSet<object>();
        }

        public void Bind<T>(T instance)
        {
            Bind<T, T>(instance);
        }

        public void Bind<TKey, TValue>(TValue instance)
        {
            bindings.Add(typeof(TKey), new Binding(typeof(TValue), instance));
        }

        public void Bind(Type key, Type value, object instance)
        {
            if (!key.IsAssignableFrom(value))
            {
                throw new Exception($"Unable to bind key-value pair. The key ({key}) must be assignable from the value ({value})");
            }
            if (!key.IsInstanceOfType(instance) || !value.IsInstanceOfType(instance))
            {
                throw new Exception($"Unable to bind key-value pair. The instance must be an instance of both a key({key}) and a value ({value})");
            }
            bindings.Add(key, new Binding(value, instance));
        }

        public void BindInaccessible(object instance)
        {
            inaccessibleBindings.Add(instance, new Binding(instance.GetType(), instance));
        }

        public T Resolve<T>()
        {
            T result;
            try
            {
                result = (T)Resolve(typeof(T));
            }
            catch (Exception innerException)
            {
                throw new ContainerResolveException(typeof(T), innerException);
            }
            return result;
        }

        private object Resolve(Type key)
        {
            if (!bindings.TryGetValue(key, out Binding binding))
            {
                throw new Exception($"Cannot relove a key. {key} has no binding.");
            }
            ResolveAndInjectBinding(binding);
            return binding.instance;
        }


        public void ResolveAllBindings()
        {
            foreach (var item in bindings.Values)
            {
                ResolveAndInjectBinding(item);
            }
            foreach (var item in inaccessibleBindings.Values)
            {
                ResolveAndInjectBinding(item);
            }
        }

        private void ResolveAndInjectBinding(Binding binding)
        {
            if (resolved.Add(binding.instance))
            {
                InjectBinding(binding);
            }
        }

        private void InjectBinding(Binding binding)
        {
            var injectionType = GetInjectionType(binding.type);
            if (injectionType != InjectionType.None)
            {
                if ((injectionType & InjectionType.Fields) != InjectionType.None)
                {
                    InjectBindingFields(binding);
                }
                if ((injectionType & InjectionType.Properties) != InjectionType.None)
                {
                    InjectBindingProperties(binding);
                }
                if ((injectionType & InjectionType.Methodes) != InjectionType.None)
                {
                    InjectBindingMethods(binding);
                }
            }
        }

        private void InjectBindingFields(Binding binding)
        {
            foreach (var item in GetInjectFields(binding.type))
            {
                item.SetValue(binding.instance, Resolve(item.FieldType));
            }
        }

        private void InjectBindingProperties(Binding binding)
        {
            foreach (var item in GetInjectProperties(binding.type))
            {
                item.SetValue(binding.instance, Resolve(item.PropertyType));
            }
        }

        private void InjectBindingMethods(Binding binding)
        {
            foreach (var methodInfo in GetInjectMethods(binding.type))
            {
                var parameterInfos = methodInfo.GetParameters();
                int numberOfParameters = parameterInfos.Length;
                var parameters = new object[numberOfParameters];
                for (int i = 0; i < numberOfParameters; i++)
                {
                    parameters[i] = Resolve(parameterInfos[i].ParameterType);
                }
                methodInfo.Invoke(binding.instance, parameters);
            }
        }

        private static IEnumerable<FieldInfo> GetInjectFields(Type type)
        {
            return type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(fi => fi.GetCustomAttribute<InjectAttribute>() != null);
        }

        private static IEnumerable<PropertyInfo> GetInjectProperties(Type type)
        {
            return type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(fi => fi.GetCustomAttribute<InjectAttribute>() != null);
        }

        private static IEnumerable<MethodInfo> GetInjectMethods(Type type)
        {
            return type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(fi => fi.GetCustomAttribute<InjectAttribute>() != null);
        }

        private static InjectionType GetInjectionType(Type type)
        {
            var injectionTarget = type.GetCustomAttribute<InjectionTargetAttribute>();
            return injectionTarget != null ? injectionTarget.injectionType : InjectionType.None;
        }

        public void ForeachResolved<T>(Action<T> action)
        {
            foreach (var item in resolved)
            {
                if (item is T instance)
                {
                    action.Invoke(instance);
                }
            }
        }

        public static void CopyContent(Container source, Container target)
        {
            foreach (var item in source.bindings)
            {
                target.bindings.Add(item.Key, item.Value);
            }
            foreach (var item in source.inaccessibleBindings)
            {
                target.inaccessibleBindings.Add(item.Key, item.Value);
            }
            foreach (var item in source.resolved)
            {
                target.resolved.Add(item);
            }
        }
    } 
}
