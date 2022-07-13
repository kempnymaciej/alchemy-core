using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlchemyBow.Core.Elements
{
    /// <summary>
    /// Provides a mechanism for starting and observing (loadables) loading processes.
    /// </summary>
    internal sealed class LoadablesProcess
    {
        /// <summary>
        /// An event that is raised when the process progresses.
        /// </summary>
        public event System.Action<LoadablesProgress> LoadablesProgressed;

        /// <summary>
        /// The total number of loadables that are involved in the process.
        /// </summary>
        public readonly int numberOfLoadables;

        private readonly MonoBehaviour runner;
        private ICoreLoadable[] loadables;
        private bool started;

        /// <summary>
        /// Determines whether the process is complete. 
        /// </summary>
        /// <returns><c>true</c> if complete; Otherwise, <c>false</c>.</returns>
        public bool Completed { get; private set; }

        /// <summary>
        /// Creates a loading process.
        /// </summary>
        /// <param name="runner">The runner for the process.</param>
        /// <param name="loadables">The members of the process.</param>
        public LoadablesProcess(MonoBehaviour runner, IEnumerable<ICoreLoadable> loadables)
        {
            this.runner = runner;
            this.loadables = loadables != null ? System.Linq.Enumerable.ToArray(loadables) : null;
            numberOfLoadables = this.loadables != null ? this.loadables.Length : 0;
            if (numberOfLoadables == 0)
            {
                Completed = true;
            }
        }

        /// <summary>
        /// Ensures the process is started.
        /// </summary>
        public void EnsureStarted()
        {
            if (!started)
            {
                started = true;
                if (!Completed && numberOfLoadables > 0)
                {
                    runner.StartCoroutine(CreateLoadingCoroutine()); 
                }            
            }
        }

        private IEnumerator CreateLoadingCoroutine()
        {
            for (int i = 0; i < numberOfLoadables; i++)
            {
                LoadablesProgressed?.Invoke(new LoadablesProgress(loadables[i], i, false, numberOfLoadables));
                var operationHandle = new OperationHandle();
                loadables[i].Load(operationHandle);
                yield return null;
                yield return new WaitUntil(() => operationHandle.IsDone);
                LoadablesProgressed?.Invoke(new LoadablesProgress(loadables[i], i, true, numberOfLoadables));
            }
            loadables = null;
            Completed = true;
        }


        /// <summary>
        /// Creates an empty(complete) process.
        /// </summary>
        /// <returns>An empty(complete) process.</returns>
        public static LoadablesProcess CreateEmpty() 
        {
            return new LoadablesProcess(null, null)
            {
                started = true
            };
        }
    } 
}
