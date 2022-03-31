namespace AlchemyBow.Core
{
    /// <summary>
    /// Provides the base interface for an object that can receive core loading callbacks.
    /// </summary>
    public interface ICoreLoadingCallbacksHandler
    {
        /// <summary>
        /// Called when binding and loading is completed (when the controller enters the working stage).
        /// </summary>
        void OnCoreLoadingFinished();
        /// <summary>
        /// Called when the scene change process begins.
        /// </summary>
        void OnCoreSceneChangeStarted();
    }
}
