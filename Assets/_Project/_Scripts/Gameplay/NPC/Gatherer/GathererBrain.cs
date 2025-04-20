using System;
using System.Collections;
using FrontierPioneers.Gameplay.Building;
using FrontierPioneers.Gameplay.Resources;
using UnityEngine;

namespace FrontierPioneers.Gameplay.NPC.Gatherer
{
    [RequireComponent(typeof(GathererController))]
    public class GathererBrain : NPCBrain
    {
        private GathererController _controller;
        private Animator _animator;
        public IGatherable CurrentGatherable { get; private set; }
        public BuildingController Workplace { get; private set; }
        
        public event Action OnCurrentGatherableChanged; 
        
        Coroutine _gatheringCoroutine;
        
        protected override void Start()
        {
            base.Start();
            _animator = GetComponent<Animator>();
            _controller = GetComponent<GathererController>();
            CurrentGatherable = null; //Initialize with WorldResourceManager
            Workplace = null; //Initialize when gatherer is "born"
        
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
        }

        void CheckCurrentGatherable()
        {
            if(WorldResourcesManager.Instance?.SelectedResources?.Contains(CurrentGatherable) == false)
            {//our current gatherable isn't available anymore
                //TODO: Pick new current gatherable, raise event to change state
                
                OnCurrentGatherableChanged?.Invoke();
                CurrentGatherable = WorldResourcesManager.Instance.SelectedResources[0]; //TODO: Pick new current gatherable based on position

            } //else our gatherable is still selected and valid
        }

        public void StartGatheringCoroutine()
        {
            if(CurrentGatherable == null)
            {
                Debug.LogError("Current gatherable is null");
                return;
            }

            if(_gatheringCoroutine != null)
            {
                Debug.LogError("Gatherer has a gathering coroutine running already");
                return;
            }

            _controller.StartGathering(CurrentGatherable);
        }

        public void StopGatheringCoroutine()
        {
            _controller.StopGathering();
        }        
    }
}