using System;
using FrontierPioneers.Core.StateMachine;
using FrontierPioneers.Gameplay.Building;
using FrontierPioneers.Gameplay.Resources;
using UnityEngine;

namespace FrontierPioneers.Gameplay.NPC.Gatherer
{
    public class GatheringState : BaseState
    {
        const string AnimationName = "Gather";
        
        readonly GathererBrain _brain;
        
        public GatheringState(Animator animator, GathererBrain brain) : base("GatheringState", animator)
        {
            _brain = brain;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            PlayAnimation(Animator.StringToHash(AnimationName));
            _brain.StartGatheringCoroutine();
        }

        public override void OnExit()
        {
            base.OnExit();
            _brain.StopGatheringCoroutine();
        }
    }
}