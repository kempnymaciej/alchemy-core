using System;

namespace AlchemyBow.Core.IoC
{
    public interface IBindOnlyContainer
    {
        void Bind<T>(T instance);

        void Bind<TKey, TValue>(TValue instance);

        void Bind(Type key, Type value, object instance);

        void BindInaccessible(object instance);
    } 
}
