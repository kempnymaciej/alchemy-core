namespace AlchemyBow.Core
{
    /// <summary>
    /// The class that allows you to observe the state of the operation.
    /// </summary>
    public sealed class OperationHandle
    {
        /// <summary>
        /// Is the operation marked as done?
        /// </summary>
        public bool IsDone { get; private set; }

        /// <summary>
        /// Sets <c>IsDone</c> to <c>true</c>.
        /// </summary>
        public void MarkDone() => IsDone = true;
    } 
}
