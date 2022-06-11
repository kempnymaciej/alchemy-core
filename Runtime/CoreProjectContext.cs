using AlchemyBow.Core.IoC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlchemyBow.Core
{
    /// <summary>
    /// The base class for all project contexts.
    /// </summary>
    public abstract class CoreProjectContext : MonoBehaviour
    {
        private enum Stage { NotInitiated, Initiated, Loading, Completed }

        [SerializeField]
        private MonoInstaller[] installers = new MonoInstaller[0];

        private Stage stage = Stage.NotInitiated;
        private OperationHandle loadingHandle;

        /// <summary>
        /// The container used for dependency injection.
        /// </summary>
        /// <returns>
        /// The container used for dependency injection or <c>null</c> before the binding stage.
        /// </returns>
        protected Container Container { get; private set; }

        /// <summary>
        /// Copies the contents of its own dependency injection container to another container.
        /// </summary>
        /// <param name="target">The target container.</param>
        /// <remarks>
        /// The dynamic collections bindings are sealed in the target container.
        /// </remarks>
        public void CopyBindingsToContainer(Container target)
        {
            if (stage == Stage.NotInitiated)
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
                stage = Stage.Initiated;
            }
            Container.CopyContent(Container, target, true);
        }

        /// <summary>
        /// Override this method to install additional bindings.
        /// </summary>
        /// <param name="container">The dependency injection container.</param>
        protected virtual void InstallAdditionalBindings(IBindOnlyContainer container) { }

        /// <summary>
        /// Override this method to specify ordered loading components.
        /// </summary>
        /// <returns>Returns ordered loading components.</returns>
        protected abstract IEnumerable<ICoreLoadable> GetLoadables();

        /// <summary>
        /// Starts the loading process.
        /// </summary>
        /// <param name="handle">The handle of the loading process.</param>
        public void Load(OperationHandle handle)
        {
            switch (stage)
            {
                case Stage.NotInitiated:
                    Debug.LogError("The project context was not initiated. Cannot proceed.");
                    break;
                case Stage.Initiated:
                    stage = Stage.Loading;
                    loadingHandle = handle;
                    StartCoroutine(Loading());
                    break;
                case Stage.Loading:
                    if (loadingHandle.IsDone)
                    {
                        handle.MarkDone();
                    }
                    else
                    {
                        loadingHandle = handle;
                    }
                    break;
                case Stage.Completed:
                    handle.MarkDone();
                    break;
            }
        }

        private IEnumerator Loading()
        {
            var loadables = GetLoadables();
            if (loadables != null)
            {
                foreach (var loadable in loadables)
                {
                    var operationHandle = new OperationHandle();
                    loadable.Load(operationHandle);
                    yield return null;
                    yield return new WaitUntil(() => operationHandle.IsDone);
                } 
            }
            stage = Stage.Completed;
            loadingHandle.MarkDone();
        }
    }
}
