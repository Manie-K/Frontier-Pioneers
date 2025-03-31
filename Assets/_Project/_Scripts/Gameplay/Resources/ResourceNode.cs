using System;
using FrontierPioneers.Gameplay.InventorySystem;
using UnityEngine;
using UnityEngine.Assertions;

namespace FrontierPioneers.Gameplay.Resources
{
    public class ResourceNode : MonoBehaviour, IGatherable
    {
        [SerializeField] ResourceNodeConfigSO resourceNodeConfig;       

        // Basic resource
        ItemSO _basicResource;
        int _basicResourceStartingAmount;
        int _basicResourceAmountPerGather;
        int _basicResourceAmountLeft;
        
        // Special resource
        ItemSO _specialResource;
        int _specialResourceStartingAmount;
        int _specialResourceAmountPerGather;
        int _specialResourceAmountLeft;

        public event Action<float> OnSpecialResourceCollected;

        bool _toBeDeleted = false;
        void Awake()
        {
            Assert.IsNotNull(resourceNodeConfig, "Resource node doesn't have a config.");
            
            _basicResource = resourceNodeConfig.basicResource;
            _specialResource = resourceNodeConfig.specialResource;
            
            _basicResourceStartingAmount = resourceNodeConfig.basicResourceAmountPerGather;
            _specialResourceStartingAmount = resourceNodeConfig.specialResourceAmountPerGather;
            
            _basicResourceAmountLeft = _basicResourceStartingAmount = resourceNodeConfig.basicResourceAmount;
            _specialResourceAmountLeft = _specialResourceStartingAmount = resourceNodeConfig.specialResourceAmount;
            
            Assert.IsNotNull(_basicResource, "Basic resource wasn't set.");
        }
        
        public void Gather(Gatherer gatherer)
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
            
            if (_basicResourceAmountLeft > 0)
            {
                int amountToGather = _basicResourceAmountPerGather * gatherer.BasicMiningEfficiency;
                amountToGather = Math.Min(amountToGather, _basicResourceAmountLeft); //Resource node restriction
                amountToGather = Math.Min(amountToGather, gathererInventory.GetItemCapacity(_basicResource)); //Gatherer restriction
                
                _basicResourceAmountLeft -= amountToGather;
                bool added = gathererInventory.AddItem(_basicResource, amountToGather); 
                if(!added)
                {
                    throw new ArgumentException($"Couldn't add {amountToGather} of {_basicResource} to gatherer's inventory}}");
                }
            }

            if(_specialResource != null && _specialResourceAmountLeft > 0)
            {
                int amountToGather = _specialResourceAmountPerGather * gatherer.SpecialMiningEfficiency;
                amountToGather = Math.Min(amountToGather, _specialResourceAmountLeft); //Resource node restriction
                amountToGather = Math.Min(amountToGather, gathererInventory.GetItemCapacity(_specialResource)); //Gatherer restriction            
                
                _specialResourceAmountLeft -= amountToGather;
                bool added = gathererInventory.AddItem(_specialResource, amountToGather); 
                if(!added)
                {
                    throw new ArgumentException($"Couldn't add {amountToGather} of {_specialResource} to gatherer's inventory}}");
                }
                
                float currentSpecialResourcePercentage = _specialResourceAmountLeft / (float)_specialResourceStartingAmount;
                OnSpecialResourceCollected?.Invoke(currentSpecialResourcePercentage);
            }
            
            if(_basicResourceAmountLeft <= 0 && (_specialResourceAmountLeft <= 0 || _specialResource == null))
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
