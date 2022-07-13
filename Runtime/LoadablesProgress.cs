namespace AlchemyBow.Core
{
    /// <summary>
    /// Represents the progress of the (loadables) loading process.
    /// </summary>
    public sealed class LoadablesProgress
    {
        /// <summary>
        /// The current loadable.
        /// </summary>
        public readonly ICoreLoadable loadable;
        /// <summary>
        /// The index of the current loadable.
        /// </summary>
        public readonly int loadableIndex;
        /// <summary>
        /// The status of the current loadable (<c>true</c> if complete).
        /// </summary>
        public readonly bool loadableCompleted;
        /// <summary>
        /// The total number of loadables that are involved in the loading process.
        /// </summary>
        public readonly int numberOfLoadables;

        /// <summary>
        /// Creates an instance of the class.
        /// </summary>
        /// <param name="loadable">The current loadable.</param>
        /// <param name="loadableIndex">The index of the current loadable.</param>
        /// <param name="loadableCompleted">The status of the current loadable (<c>true</c> if complete).</param>
        /// <param name="numberOfLoadables">The total number of loadables that are involved in the loading process.</param>
        public LoadablesProgress(ICoreLoadable loadable, int loadableIndex, bool loadableCompleted, int numberOfLoadables)
        {
            this.loadable = loadable;
            this.loadableIndex = loadableIndex;
            this.loadableCompleted = loadableCompleted;
            this.numberOfLoadables = numberOfLoadables;
        }
    } 
}
