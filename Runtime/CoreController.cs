using AlchemyBow.Core.Elements;
using AlchemyBow.Core.IoC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AlchemyBow.Core
{
    /// <summary>
    /// The base class for all core controllers.
    /// </summary>
    /// <typeparam name="TProjectContext">The type of the project context visible for the controller.</typeparam>
    [InjectionTarget]
    public abstract class CoreController<TProjectContext> : MonoBehaviour where TProjectContext : CoreProjectContext
    {
        private static TProjectContext projectContext;
        private static AsyncOperation previousSceneUnloading;

        [Inject]
        private readonly List<ICoreLoadingCallbacksHandler> loadingCallbacksHandlers;

        [SerializeField]
        private MonoInstaller[] installers = new MonoInstaller[0];

        private enum Stage { Starting, Binding, Loading, Working, Unloading }
        private Stage stage;

        /// <summary>
        /// Returns the container used for dependency injection or <c>null</c> before the binding stage.
        /// </summary>
        /// <returns>
        /// The container used for dependency injection or <c>null</c> before the binding stage.
        /// </returns>
        protected Container Container { get; private set; }

        /// <summary>
        /// The additional delay between the binding and loading stages (measured in seconds, real time).
        /// </summary>
        protected virtual float PreLoadingDelay => 0;

        /// <summary>
        /// The additional delay between the loading and working stages (measured in seconds, real time).
        /// </summary>
        protected virtual float PostLoadingDelay => 0;

        private void Awake()
        {
            StartCoroutine(CreateLoadCoroutine());
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        #region BindingAndLoading
        private IEnumerator CreateLoadCoroutine()
        {
            stage = Stage.Starting;
            var startOperationHandle = new OperationHandle();
            OnStarted(startOperationHandle);
            yield return null;
            yield return new WaitUntil(() => startOperationHandle.IsDone);

            stage = Stage.Binding;
            EnsureProjectContext();
            CreateAndResolveContainer();
            OnBindingFinished();
            yield return null;

            stage = Stage.Loading;
            if (PreLoadingDelay > 0)
            {
                yield return new WaitForSecondsRealtime(PreLoadingDelay);
            }
            yield return null;

            var projectContextLoadablesProcess = projectContext?.GetLoadingProcess() ?? LoadablesProcess.CreateEmpty();
            var controllerLoadablesProcess = new LoadablesProcess(this, GetLoadables());

            yield return new LoadCompositeLoadablesProcess(
                processes: new LoadablesProcess[] { projectContextLoadablesProcess, controllerLoadablesProcess },
                progressedCallback: OnLoadablesProgressed);

            if (PostLoadingDelay > 0)
            {
                yield return new WaitForSecondsRealtime(PostLoadingDelay);
            }
            yield return null;

            if(previousSceneUnloading != null)
            {
                yield return new WaitUntil(() => previousSceneUnloading.isDone);
                previousSceneUnloading = null;
            }

            stage = Stage.Working;
            OnLoadingFinished();
        }

        /// <summary>
        /// This method is called when the script instance is being loaded. It can be used to perform operations such as turning on a loading screen.
        /// </summary>
        /// <param name="operationHandle">The handle of the operation. (The base verision of the method marks the operation handle done.)</param>
        /// <remarks>
        /// It's called before the binding stage, so dependencies are not injected yet.
        /// </remarks>
        protected virtual void OnStarted(OperationHandle operationHandle) 
        {
            operationHandle.MarkDone();
        }

        private void EnsureProjectContext()
        {
            if (projectContext == null)
            {
                string path = $"Core/{typeof(TProjectContext).Name}";
                var prefab = Resources.Load<TProjectContext>(path);
                if (prefab != null)
                {
                    var instance = Instantiate(prefab);
                    projectContext = instance;
                    DontDestroyOnLoad(instance.gameObject);
                }
                else
                {
                    Debug.LogWarning($"The project context of type {typeof(TProjectContext)} is not configured. Create a prefab at path: {path} in the Resources folder.");
                }
            }
        }

        private void CreateAndResolveContainer()
        {
            Container = new Container();
            projectContext?.CopyBindingsToContainer(Container);
            Container.BindInaccessible(this);
            Container.EnsureDynamicCollectionBinding<List<ICoreLoadingCallbacksHandler>>();
            InstallBindings(Container);
            Container.ResolveAllBindings();
        }

        private void InstallBindings(IBindOnlyContainer container)
        {
            if (installers != null)
            {
                foreach (var installer in installers)
                {
                    installer.InstallBindings(container);
                }
            }
            InstallAdditionalBindings(container);
        }

        /// <summary>
        /// Override this method to install additional bindings.
        /// </summary>
        /// <param name="container">The dependency injection container.</param>
        protected virtual void InstallAdditionalBindings(IBindOnlyContainer container) { }

        /// <summary>
        /// This method is called after the dependencies are resolved (before the start of the loading stage).
        /// </summary>
        protected virtual void OnBindingFinished() { }

        /// <summary>
        /// Override this method to specify an ordered collection of loading members.
        /// </summary>
        /// <returns>Returns an ordered collection of loading members or <c>null</c>.</returns>
        protected abstract IEnumerable<ICoreLoadable> GetLoadables();

        /// <summary>
        /// Override this method to respond to the the loading process progress. For example, to update your loading screen.
        /// </summary>
        /// <param name="progress">The current progress.</param>
        /// <remarks>This method is called twice for each loadable during the loading process (not once if there are none). Project context loadables are included if they haven't been loaded previously.</remarks>
        protected virtual void OnLoadablesProgressed(LoadablesProgress progress)  { }

        /// <summary>
        /// This method is called when binding and loading is completed (when the controller enters the working stage).
        /// </summary>
        /// <remarks>
        /// By default, it invokes <c>ICoreLoadingCallbacksHandler.OnCoreLoadingFinished()</c>.
        /// </remarks>
        protected virtual void OnLoadingFinished()
        {
            foreach (var item in loadingCallbacksHandlers)
            {
                item.OnCoreLoadingFinished();
            }
        } 
        #endregion

        #region SceneChange
        /// <summary>
        /// Starts a scene change process.
        /// </summary>
        /// <param name="buildIndex">The build index of the scene to load.</param>
        protected void ChangeScene(int buildIndex)
        {
            if (stage != Stage.Working)
            {
                Debug.LogError($"The controller is in the '{stage}' stage. The scene cannot be changed now.");
                return;
            }
            stage = Stage.Unloading;
            StartCoroutine(CreateChangeSceneCoroutine(buildIndex));
        }

        private IEnumerator CreateChangeSceneCoroutine(int buildIndex)
        {
            OnSceneChangeStarted();
            var keepUnloadingHandle = new OperationHandle();
            OnKeepUnloading(keepUnloadingHandle);
            if (!keepUnloadingHandle.IsDone)
            {
                yield return new WaitUntil(() => keepUnloadingHandle.IsDone); 
            }
            var sceneToUnload = gameObject.scene;
            var loading = SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Additive);
            yield return new WaitUntil(() => loading.isDone);
            previousSceneUnloading = SceneManager.UnloadSceneAsync(sceneToUnload);
        }

        /// <summary>
        /// This method is called when the scene change process begins. By default, it invokes <c>ICoreLoadingCallbacksHandler.OnCoreSceneChangeStarted()</c>.
        /// </summary>
        /// <remarks>
        /// If you use the <c>UnityEngine.UI</c> package, you may need to add <c>UnityEngine.EventSystems.EventSystem.current.enabled = false;</c> here.
        /// </remarks>
        protected virtual void OnSceneChangeStarted()
        {
            foreach (var item in loadingCallbacksHandlers)
            {
                item.OnCoreSceneChangeStarted();
            }
        }

        /// <summary>
        /// This method is called after <c>OnSceneChangeStarted()</c> and before the actual scene change process. You can use it to clean up external resources, such as additional scenes.
        /// </summary>
        /// <param name="operationHandle">The handle of the operation. (The base verision of the method marks the operation handle done.)</param>
        protected virtual void OnKeepUnloading(OperationHandle operationHandle)
        {
            operationHandle.MarkDone();
        }

        /// <summary>
        /// Destroys the project context. A potential use of this method is to allow you to switch between different project contexts.
        /// </summary>
        protected void DestroyProjectContext()
        {
            if (projectContext != null)
            {
                Destroy(projectContext.gameObject);
                projectContext = null;
            }
        }
        #endregion
    }
}
