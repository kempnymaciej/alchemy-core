using AlchemyBow.Core.IoC;
using AlchemyBow.Core.IoC.Installers;
using AlchemyBow.Core.Loading;
using AlchemyBow.Core.Signals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace AlchemyBow.Core
{
    public abstract class CoreController<T> : MonoBehaviour where T : CoreProjectContext
    {
        private static T projectContext;
        private static AsyncOperation previousSceneUnloading;

        [SerializeField]
        private MonoInstaller[] installers = null;

        private enum Stage { Starting, Binding, Loading, Working, Unloading }
        private Stage stage;
        private ICoreState state;


        protected virtual float PreLoadingDelay => 0;
        protected virtual float PostLoadingDelay => 0;

        protected SignalSystem SignalSystem { get; private set; }
        protected Container Container { get; private set; }

        private void Awake()
        {
            StartCoroutine(Starting());
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        #region BindingAndLoading
        private IEnumerator Starting()
        {
            stage = Stage.Starting;
            OnStarting();
            yield return null;

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

            if (projectContext != null)
            {
                var operationHandle = new OperationHandle();
                projectContext.Load(operationHandle);
                yield return null;
                yield return new WaitUntil(() => operationHandle.IsDone);
            }

            foreach (var loadable in GetLoadables())
            {
                var operationHandle = new OperationHandle();
                loadable.Load(operationHandle);
                yield return null;
                yield return new WaitUntil(() => operationHandle.IsDone);
            }

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
            OnLoadingFinish();
        }

        protected virtual void OnStarting() { }

        private void EnsureProjectContext()
        {
            if (projectContext == null)
            {
                string path = $"Core/{typeof(T).Name}";
                var prefab = Resources.Load<T>(path);
                if (prefab != null)
                {
                    var instance = Instantiate(prefab);
                    projectContext = instance;
                    DontDestroyOnLoad(instance.gameObject);
                }
                else
                {
                    Debug.LogWarning($"The project context of type {typeof(T)} is not configured. Create a prefab at path: {path} in the Resources folder.");
                }
            }
        }

        private void CreateAndResolveContainer()
        {
            Container = new Container();
            projectContext?.CopyBindingsToContainer(Container);
            InstallSignalSystem();
            InstallBindings(Container);
            Container.ResolveAllBindings();
        }

        private void InstallSignalSystem()
        {
            var declarator = new SignalDeclarator();
            DeclareSignals(declarator);
            SignalSystem = new SignalSystem(declarator);
            Container.Bind(SignalSystem);
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
            OnInstallBindings(container);
        }

        protected virtual void OnInstallBindings(IBindOnlyContainer container) { }

        protected virtual void OnBindingFinished() { }

        protected abstract IEnumerable<ICoreLoadable> GetLoadables();

        protected virtual void OnLoadingFinish()
        {
            Container.ForeachResolved<ICoreLoadingFinishedHandler>(handler => handler.OnLoadingFinished());
        } 
        #endregion

        #region SceneChange
        protected void ChangeScene(int buildIndex)
        {
            if (state != null)
            {
                Debug.LogWarning("Since the scene change process is a coroutine, consider changing the state to null first.");
            }
            if (stage != Stage.Working)
            {
                Debug.LogError($"The controller is in the'{stage}' stage. The scene cannot be changed now.");
                return;
            }
            stage = Stage.Unloading;
            StartCoroutine(ChangingScene(buildIndex));
        }

        private IEnumerator ChangingScene(int buildIndex)
        {
            OnSceneChangeStart();
            EventSystem.current.enabled = false;
            var sceneToUnload = gameObject.scene;
            var loading = SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Additive);
            yield return new WaitUntil(() => loading.isDone);
            previousSceneUnloading = SceneManager.UnloadSceneAsync(sceneToUnload);
        }

        protected virtual void OnSceneChangeStart()
        {
            Container.ForeachResolved<ICoreSceneChangeStartHandler>(handler => handler.OnSceneChangeStart());
        } 
        #endregion

        #region State
        protected void ChangeState(ICoreState state)
        {
            this.state?.OnDeinit();
            this.state = state;
            this.state?.OnInit();
        }

        private void Update()
        {
            state?.OnUpdate();
        }

        private void FixedUpdate()
        {
            state?.OnFixedUpdate();
        }
        #endregion

        protected abstract void DeclareSignals(SignalDeclarator declarator);
    } 
}
