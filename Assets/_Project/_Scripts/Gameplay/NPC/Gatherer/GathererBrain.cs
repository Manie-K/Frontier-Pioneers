using System;
using FrontierPioneers.Gameplay.Resources;
using UnityEngine;

namespace FrontierPioneers.Gameplay.NPC.Gatherer
{
    [RequireComponent(typeof(GathererController))]
    public class GathererBrain : NPCBrain
    {
        GathererController _controller;
        Animator _animator;
        IGatherable _currentGatherable;
        public event Action OnCurrentGatherableChanged; 
        
        Coroutine _gatheringCoroutine;
        
        protected override void Start()
        {
            base.Start();
            _animator = GetComponent<Animator>();
            _controller = GetComponent<GathererController>();
            _currentGatherable = null; //Initialize with WorldResourceManager
        
            WorldResourcesManager.Instance.OnSelectedResourcesChanged += CheckCurrentGatherable;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if(WorldResourcesManager.Instance != null)
                WorldResourcesManager.Instance.OnSelectedResourcesChanged -= CheckCurrentGatherable;
        }

        protected override void ConfigureStateMachine()
        {
            var gatheringState = new GatheringState(_animator, this);
            var unloadingState = new UnloadingState(_animator, this);
            var idleState = new IdleState(_animator);
            var walkingToResourceState = new StaticWalkState(_animator, navMeshAgent, () => 
                ((MonoBehaviour)_currentGatherable).transform.position);
            var walkingToWorkplaceState = new StaticWalkState(_animator, navMeshAgent, () =>
                _controller.Workplace.transform.position);
            
            //Set up transitions
        }

        void CheckCurrentGatherable()
        {
            if(WorldResourcesManager.Instance?.SelectedResources?.Contains(_currentGatherable) == false)
            {//our current gatherable isn't available anymore
                //TODO: Pick new current gatherable, raise event to change state
                
                OnCurrentGatherableChanged?.Invoke();
                _currentGatherable = WorldResourcesManager.Instance.SelectedResources[0]; //TODO: Pick new current gatherable based on position

            } //else our gatherable is still selected and valid
        }

        public void StartGatheringCoroutine()
        {
            if(_currentGatherable == null)
            {
                Debug.LogError("Current gatherable is null");
                return;
            }

            if(_gatheringCoroutine != null)
            {
                Debug.LogError("Gatherer has a gathering coroutine running already");
                return;
            }

            _controller.StartGathering(_currentGatherable);
        }

        public void StopGatheringCoroutine()
        {
            _controller.StopGathering();
        }

        public void StartUnloadingCoroutine()
        {
            if(Vector3.Distance(transform.position, _controller.Workplace.transform.position) > 3f) // check major difference
            {
                Debug.LogError("Gatherer is too far from the workplace to unload");
            }
            
            if(_gatheringCoroutine != null)
            {
                Debug.LogError("Gatherer has a gathering coroutine running, can't unload");
                return;
            }
            
            if(_controller.Inventory.IsEmpty)
            {
                Debug.LogError("Gatherer has no items to unload"); 
                return;
            }
            
            _controller.StartUnloading();
        }
    }
}