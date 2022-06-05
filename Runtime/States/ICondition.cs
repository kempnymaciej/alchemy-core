using System;

namespace AlchemyBow.Core.States
{
    /// <summary>
    /// Provides the base interface for the abstraction of conditions.
    /// </summary>
    public interface ICondition
    {
        /// <summary>
        /// An event that is raised when the condition should be checked.
        /// </summary>
        /// <remarks>
        /// In most cases, it should be raised when the condition is met.
        /// </remarks>
        event Action Triggered;

        /// <summary>
        /// Activates/Deactivates the conditions. An active condition should track its status and raise the <c>Triggered</c> event when met.
        /// </summary>
        /// <param name="value">When <c>true</c> activates the conditions; otherwise, deactivates it.</param>
        /// <remarks>
        /// This method can be used to reset the internal state of a condition between uses.
        /// </remarks>
        void SetActive(bool value);

        /// <summary>
        /// Determines whether the condition is met.
        /// </summary>
        /// <returns><c>true</c> if met; Otherwise, <c>false</c>.</returns>
        bool CheckCondition();
    } 
}
