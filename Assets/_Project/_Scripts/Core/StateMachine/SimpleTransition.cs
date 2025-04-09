namespace FrontierPioneers.Core.StateMachine
{
    public class SimpleTransition : ITransition
    {
        public IState To { get; }
        public IPredicate Condition { get; }
        
        public SimpleTransition(IState to, IPredicate condition)
        {
            To = to;
            Condition = condition;
        }
    }
}