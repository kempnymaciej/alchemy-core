using System;

namespace AlchemyBow.Core.IoC.Exceptions
{
    public class ContainerResolveException : Exception
    {
        public ContainerResolveException(Type key, Exception innerException) 
            : base(CreateMessage(key, innerException.Message), innerException) { }

        private static string CreateMessage(Type key, string message)
        {
            return $"The key({key}) has caused the container to crash.\n{message}";
        }
    }
}
