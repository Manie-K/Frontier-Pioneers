using System;
using FrontierPioneers.Core.StateMachine;
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

        void Awake()
        {
            _animator = GetComponent<Animator>();
            _controller = GetComponent<GathererController>();
            _currentGatherable = null;
        }

        protected override void Start()
        {
            base.Start();
            WorldResourcesManager.Instance.OnSelectedResourcesChanged += CheckCurrentGatherable;
            CheckCurrentGatherable();
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
            var idleState = new BasicIdleState(_animator);
            var walkingToResourceState = new StaticWalkState(_animator, navMeshAgent, () =>
                ((MonoBehaviour)_currentGatherable).transform.position);
            var walkingToWorkplaceState = new StaticWalkState(_animator, navMeshAgent, () =>
                _controller.Workplace.transform.position);

            //Set up transitions
            stateMachine.AddTransition(unloadingState, idleState,
                new EventPredicate("OnUnloadingFinished", _controller));

            stateMachine.AddTransition(idleState, walkingToResourceState, new FuncPredicate(() =>
                CurrentGatherableIsValid() && _controller.HasInventorySpace(_currentGatherable)));
            stateMachine.AddTransition(walkingToResourceState, idleState, 
                new EventPredicate("OnCurrentGatherableChanged", this));
            
            stateMachine.AddTransition(gatheringState, idleState, 
                new EventPredicate("OnCurrentGatherableChanged", this));
            
            stateMachine.AddTransition(walkingToResourceState, gatheringState,
                new FuncPredicate(()=>walkingToResourceState.HasReachedDestination(3f)));

            stateMachine.AddTransition(gatheringState, walkingToWorkplaceState, new EventPredicate(
                "OnMiningFinished", _controller));
            
            stateMachine.AddTransition(gatheringState, walkingToWorkplaceState, new FuncPredicate(()=>
                !_controller.HasInventorySpace(_currentGatherable)));

            stateMachine.AddTransition(idleState, unloadingState, new FuncPredicate(() => 
                    !_controller.Inventory.IsEmpty &&
                    _controller.IsNextToWorkplace() &&
                    !CurrentGatherableIsValid()
                ));
            
            stateMachine.AddTransition(walkingToWorkplaceState, idleState, new FuncPredicate(()=>
                    _controller.IsNextToWorkplace() &&
                    _controller.Inventory.IsEmpty
                ));
            
            stateMachine.AddTransition(walkingToWorkplaceState, unloadingState, new FuncPredicate(() =>
                    _controller.IsNextToWorkplace() &&
                    !_controller.Inventory.IsEmpty
                ));
            
            stateMachine.AddTransition(idleState, walkingToWorkplaceState, new FuncPredicate(()=>
                    !_controller.IsNextToWorkplace() && !_controller.HasInventorySpace(_currentGatherable)
                ));
            
            stateMachine.AddTransition(idleState, walkingToWorkplaceState, new FuncPredicate(()=>
                    !_controller.IsNextToWorkplace() && !CurrentGatherableIsValid()
                ));
            
            stateMachine.AddTransition(walkingToWorkplaceState, unloadingState, new FuncPredicate(()=>
                    _controller.IsNextToWorkplace() && !_controller.Inventory.IsEmpty
                ));
            
            stateMachine.SetInitialState(idleState);
        }

        void CheckCurrentGatherable()
        {
            if(!CurrentGatherableIsValid() || WorldResourcesManager.Instance?.SelectedResources?.Contains(_currentGatherable) == false)
            {   //our current gatherable isn't available anymore
                OnCurrentGatherableChanged?.Invoke();
                _currentGatherable = WorldResourcesManager.Instance?.SelectedResources.Count == 0 ? null 
                    : WorldResourcesManager.Instance?.SelectedResources?[0]; 
                //TODO: Pick new current gatherable based on position
                
            }   //else our gatherable is still selected and valid
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
            if(Vector3.Distance(transform.position, _controller.Workplace.transform.position) >
               3f) // check major difference
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
        bool CurrentGatherableIsValid() => _currentGatherable != null && !_currentGatherable.Equals(null);
    }
}
