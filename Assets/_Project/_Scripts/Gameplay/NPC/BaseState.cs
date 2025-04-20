using System;
using FrontierPioneers.Core.StateMachine;
using UnityEngine;

namespace FrontierPioneers.Gameplay.NPC
{
    public abstract class BaseState : IState
    { 
        protected Animator animator;
        public string Name { get; }
        public string Id { get; }

        protected BaseState(string name, Animator animator)
        {
            Name = name;
            Id = Guid.NewGuid().ToString();
            this.animator = animator;
        }
        
        public virtual void OnEnter()
        {
            //noop
        }
        

        public virtual void OnUpdate()
        {
            //noop
        }

        public virtual void OnFixedUpdate()
        {
            //noop
        }

        public virtual void OnExit()
        {
            //noop
        }
        
        protected void PlayAnimation(int animationHash)
        {
            if (animator != null)
            {
                animator.Play(animationHash);
            }
            else
            {
                Debug.LogWarning("Animator is null, cannot play animation.");
            }
        }
    }
}