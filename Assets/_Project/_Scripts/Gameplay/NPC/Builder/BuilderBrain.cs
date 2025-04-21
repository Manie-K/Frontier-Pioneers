using System;
using System.Linq;
using FrontierPioneers.Core.StateMachine;
using FrontierPioneers.Gameplay.Building;
using UnityEngine;

namespace FrontierPioneers.Gameplay.NPC.Builder
{
    [RequireComponent(typeof(BuilderController))]
    public class BuilderBrain : NPCBrain
    {
        BuilderController _controller;
        Animator _animator;
        ConstructionSiteController _currentSite;

        public event Action OnCurrentConstructionSiteChanged;
        
        void Awake()
        {
            _animator = GetComponent<Animator>();
            _controller = GetComponent<BuilderController>();
        }
        
        protected override void Start()
        {
            ConstructionSiteManager.Instance.OnConstructionSiteRegistered += CheckForNewSiteAvailable;
            ConstructionSiteManager.Instance.OnConstructionSiteUnpaused += CheckForNewSiteAvailable;
            
            ConstructionSiteManager.Instance.OnConstructionSiteDeregistered += CheckIfOurSiteIsStillAvailable;
            ConstructionSiteManager.Instance.OnConstructionSitePaused += CheckIfOurSiteIsStillAvailable;
            
            base.Start(); //Set up state machine
            _currentSite = ConstructionSiteManager.Instance.RequestConstructionSite(_controller);
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if(ConstructionSiteManager.Instance != null)
            {
                ConstructionSiteManager.Instance.OnConstructionSiteRegistered -= CheckForNewSiteAvailable;
                ConstructionSiteManager.Instance.OnConstructionSiteUnpaused -= CheckForNewSiteAvailable;
            
                ConstructionSiteManager.Instance.OnConstructionSiteDeregistered -= CheckIfOurSiteIsStillAvailable;
                ConstructionSiteManager.Instance.OnConstructionSitePaused -= CheckIfOurSiteIsStillAvailable;
            }
        }

        protected override void ConfigureStateMachine()
        {
            var idleState = new IdleState(_animator);
            var walkingToWorkplaceState = new StaticWalkState(_animator, navMeshAgent, () =>
                _controller.Workplace.transform.position);
            var walkingToBuildingState = new StaticWalkState(_animator, navMeshAgent, () =>
                _currentSite?.transform.position ?? transform.position);
            var buildingState = new BuildingState(_animator, this);
            
            //Set up transitions
            stateMachine.AddTransition(walkingToWorkplaceState, idleState, new FuncPredicate(() =>
                    _controller.IsNextToWorkplace()
                ));
            
            stateMachine.AddTransition(walkingToBuildingState, buildingState, new FuncPredicate(() =>
                    walkingToBuildingState.HasReachedDestination(2f)
                ));
            
            stateMachine.AddTransition(walkingToBuildingState, idleState,
                new EventPredicate("OnCurrentConstructionSiteChanged",this));
            
            stateMachine.AddTransition(idleState, walkingToBuildingState, new FuncPredicate(()=>
                    _currentSite != null && !_currentSite.Equals(null)
                ));
            
            stateMachine.AddTransition(idleState, walkingToWorkplaceState, new FuncPredicate(()=>
                !_controller.IsNextToWorkplace() && _currentSite == null
            ));
            
            stateMachine.AddTransition(buildingState, idleState, new EventPredicate("OnConstructionFinished", _controller));
            stateMachine.AddTransition(buildingState, idleState, new EventPredicate("OnCurrentConstructionSiteChanged", this));
            
            stateMachine.SetInitialState(idleState);
        }

        public void StartConstructionCoroutine()
        {
            float constructionTime = _currentSite.CurrentStage.buildTime;
            _controller.StartConstruction(_currentSite, constructionTime);
        }
        public void StopConstructionCoroutine() => _controller.StopConstruction();
        
        void CheckIfOurSiteIsStillAvailable(ConstructionSiteController site) //When site is paused or deregistered
        {
            if(site == _currentSite)
            {
                _currentSite = null;
                OnCurrentConstructionSiteChanged?.Invoke();
            }
        }
        
        void CheckForNewSiteAvailable(ConstructionSiteController site) //When site is registered or unpaused
        {
            if(_currentSite != null) return; //Our site is still valid
            _currentSite = ConstructionSiteManager.Instance.RequestConstructionSite(_controller);
        }
        
    }
}