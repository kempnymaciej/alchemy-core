using AlchemyBow.Core.Elements;
using AlchemyBow.Core.IoC;
using System.Collections.Generic;
using UnityEngine;

namespace AlchemyBow.Core
{
    /// <summary>
    /// The base class for all project contexts.
    /// </summary>
    public abstract class CoreProjectContext : MonoBehaviour
    {
        [SerializeField]
        private MonoInstaller[] installers = new MonoInstaller[0];

        private bool initiated;
        private LoadablesProcess loadingProcess;

        /// <summary>
        /// Returns the container used for dependency injection or <c>null</c> before the binding stage.
        /// </summary>
        /// <returns>
        /// The container used for dependency injection or <c>null</c> before the binding stage.
        /// </returns>
        protected Container Container { get; private set; }

        /// <summary>
        /// Override this method to install additional bindings.
        /// </summary>
        /// <param name="container">The dependency injection container.</param>
        protected virtual void InstallAdditionalBindings(IBindOnlyContainer container) { }

        /// <summary>
        /// Override this method to specify an ordered collection of loading members.
        /// </summary>
        /// <returns>Returns an ordered collection of loading members or <c>null</c>.</returns>
        protected abstract IEnumerable<ICoreLoadable> GetLoadables();

        /// <summary>
        /// Copies the contents of its own dependency injection container to another container.
        /// </summary>
        /// <param name="target">The target container.</param>
        /// <remarks>
        /// The dynamic collections bindings are sealed in the target container.
        /// </remarks>
        internal void CopyBindingsToContainer(Container target)
        {
            EnsureInitated();
            Container.CopyContent(Container, target, true);
        }

        /// <summary>
        /// Gets the loading process.
        /// </summary>
        /// <returns>The loading process.</returns>
        internal LoadablesProcess GetLoadingProcess()
        {
            EnsureInitated();
            if (loadingProcess == null)
            {
                loadingProcess = new LoadablesProcess(this, GetLoadables());
            }
            return loadingProcess;
        }

        private void EnsureInitated()
        {
            if (!initiated)
            {
                Container = new Container();
                Container.BindInaccessible(this);
                if (installers != null)
                {
                    foreach (var installer in installers)
                    {
                        installer.InstallBindings(Container);
                    }
                }
                InstallAdditionalBindings(Container);
                Container.ResolveAllBindings();
                initiated = true;
            }
        }
    }
}
