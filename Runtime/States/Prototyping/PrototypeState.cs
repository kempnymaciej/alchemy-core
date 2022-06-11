using System;

namespace AlchemyBow.Core.States.Prototyping
{
    /// <summary>
    /// Provides a general implementation of the <c>IState</c> interface that uses actions.
    /// </summary>
    public class PrototypeState : IState
    {
        private readonly Action enterCallback;
        private readonly Action exitCallback;

        /// <summary>
        /// Creates a generic state.
        /// </summary>
        /// <param name="enterCallback">The callback to invoke on <c>Enter()</c>.</param>
        /// <param name="exitCallback">The callback to invoke on <c>Exit()</c>.</param>
        public PrototypeState(Action enterCallback, Action exitCallback)
        {
            this.enterCallback = enterCallback;
            this.exitCallback = exitCallback;
        }

        /// <summary>
        /// Activates the state.
        /// </summary>
        public void Enter()
        {
            enterCallback?.Invoke();
        }

        /// <summary>
        /// Deactivates the state.
        /// </summary>
        public void Exit()
        {
            exitCallback?.Invoke();
        }
    }
}
