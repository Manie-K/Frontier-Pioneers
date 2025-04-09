using System;

namespace FrontierPioneers.Core.StateMachine
{
    public class FuncPredicate : IPredicate
    {
        readonly Func<bool> _predicate;

        /// <param name="predicate">
        /// Evaluation function
        /// </param>
        public FuncPredicate(Func<bool> predicate)
        {
            _predicate = predicate;
        }

        public bool Evaluate()
        {
            bool result = _predicate.Invoke();
            return result;
        }
    }
}