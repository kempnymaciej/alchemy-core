using UnityEngine;

namespace AlchemyBow.Core.IoC
{
    /// <summary>
    /// The base class for the <c>MonoBehaviour</c> based dependency injection installers.
    /// </summary>
    public abstract class MonoInstaller : MonoBehaviour
    {
        /// <summary>
        /// Override this method to install bindings in the dependency injection container.
        /// </summary>
        /// <param name="container">The dependency injection container.</param>
        public abstract void InstallBindings(IBindOnlyContainer container);
    }
}
