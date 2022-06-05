using AlchemyBow.Core.States.Elements;
using System.Collections.Generic;
using System.Linq;

namespace AlchemyBow.Core.States
{
    /// <summary>
    /// A helper class used to describe the <c>StateGraph</c> nodes.
    /// </summary>
    public sealed class StateGraphComposer
    {
        private readonly List<StateGraphComposer> nodes = new List<StateGraphComposer>();
        private readonly List<StateGraphLink> links = new List<StateGraphLink>();

        /// <summary>
        /// Returns the state associated with the node if any.
        /// </summary>
        /// <returns>The state associated with the node if any.</returns>
        public IState State { get; private set; }

        /// <summary>
        /// Returns all added descriptions of the child nodes.
        /// </summary>
        /// <returns>All added descriptions of the child nodes</returns>
        public IReadOnlyList<StateGraphComposer> Nodes => nodes;

        /// <summary>
        /// Returns all added links between the child nodes.
        /// </summary>
        /// <returns>All added links between the child nodes</returns>
        public IReadOnlyList<StateGraphLink> Links => links;

        /// <summary>
        /// Creates an empty description of the <c>StateGraph</c> node.
        /// </summary>
        /// <param name="state">The state associated with the node. (<c>null</c> is supported.)</param>
        public StateGraphComposer(IState state)
        {
            State = state;
        }

        /// <summary>
        /// Adds a child node description.
        /// </summary>
        /// <param name="composer">The description of the child node.</param>
        public void AddNode(StateGraphComposer composer)
        {
            if (composer == null)
            {
                throw new System.ArgumentNullException(nameof(composer));
            }
            if (nodes.Contains(composer))
            {
                throw new System.ArgumentException(nameof(composer), "The child was already added.");
            }
            nodes.Add(composer);
        }

        /// <summary>
        /// Adds a link (transition) between the specified child nodes.
        /// </summary>
        /// <param name="from">The origin child node.</param>
        /// <param name="to">The destination child node.</param>
        /// <param name="condition">The transition condition.</param>
        public void AddLink(StateGraphComposer from, StateGraphComposer to, ICondition condition)
        {
            AddLink(nodes.IndexOf(from), nodes.IndexOf(to), condition);
        }

        private void AddLink(int from, int to, ICondition condition)
        {
            int numberOfNodes = nodes.Count;
            if (from < 0 || from >= numberOfNodes)
            {
                throw new System.ArgumentException(nameof(from), "The child node wasn't added.");
            }
            if (to < 0 || to >= numberOfNodes)
            {
                throw new System.ArgumentException(nameof(to), "The child node wasn't added.");
            }
            if (condition == null)
            {
                throw new System.ArgumentNullException(nameof(condition));
            }
            if (links.Find(l => l.from == from && l.to == to && l.condition == condition) != null)
            {
                throw new System.Exception("The link was already added.");
            }
            links.Add(new StateGraphLink(from, to, condition));
        }

        /// <summary>
        /// Throws exceptions if the graph node description is invalid.
        /// </summary>
        /// <remarks>This method is useful for debugging. However, consider using the <c>#if UNITY_EDITOR</c> directive to avoid performance issues in the build.</remarks>
        public void Validate()
        {
            ForeachGraphPath(pathSegments =>
            {
                var uniqueStates = new HashSet<IState>();
                var uniqueConditions = new HashSet<ICondition>();
                foreach (var pathSegment in pathSegments)
                {
                    if (pathSegment.State != null && !uniqueStates.Add(pathSegment.State))
                    {
                        throw new System.Exception($"The same state is used more than once in the graph path. (Path: {FormatPath(pathSegments)})");
                    }
                    var levelConditions = new HashSet<ICondition>(pathSegment.links.Select(l => l.condition));
                    foreach (var levelCondition in levelConditions)
                    {
                        if (!uniqueConditions.Add(levelCondition))
                        {
                            throw new System.Exception($"The same condition ({levelCondition}) is used on multiple levels of the graph path. (Path: {FormatPath(pathSegments)})");
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Returns all possible paths from root to leaves. Uses <c>.ToString()</c> on the <c>State</c> property value to determine path segment names.
        /// </summary>
        /// <returns>All possible paths from root to leaves.</returns>
        /// <remarks>This method is useful for debugging. However, consider using the <c>#if UNITY_EDITOR</c> directive to avoid performance issues in the build.</remarks>
        public List<string> GetGraphPaths()
        {
            var paths = new List<string>();
            ForeachGraphPath(pathSegments => paths.Add(FormatPath(pathSegments)));
            return paths;
        }

        private void ForeachGraphPath(System.Action<List<StateGraphComposer>> action)
        {
            foreach (var node in nodes)
            {
                node.ForeachGraphPath(new List<StateGraphComposer>() { this }, action);
            }
        }

        private void ForeachGraphPath(List<StateGraphComposer> parents, System.Action<List<StateGraphComposer>> action)
        {
            if (parents.Contains(this))
            {
                throw new System.Exception($"The graph path contains a loop. ({FormatPath(parents, this)})");
            }
            var content = new List<StateGraphComposer>(parents) { this };
            if (nodes.Count == 0)
            {
                action.Invoke(content);
            }
            else
            {
                foreach (var node in nodes)
                {
                    node.ForeachGraphPath(content, action);
                }
            }
        }

        private static string FormatPath(List<StateGraphComposer> pathSegments)
        {
            string path = "";
            int lastSegmentIndex = pathSegments.Count - 1;
            for (int i = 0; i < lastSegmentIndex; i++)
            {
                path += (pathSegments[i].State == null ? "null" : pathSegments[i].State.ToString()) + " / ";
            }
            path += pathSegments[lastSegmentIndex].State == null ? "null" : pathSegments[lastSegmentIndex].State.ToString();
            return path;
        }

        private static string FormatPath(List<StateGraphComposer> pathSegments, StateGraphComposer lastElement)
        {
            string path = "";
            foreach (var pathElement in pathSegments)
            {
                path += (pathElement.State == null ? "null" : pathElement.State.ToString()) + " / ";
            }
            path += lastElement.State == null ? "null" : lastElement.State.ToString();
            return path;
        }
    }
}