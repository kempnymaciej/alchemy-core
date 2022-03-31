namespace AlchemyBow.Core
{
    /// <summary>
    /// Provides the base interface for an object that contains loading logic with no specified duration.
    /// </summary>
    public interface ICoreLoadable
    {
        /// <summary>
        /// Starts the loading process.
        /// </summary>
        /// <param name="handle">The handle of the loading process.</param>
        void Load(OperationHandle handle);
    }
}
