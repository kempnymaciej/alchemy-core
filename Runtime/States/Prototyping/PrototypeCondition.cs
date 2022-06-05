using System;

namespace AlchemyBow.Core.States.Prototyping
{
    /// <summary>
    /// Provides a general implementation of the <c>ICondition</c> interface that acts like a trigger button.
    /// </summary>
    public class PrototypeCondition : ICondition
    {
        /// <summary>
        /// An event that is raised when the condition should be checked.
        /// </summary>
        public event Action Triggered;

        private bool value;

        /// <summary>
        /// Sets the value of the condition to <c>true</c> and raises the <c>Triggered</c> event.
        /// </summary>
        public void Trigger()
        {
            value = true;
            Triggered?.Invoke();
        }

        /// <summary>
        /// Determines whether the condition is met.
        /// </summary>
        /// <returns>The value of the condition.</returns>
        public bool CheckCondition() => value;

        /// <summary>
        /// Sets the value of the condition to <c>false</c>.
        /// </summary>
        /// <param name="value">Not used in this implementation.</param>
        public void SetActive(bool value)
        {
            this.value = false;
        }
    }
}
