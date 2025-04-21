using UnityEngine;

namespace FrontierPioneers.Gameplay.NPC.Builder
{
    public class BuildingState : BaseState
    {
        const string AnimationName = "Construct";
        
        readonly BuilderBrain _brain;

        public BuildingState(Animator animator, BuilderBrain brain) : base("BuildingState", animator)
        {
            _brain = brain;
        }
        
        public override void OnEnter()
        {
            PlayAnimation(Animator.StringToHash(AnimationName));
            _brain.StartConstructionCoroutine();
        }

        public override void OnExit()
        {
            _brain.StopConstructionCoroutine();
        }
    }
}