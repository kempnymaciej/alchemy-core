namespace AlchemyBow.Core.Extras.Behaviours
{
    /// <summary>
    /// Specifies the strategy for enabling and disabling <c>CoreBehaviours</c>.
    /// </summary>
    public enum CoreBehaviourEnablingStrategy
    {
        /// <summary>
        /// A <c>CoreBehaviour</c> is enabled when its <c>Enabled</c> property is set to <c>true</c> and disabled when set to <c>false</c>.
        /// </summary>
        Simple,

        /// <summary>
        /// Similar to <c>Simple</c>, but the <c>CoreBehaviour</c> is automatically enabled when <c>ICoreLoadingCallbacksHandler.OnCoreLoadingFinished</c> is called.
        /// </summary>
        SimpleAutoEnable,

        /// <summary>
        /// Similar to <c>Simple</c>, but the <c>CoreBehaviour</c> is automatically disabled when <c>ICoreLoadingCallbacksHandler.OnCoreSceneChangeStarted</c> is called.
        /// </summary>
        SimpleAutoDisable,

        /// <summary>
        /// Similar to <c>Simple</c>, but the <c>CoreBehaviour</c> is automatically enabled when <c>ICoreLoadingCallbacksHandler.OnCoreSceneChangeFinished</c> is called and disabled when <c>ICoreLoadingCallbacksHandler.OnCoreSceneChangeStarted</c> is called.
        /// </summary>
        SimpleAutoEnableDisable,

        /// <summary>
        /// A <c>CoreBehaviour</c> is enabled when its <c>Enabled</c> property is set to <c>true</c> a positive number of times and disabled when set to <c>false</c> an equal or greater number of times.
        /// </summary>
        Cumulative,
    }
}