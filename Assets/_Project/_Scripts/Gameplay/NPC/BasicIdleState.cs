using UnityEngine;

namespace FrontierPioneers.Gameplay.NPC
{
    public class BasicIdleState : BaseState
    {
        const string AnimationName = "Idle";
        
        public BasicIdleState(Animator animator) : base("IdleState", animator){}

        public override void OnEnter()
        {
            base.OnEnter();
            PlayAnimation(Animator.StringToHash(AnimationName));
        }
    }
}