namespace AlchemyBow.Core
{
    /// <summary>
    /// Provides the base interface for the abstraction of states.
    /// </summary>
    public interface ICoreState
    {
        /// <summary>
        /// Activates the object.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Deactivates the object.
        /// </summary>
        void Deinitialize();
    } 
}
