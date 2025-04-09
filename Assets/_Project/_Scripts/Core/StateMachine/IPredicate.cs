namespace FrontierPioneers.Core.StateMachine
{
    /// <summary>
    /// Interface for a predicate deciding <see cref="ITransition"/> transitions in a state machine.
    /// </summary>
    public interface IPredicate
    {
        /// <summary>
        /// Evaluates the predicate.
        /// </summary>
        public bool Evaluate();
    }
}