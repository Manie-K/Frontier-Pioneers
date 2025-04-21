using UnityEngine;

namespace FrontierPioneers.Gameplay.NPC
{
    public class IdleState : BaseState
    {
        const string AnimationName = "Idle";
        
        public IdleState(Animator animator) : base("IdleState", animator){}

        public override void OnEnter()
        {
            base.OnEnter();
            PlayAnimation(Animator.StringToHash(AnimationName));
        }
    }
}