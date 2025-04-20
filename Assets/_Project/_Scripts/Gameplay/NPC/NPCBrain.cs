using System;
using FrontierPioneers.Core.StateMachine;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

namespace FrontierPioneers.Gameplay.NPC
{
    /// <summary>
    /// Abstract MonoBehaviour that handles the brain of an NPC.
    /// It should register and deregister itself with the <see cref="NPCManager"/>.
    /// </summary>
    public abstract class NPCBrain : MonoBehaviour
    {
        protected readonly StateMachine stateMachine = new();
        protected NavMeshAgent navMeshAgent;
        protected NPCController npcController;
        
        [CanBeNull] public IState CurrentState => stateMachine.CurrentState;
        protected virtual void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            if (navMeshAgent == null)
            {
                Debug.LogError($"NPCBrain: {gameObject.name} does not have a NavMeshAgent component.");
            }
            npcController = GetComponent<NPCController>();
            if (npcController == null)
            {
                Debug.LogError($"NPCBrain: {gameObject.name} does not have a NPCController component.");
            }
            
            ConfigureStateMachine();
            NPCManager.Instance?.RegisterNPC(this);
        }

        protected virtual void OnDestroy()
        {
            NPCManager.Instance?.UnregisterNPC(this);
        }
        public virtual void OnUpdate()
        {
            stateMachine.OnUpdate();    
        }

        public virtual void OnFixedUpdate()
        {
            stateMachine.OnFixedUpdate();
        }
        
        protected abstract void ConfigureStateMachine();
    }
}