using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace FrontierPioneers.Core.StateMachine
{
    /// <summary>
    /// Implementation of a state machine. It should be created in the MonoBehaviour as a POCO.
    /// </summary>
    public class StateMachine
    {
        /// <summary>
        /// Holds information about a <see cref="IState"/>  state and its outgoing <see cref="ITransition"/> transitions.
        /// </summary>
        class StateNode
        {
            public IState State { get; }
            public HashSet<ITransition> Transitions { get; }

            public StateNode(IState state)
            {
                State = state;
                Transitions = new();
            }
            public void AddTransition(ITransition transition)
            {
                Transitions.Add(transition);
            }
        }
        
        readonly Dictionary<string, StateNode> _stateNodes = new();
        readonly HashSet<ITransition> _anyTransitions = new(); //Transitions which are always available (from all other states)
                                                               //to a given state
        
        StateNode _currentStateNode;
        
        /// <summary>
        /// The current <see cref="IState"/> (nullable) state of the state machine.
        /// </summary>
        [CanBeNull]
        public IState CurrentState => _currentStateNode?.State;
        
        /// <summary>
        /// Needs to be called every Update.
        /// Transitions are checked and the state is changed if needed.
        /// </summary>
        public void OnUpdate()
        {
            ITransition transition = GetAvailableTransition();
            if (transition != null)
                ChangeState(transition.To);
            else
                _currentStateNode?.State?.OnUpdate();
        }
        
        /// <summary>
        /// Needs to be called every FixedUpdate.
        /// </summary>
        public void OnFixedUpdate()
        {
            _currentStateNode?.State?.OnFixedUpdate();
        }

        /// <summary>
        /// Adds a transition to the given state, regardless of initial state.
        /// <param name="to"> <see cref="IState"/> Target state </param>
        /// <param name="condition"> <see cref="IPredicate"/> Condition that needs to be true </param>
        /// </summary>
        public void AddAnyTransition(IState to, IPredicate condition)
        {
            if (to == null || condition == null)
            {
                throw new ArgumentNullException($"State and condition cannot be null. Target state: {to}, " +
                                                $"Condition: {condition}");
            }
            GetOrAddStateNode(to);
            _anyTransitions.Add(new SimpleTransition(to, condition));
        }
        
        /// <summary>
        /// Adds a transition to the given state, from initial state.
        /// <param name="from"> <see cref="IState"/> Initial state </param>
        /// <param name="to"> <see cref="IState"/> Target state </param>
        /// <param name="condition"> <see cref="IPredicate"/> Condition that needs to be true </param>
        /// </summary>
        public void AddTransition(IState from, IState to, IPredicate condition)
        {
            if (from == null || to == null || condition == null)
            {
                throw new ArgumentNullException($"States and condition cannot be null. " +
                                                $"Initial state: {from}, Target state: {to}, Condition: {condition}");
            }
            GetOrAddStateNode(to);
            GetOrAddStateNode(from).AddTransition(new SimpleTransition(to, condition));
        }

        /// <summary>
        /// Sets initial state of the state machine.
        /// Logs error when state machine already has an initial state.
        /// </summary>
        public void SetInitialState(IState state)
        {
            if(_currentStateNode != null)
            {
                Debug.LogError($"State machine already has an initial state: {_currentStateNode.State.Name}");
                return;
            }

            if(state == null)
            {
                throw new NullReferenceException($"State cannot be null.");
            }
            
            ChangeState(state);
        }
        
        StateNode GetOrAddStateNode(IState state)
        {
            StateNode foundStateNode = _stateNodes.GetValueOrDefault(state.Id);
            if (foundStateNode == null)
            {
                foundStateNode = new StateNode(state);
                _stateNodes.Add(state.Id, foundStateNode);
            }

            return foundStateNode;
        }
        ITransition GetAvailableTransition()
        {
            foreach(var transition in _anyTransitions)
            {
                if (transition.Condition.Evaluate())
                    return transition;
            }

            foreach (var transition in _currentStateNode.Transitions)
            {
                if (transition.Condition.Evaluate())
                    return transition;
            }

            return null;
        }
        void ChangeState(IState newState)
        {
            if (_currentStateNode?.State == newState)
                return;

            _currentStateNode?.State?.OnExit();
            _currentStateNode = _stateNodes[newState.Id];
            _currentStateNode?.State?.OnEnter();
        }
    }
}