namespace AlchemyBow.Core.States
{
    /// <summary>
    /// Provides the base interface for the abstraction of states.
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// Activates the state.
        /// </summary>
        void Enter();

        /// <summary>
        /// Deactivates the state.
        /// </summary>
        void Exit();
    } 
}
