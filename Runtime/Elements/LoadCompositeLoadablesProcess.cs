using UnityEngine;

namespace AlchemyBow.Core.Elements
{
    /// <summary>
    /// A custom yield instruction that loads loadables processes in sequence and suspends the coroutine execution until completion.
    /// </summary>
    internal sealed class LoadCompositeLoadablesProcess : CustomYieldInstruction
    {
        private readonly LoadablesProcess[] processes;
        private readonly System.Action<LoadablesProgress> progressedCallback;

        private int processIndex = -1;
        private int actualStartProcessIndex = -1;
        private int actualNumberOfLoadables;

        /// <summary>
        /// Initializes a yield instruction with given processes and a progression callback.
        /// </summary>
        /// <param name="processes">The processes to load.</param>
        /// <param name="progressedCallback">The progression callback.</param>
        public LoadCompositeLoadablesProcess(LoadablesProcess[] processes, System.Action<LoadablesProgress> progressedCallback)
            : base()
        {
            this.processes = processes;
            this.progressedCallback = progressedCallback;
        }

        public override bool keepWaiting
        {
            get
            {
                if (processIndex == -1)
                {
                    HandleNotInitatedStage();
                }
                else if (processIndex < processes.Length)
                {
                    HandleInitatedStage();
                }
                else
                {
                    return false;
                }
                return true;
            }
        }

        private void HandleNotInitatedStage()
        {
            processIndex = 0;
            int numberOfProcesses = processes.Length;
            while (processIndex < numberOfProcesses && processes[processIndex].Completed)
            {
                processIndex++;
            }
            if (processIndex < numberOfProcesses)
            {
                actualStartProcessIndex = processIndex;
                for (int i = processIndex; i < numberOfProcesses; i++)
                {
                    actualNumberOfLoadables += processes[i].numberOfLoadables;
                }
                processes[processIndex].LoadablesProgressed += OnLoadablesProgressed;
                processes[processIndex].EnsureStarted(); 
            }
        }

        private void HandleInitatedStage()
        {
            if (processes[processIndex].Completed)
            {
                processes[processIndex].LoadablesProgressed -= OnLoadablesProgressed;
                processIndex++;
                if (processIndex < processes.Length)
                {
                    processes[processIndex].LoadablesProgressed += OnLoadablesProgressed;
                    processes[processIndex].EnsureStarted();
                }
            }
        }

        private void OnLoadablesProgressed(LoadablesProgress progress)
        {
            int loadableIndex = progress.loadableIndex;
            for (int i = actualStartProcessIndex; i < processIndex; i++)
            {
                loadableIndex += processes[i].numberOfLoadables;
            }
            progressedCallback?.Invoke(new LoadablesProgress(
                loadable: progress.loadable,
                loadableIndex: loadableIndex,
                loadableCompleted: progress.loadableCompleted,
                numberOfLoadables: actualNumberOfLoadables));
        }
    }
}