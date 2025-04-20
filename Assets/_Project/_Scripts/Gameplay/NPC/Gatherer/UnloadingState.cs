using UnityEngine;

namespace FrontierPioneers.Gameplay.NPC.Gatherer
{
    public class UnloadingState : BaseState
    {
        const string AnimationName = "Unload";
        
        readonly GathererBrain _brain;
        
        public UnloadingState(Animator animator, GathererBrain brain) : base("UnloadingState", animator)
        {
            _brain = brain;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            PlayAnimation(Animator.StringToHash(AnimationName));
            _brain.StartUnloadingCoroutine();
        }
    }
}