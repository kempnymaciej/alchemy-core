namespace AlchemyBow.Core.States.Elements
{
    /// <summary>
    /// Represents a transition between the <c>StateGraph</c> nodes.
    /// </summary>
    public sealed class StateGraphLink
    {
        /// <summary>
        /// The index of the origin node.
        /// </summary>
        /// <returns>The index of the origin node.</returns>
        public readonly int from;
        /// <summary>
        /// The index of the destination node.
        /// </summary>
        /// <returns>The index of the destination node.</returns>
        public readonly int to;
        /// <summary>
        /// The condition of the transition.
        /// </summary>
        /// <returns>The condition of the transition.</returns>
        public readonly ICondition condition;

        /// <summary>
        /// Creates an instance of the class.
        /// </summary>
        /// <param name="from">The index of the origin node.</param>
        /// <param name="to">The index of the destination node.</param>
        /// <param name="condition">The condition of the transition.</param>
        public StateGraphLink(int from, int to, ICondition condition)
        {
            this.from = from;
            this.to = to;
            this.condition = condition;
        }
    } 
}
