using System;
using System.Collections;
using System.Collections.Generic;
using FrontierPioneers.Gameplay.InventorySystem;
using FrontierPioneers.Gameplay.Resources;
using UnityEngine;

namespace FrontierPioneers.Gameplay.NPC.Gatherer
{
    public class GathererController : NPCController
    {
        [SerializeField] private int inventoryCapacity = 10;
        [SerializeField] private float gatheringInterval = 4f;
        [SerializeField] private int gatheringIntervalsPerSession = 5;
        public int BasicMiningEfficiency { get; private set; } = 1;
        public int SpecialMiningEfficiency { get; private set; } = 1;

        public event Action OnMiningFinished;
        
        Coroutine _gatherCoroutine;
        
        
        void Awake()
        {
            Inventory = new Inventory(inventoryCapacity);
        }

        /// <summary>
        /// Checks if the gatherer has enough space in its inventory to gather the resources.
        /// </summary>
        /// <param name="gatherable">Defines which resource are checked.</param>
        public bool HasInventorySpace(IGatherable gatherable) => gatherable.CanGather(this);
        
        /// <summary>
        /// ! To only be called from the GathererBrain !
        /// </summary>
        /// <param name="gatherable">Gatherable to gather.</param>
        public void StartGathering(IGatherable gatherable)
        {   
            if(_gatherCoroutine != null) return;
            _gatherCoroutine = StartCoroutine(GatherResourceCoroutine(gatherable));
        }
        
        /// <summary>
        /// ! To only be called from the GathererBrain !
        /// </summary>
        public void StopGathering()
        {
            if(_gatherCoroutine == null) return;
            StopCoroutine(_gatherCoroutine);
            _gatherCoroutine = null;
        }
        
        IEnumerator GatherResourceCoroutine(IGatherable currentGatherable)
        {
            int gathersCount = 0;
            while(gathersCount < gatheringIntervalsPerSession)
            {
                gathersCount++;
                currentGatherable.Gather(this);
                yield return new WaitForSeconds(gatheringInterval);
            }
            OnMiningFinished?.Invoke();
        }
    }
}