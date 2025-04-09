using System;
using FrontierPioneers.Core.Helpers;
using FrontierPioneers.Gameplay.InventorySystem;
using FrontierPioneers.Gameplay.NPC.Gatherer;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace FrontierPioneers.Gameplay.Resources
{
    public class ResourceNode : MonoBehaviour, IGatherable
    {
        [Space(1)]
        [SerializeField] ResourceNodeConfigSO resourceNodeConfig;       
        
        // Basic resource
        [Space(5)]
        [Header("Basic Resource")]
        [Space(2)]
        [SerializeField][ReadOnly]ItemSO basicResource;
        
        [SerializeField][ReadOnly]int basicResourceStartingAmount;
        [SerializeField][ReadOnly]int basicResourceAmountPerGather;
        [SerializeField][ReadOnly]int basicResourceAmountLeft;
        
        // Special resource
        [Space(5)]
        [Header("Special Resource")]
        [Space(2)]
        [SerializeField][ReadOnly]ItemSO specialResource;
        
        [SerializeField][ReadOnly]int specialResourceStartingAmount;
        [SerializeField][ReadOnly]int specialResourceAmountPerGather;
        [SerializeField][ReadOnly]int specialResourceAmountLeft;

        public event Action<float> OnSpecialResourceCollected;

        bool _toBeDeleted = false;
        void Awake()
        {
            Assert.IsNotNull(resourceNodeConfig, "Resource node doesn't have a config.");
            
            basicResource = resourceNodeConfig.basicResource;
            specialResource = resourceNodeConfig.specialResource;
            
            basicResourceAmountPerGather = resourceNodeConfig.basicResourceAmountPerGather;
            specialResourceAmountPerGather = resourceNodeConfig.specialResourceAmountPerGather;
            
            basicResourceAmountLeft = basicResourceStartingAmount = resourceNodeConfig.basicResourceAmount;
            specialResourceAmountLeft = specialResourceStartingAmount = resourceNodeConfig.specialResourceAmount;
            
            Assert.IsNotNull(basicResource, "Basic resource wasn't set.");
        }
        
        public void Gather(GathererController gatherer)
        {
            if(_toBeDeleted) return;
            if (gatherer == null)
            {
                throw new NullReferenceException("Gatherer is null");
            }
            
            Inventory gathererInventory = gatherer.Inventory;
            if(gathererInventory == null)
            {
                throw new NullReferenceException("Gatherer inventory is null");
            }
            
            if (basicResourceAmountLeft > 0)
            {
                int amountToGather = basicResourceAmountPerGather * gatherer.BasicMiningEfficiency;
                amountToGather = Math.Min(amountToGather, basicResourceAmountLeft); //Resource node restriction
                amountToGather = Math.Min(amountToGather, gathererInventory.GetItemCapacity(basicResource)); //Gatherer restriction
                
                basicResourceAmountLeft -= amountToGather;
                bool added = gathererInventory.AddItem(basicResource, amountToGather); 
                if(!added)
                {
                    throw new ArgumentException($"Couldn't add {amountToGather} of {basicResource} to gatherer's inventory}}");
                }
            }

            if(specialResource != null && specialResourceAmountLeft > 0)
            {
                int amountToGather = specialResourceAmountPerGather * gatherer.SpecialMiningEfficiency;
                amountToGather = Math.Min(amountToGather, specialResourceAmountLeft); //Resource node restriction
                amountToGather = Math.Min(amountToGather, gathererInventory.GetItemCapacity(specialResource)); //Gatherer restriction            
                
                specialResourceAmountLeft -= amountToGather;
                bool added = gathererInventory.AddItem(specialResource, amountToGather); 
                if(!added)
                {
                    throw new ArgumentException($"Couldn't add {amountToGather} of {specialResource} to gatherer's inventory}}");
                }
                
                float currentSpecialResourcePercentage = specialResourceAmountLeft / (float)specialResourceStartingAmount;
                OnSpecialResourceCollected?.Invoke(currentSpecialResourcePercentage);
            }
            
            if(basicResourceAmountLeft <= 0 && (specialResourceAmountLeft <= 0 || specialResource == null))
            {
                DepleteNode();
            }
        }

        void DepleteNode()
        {
            _toBeDeleted = true;
            Debug.Log("This node is depleted");
            //TODO: Play particle system
            //TODO: Remove it from the world
        }
    }
}
