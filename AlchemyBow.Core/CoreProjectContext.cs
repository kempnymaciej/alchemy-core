using AlchemyBow.Core.IoC;
using AlchemyBow.Core.IoC.Installers;
using AlchemyBow.Core.Loading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlchemyBow.Core
{
    public abstract class CoreProjectContext : MonoBehaviour, ICoreLoadable
    {
        private enum Stage { NotInitiated, Initiated, Loading, Completed }
        protected readonly Container container = new Container();

        [SerializeField]
        private MonoInstaller[] installers = null;

        private Stage stage = Stage.NotInitiated;
        private OperationHandle loadingHandle;

        public void CopyBindingsToContainer(Container target)
        {
            if (stage == Stage.NotInitiated)
            {
                if (installers != null)
                {
                    foreach (var installer in installers)
                    {
                        installer.InstallBindings(container);
                    }
                }
                container.ResolveAllBindings();
                stage = Stage.Initiated;
            }
            Container.CopyContent(container, target);
        }
        protected abstract IEnumerable<ICoreLoadable> GetLoadables();

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
            foreach (var loadable in GetLoadables())
            {
                var operationHandle = new OperationHandle();
                loadable.Load(operationHandle);
                yield return null;
                yield return new WaitUntil(() => operationHandle.IsDone);
            }
            stage = Stage.Completed;
            loadingHandle.MarkDone();
        }
    }
}
