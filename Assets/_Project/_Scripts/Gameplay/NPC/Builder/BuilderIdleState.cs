using UnityEngine;

namespace FrontierPioneers.Gameplay.NPC.Builder
{
    public class BuilderIdleState : BaseState
    {
        const string AnimationName = "Idle";
        readonly BuilderBrain _brain;
        readonly int _constructionSitesFetchingFrequency;

        int _frameCounter = 0;
        public BuilderIdleState(Animator animator, BuilderBrain brain, int constructionSitesFetchingFrequency) 
            : base("BuilderIdleState", animator)
        {
            _brain = brain;
            _constructionSitesFetchingFrequency = constructionSitesFetchingFrequency;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            PlayAnimation(Animator.StringToHash(AnimationName));
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            _frameCounter++;
            if(_frameCounter >= _constructionSitesFetchingFrequency)
            {
                _frameCounter = 0;
                _brain.FetchAvailableConstructionSites();
            }
        }

        public override void OnExit()
        {
            _frameCounter = 0;
        }
    }
}