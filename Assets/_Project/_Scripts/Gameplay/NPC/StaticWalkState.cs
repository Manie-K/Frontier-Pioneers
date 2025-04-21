using System;
using FrontierPioneers.Gameplay.NPC.Gatherer;
using UnityEngine;
using UnityEngine.AI;

namespace FrontierPioneers.Gameplay.NPC
{
    public class StaticWalkState : BaseState
    {
        const string AnimationName = "Walk";

        readonly Func<Vector3> _getTargetPositionFunc;
        readonly NavMeshAgent _navMeshAgent;
        
        public StaticWalkState(Animator animator, NavMeshAgent navAgent, Func<Vector3> getPositionFunc) : base("WalkingState", animator)
        {
            _getTargetPositionFunc = getPositionFunc;
            _navMeshAgent = navAgent;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            PlayAnimation(Animator.StringToHash(AnimationName));
            _navMeshAgent.SetDestination(_getTargetPositionFunc());
        }
        
        public bool HasReachedDestination(float distance) => _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance
                                && !_navMeshAgent.pathPending && 
                                Vector3.Distance(_navMeshAgent.gameObject.transform.position, _getTargetPositionFunc()) <= distance;
    }
}