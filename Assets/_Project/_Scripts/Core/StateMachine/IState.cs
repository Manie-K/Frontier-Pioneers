using UnityEngine;

namespace FrontierPioneers.Core.StateMachine
{
    /// <summary>
    /// Interface for a state in a state machine.
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// It is used for debug/editor purposes.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// It is used for hash.
        /// </summary>
        string Id { get; }
        
        /// <summary>
        /// Called when this state is entered.
        /// </summary>
        public void OnEnter();
        
        /// <summary>
        /// Called when the state machine is updated, and this is current state.
        /// </summary>
        public void OnUpdate();
        
        /// <summary>
        /// Called when the state machine is fixed updated, and this is current state.
        /// </summary>
        public void OnFixedUpdate();
        
        /// <summary>
        /// Called when this state is exited.
        /// </summary>
        public void OnExit();
    }
}
