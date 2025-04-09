namespace FrontierPioneers.Core.StateMachine
{
    /// <summary>
    /// Interface for a transition to a given <see cref="IState"/> state on <see cref="IPredicate"/> condition in a state machine.
    /// </summary>
    public interface ITransition
    {
        /// <summary>
        /// The target state.
        /// </summary>
        IState To { get; }

        /// <summary>
        /// Required condition.
        /// </summary>
        IPredicate Condition { get; }
    }
}