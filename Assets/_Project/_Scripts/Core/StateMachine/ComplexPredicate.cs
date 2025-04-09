using System.Linq;

namespace FrontierPioneers.Core.StateMachine
{
    /// <summary>
    /// All <see cref="IPredicate"/> sub-predicates must be true for the complex predicate to be true.
    /// </summary>
    public class ComplexPredicate : IPredicate
    {
        readonly IPredicate[] _predicates;

        /// <param name="predicates">
        /// Array of predicates to evaluate.
        /// </param>
        public ComplexPredicate(IPredicate[] predicates)
        {
            _predicates = predicates;
        }

        public bool Evaluate()
        {
            return _predicates.All(predicate => predicate.Evaluate());
        }
    }
}