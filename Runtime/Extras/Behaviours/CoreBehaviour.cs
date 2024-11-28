using UnityEngine;

namespace AlchemyBow.Core.Extras.Behaviours
{
    /// <summary>
    /// A base class for objects that can be enabled or disabled, and integrate with core loading callbacks for initialization or cleanup.
    /// </summary>
    public class CoreBehaviour : ICoreLoadingCallbacksHandler
    {
        private readonly CoreBehaviourEnablingStrategy enablingStrategy;
        private int enablingBalance;

        /// <summary>
        /// Creates a new instance of the <c>CoreBehaviour</c> class.
        /// </summary>
        /// <param name="enablingStrategy">The strategy for enabling and disabling the <c>CoreBehaviour</c>.</param>
        protected CoreBehaviour(CoreBehaviourEnablingStrategy enablingStrategy)
        {
            this.enablingStrategy = enablingStrategy;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <c>CoreBehaviour</c> is enabled.
        /// Behaves according to the strategy and invokes <c>OnEnabled</c> and <c>OnDisabled</c> when the value changes.
        /// </summary>
        public bool Enabled
        {
            get => enablingBalance > 0;
            set
            {
                if (enablingStrategy == CoreBehaviourEnablingStrategy.Cumulative)
                {
                    bool previousState = Enabled;
                    enablingBalance += value ? 1 : -1;
                    bool currentState = Enabled;

                    if (enablingBalance < 0)
                    {
                        Debug.LogWarning("The enabling balance is negative.");
                    }
                    
                    if (previousState == currentState)
                    {
                        return;
                    }
                    
                    if (currentState)
                    {
                        OnEnabled();
                    }
                    else
                    {
                        OnDisabled();
                    }
                }
                else
                {
                    if (Enabled == value)
                    {
                        return;
                    }

                    if (value)
                    {
                        enablingBalance = 1;
                        OnEnabled();
                    }
                    else
                    {
                        enablingBalance = 0;
                        OnDisabled();
                    }
                }
            }
        }

        /// <summary>
        /// Called when the <c>CoreBehaviour</c> is enabled.
        /// </summary>
        protected virtual void OnEnabled()
        {
            
        }

        /// <summary>
        /// Called when the <c>CoreBehaviour</c> is disabled.
        /// </summary>
        protected virtual void OnDisabled()
        {
            
        }
        
        /// <summary>
        /// Called when the core loading is finished.
        /// </summary>
        /// <remarks>
        /// Requires the <c>CoreBehaviour</c> to be added to the dynamic list of core loading callbacks handlers.
        /// </remarks>
        protected virtual void OnCoreLoadingFinished()
        {
            
        }

        /// <summary>
        /// Called when the core scene change is started.
        /// </summary>
        /// <remarks>
        /// Requires the <c>CoreBehaviour</c> to be added to the dynamic list of core loading callbacks handlers.
        /// </remarks>
        protected virtual void OnCoreSceneChangeStarted()
        {
            
        }
        
        void ICoreLoadingCallbacksHandler.OnCoreLoadingFinished()
        {
            OnCoreLoadingFinished();
            
            if (enablingStrategy is CoreBehaviourEnablingStrategy.SimpleAutoEnable or CoreBehaviourEnablingStrategy.SimpleAutoEnableDisable)
            {
                Enabled = true;
            }
        }

        void ICoreLoadingCallbacksHandler.OnCoreSceneChangeStarted()
        {
            OnCoreSceneChangeStarted();
            
            if (enablingStrategy is CoreBehaviourEnablingStrategy.SimpleAutoDisable or CoreBehaviourEnablingStrategy.SimpleAutoEnableDisable)
            {
                Enabled = false;
            }
        }
    }
}