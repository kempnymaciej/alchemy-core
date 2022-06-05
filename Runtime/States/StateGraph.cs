using AlchemyBow.Core.States.Elements;
using System.Linq;
using UnityEngine;

namespace AlchemyBow.Core.States
{
    /// <summary>
    /// Represents a nested directed graph designed to work with states (HFSM - Hierarchical Finite State Machine).
    /// </summary>
    public sealed class StateGraph
    {
        private const int NonStateIndex = -1;

        private readonly IState state;
        private readonly StateGraph[] nodes;
        private readonly StateGraphLink[] links;

        private bool entered;
        private bool transition;

        private int activeNodeIndex = NonStateIndex;

        private StateGraph(IState state, StateGraph[] nodes, StateGraphLink[] links)
        {
            this.state = state;
            this.nodes = nodes;
            this.links = links;
        }

        /// <summary>
        /// Builds a node of the state graph.
        /// </summary>
        /// <param name="composer">The description of the node.</param>
        /// <returns>The built node.</returns>
        /// <remarks>The composer is not automatically validated.</remarks>
        public static StateGraph Build(StateGraphComposer composer)
        {
            return new StateGraph(
                state: composer.State,
                nodes: composer.Nodes.Select(child => Build(child)).ToArray(),
                links: composer.Links.ToArray());
        }

        /// <summary>
        /// Enters the state.
        /// </summary>
        public void Enter()
        {
            if (entered)
            {
                Debug.LogWarning("You are trying to enter the state that is already entered. Ignoring...");
            }
            else if (transition)
            {
                Debug.LogWarning("You are trying to enter the state that is transitioning. Ignoring...");
            }
            else
            {
                transition = true;
                entered = true;
                state?.Enter();
                if (nodes.Length == 0)
                {
                    ChangeActiveNodeIndex(NonStateIndex);
                }
                else
                {
                    ChangeActiveNodeIndex(0);
                    nodes[activeNodeIndex].Enter();
                }
                transition = false;
                CheckActiveConditions();
            }
        }

        /// <summary>
        /// Exits the state.
        /// </summary>
        public void Exit()
        {
            if (!entered)
            {
                Debug.LogWarning("You are trying to exit the state that is not entered. Ignoring...");
            }
            else if (transition)
            {
                Debug.LogWarning("You are trying to exit the state that is transitioning. Ignoring...");
            }
            else
            {
                transition = true;
                entered = false;
                if (activeNodeIndex != NonStateIndex)
                {
                    nodes[activeNodeIndex].Exit();
                    ChangeActiveNodeIndex(NonStateIndex);
                }
                state?.Exit();
                transition = false;
            }
        }

        private void CheckActiveConditions()
        {
            if (entered && !transition)
            {
                foreach (var link in links)
                {
                    if (link.from == activeNodeIndex && link.condition.CheckCondition())
                    {
                        nodes[activeNodeIndex].Exit();
                        ChangeActiveNodeIndex(link.to);
                        nodes[activeNodeIndex].Enter();
                        return;
                    }
                }
            }
        }

        private void ChangeActiveNodeIndex(int to)
        {
            if (activeNodeIndex != NonStateIndex)
            {
                foreach (var link in links)
                {
                    if (link.from == activeNodeIndex)
                    {
                        link.condition.Triggered -= CheckActiveConditions;
                        link.condition.SetActive(false);
                    }
                }
            }
            activeNodeIndex = to;
            if (activeNodeIndex != NonStateIndex)
            {
                foreach (var link in links)
                {
                    if (link.from == activeNodeIndex)
                    {
                        link.condition.Triggered += CheckActiveConditions;
                        link.condition.SetActive(true);
                    }
                }
            }
        }

        /// <summary>
        /// Invokes the specified action on the active (nested) states (starting from the root).
        /// </summary>
        /// <param name="action">The action to invoke on the states.</param>
        /// <param name="ignoreNulls">Only invokes the action on the non-null states if <c>true</c>.</param>
        public void EnumerateDown(System.Action<IState> action, bool ignoreNulls)
        {
            if (state != null || !ignoreNulls)
            {
                action.Invoke(state);
            }
            if (activeNodeIndex != NonStateIndex)
            {
                nodes[activeNodeIndex].EnumerateDown(action, ignoreNulls);
            }
        }
    } 
}