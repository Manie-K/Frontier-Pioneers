using System;
using System.Collections;
using System.Collections.Generic;
using FrontierPioneers.Gameplay.Building;
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
        //Temporary 
        [SerializeField] private BuildingController workplace;
        public int BasicMiningEfficiency { get; private set; } = 1;
        public int SpecialMiningEfficiency { get; private set; } = 1;

        public event Action OnMiningFinished;
        public event Action OnUnloadingFinished;
        
        Coroutine _gatherCoroutine;
        
        
        void Awake()
        {
            Inventory = new Inventory(inventoryCapacity);
            Workplace = workplace;
        }
        
        /// <summary>
        /// Checks if the gatherer has enough space in its inventory to gather the resources.
        /// If the gatherable is null, it will return true.
        /// </summary>
        /// <param name="gatherable">Defines which resource are checked, nullable.</param>
        public bool HasInventorySpace(IGatherable gatherable) => gatherable?.CanGather(this) ?? true;
        
        /// <summary>
        /// ! To only be called from the GathererBrain !
        /// </summary>
        public void StartUnloading() => StartCoroutine(UnloadCoroutine());
        
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

        IEnumerator UnloadCoroutine()
        {
            int tries = 0;
            while(!Inventory.IsEmpty)
            {
                var slot = Inventory.GetItemsAsList()[0];
                ItemSO item = slot.Item;
                int quantity = slot.Quantity;
                
                Workplace.Storage.InputInventory.AddItem(item, quantity);
                Inventory.RemoveItem(item, quantity);
                
                tries++;
                yield return new WaitForSeconds(1f);

                if(tries >= 100)
                {
                    Debug.LogError($"Unloading for {tries} attempts, is everything okay?");
                }
            }
            OnUnloadingFinished?.Invoke();
        }
    }
}