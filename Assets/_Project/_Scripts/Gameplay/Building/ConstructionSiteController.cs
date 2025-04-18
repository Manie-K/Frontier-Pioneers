using System;
using System.Collections.Generic;
using System.Linq;
using FrontierPioneers.Gameplay.InventorySystem;
using UnityEngine;

namespace FrontierPioneers.Gameplay.Building
{
    public class ConstructionSiteController : MonoBehaviour
    {
        [SerializeField] BuildingController finalBuilding;
        [SerializeField] int stockpileCapacity = 10;
        [SerializeField] Mesh finalBuildingMesh;
        [SerializeField] BoxCollider finalBuildingCollider;
        [SerializeField] List<ConstructionStage> constructionStages;

        public ConstructionStage CurrentStage => constructionStages[_currentStageIndex];
        public BoxCollider FinishedBuildingCollider => finalBuildingCollider;
        public Mesh FinishedBuildingMesh => finalBuildingMesh;
        
        public static Action<ConstructionSiteController> OnConstructionStageAvailable;

        Inventory _stockpile;
        int _currentStageIndex = 0;

        void Awake()
        {
            _stockpile = new Inventory(stockpileCapacity);
        }

        void Start()
        {
            _stockpile.OnInventoryChanged += OnStockpileChanged_Delegate;
            OnStockpileChanged_Delegate(); //If there are no requirements for the first stage, we need to call it manually
        }

        void OnDestroy() => _stockpile.OnInventoryChanged -= OnStockpileChanged_Delegate;

        /// <summary>
        /// Adds an item to the stockpile. If the amount of item is not needed, it will not be added.
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <param name="amountToAdd">Amount to be added if needed</param>
        /// <returns>True if stockpile was changed, false otherwise</returns>
        public bool AddItemToStockpile(ItemSO item, int amountToAdd)
        {
            if(item == null || amountToAdd <= 0)
                return false;

            var requirement = constructionStages[_currentStageIndex].requirements
                                                                    .FirstOrDefault(r => r.item == item);
            //We can only add what is needed
            if(requirement != null && requirement.amount >= amountToAdd)
            {
                _stockpile.AddItem(item, amountToAdd);
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Deletes current visual, clears stockpile and spawns the next stage visual or completes the building.
        /// </summary>
        public void ConstructNextStage()
        {
            transform.DetachChildren();
            _stockpile.Clear();
            
            if(_currentStageIndex == constructionStages.Count - 1)
            {
                FinishConstruction();
            }
            else
            {
                Instantiate(constructionStages[_currentStageIndex].visualPrefab, transform);
            }

            _currentStageIndex++;
        }

        void FinishConstruction()
        {
            Instantiate(finalBuilding, transform.position, transform.rotation, ConstructionSiteManager.Instance.BuildingsParent);
            ConstructionSiteManager.Instance.UnregisterConstructionSite(this);
            if(Application.isEditor) DestroyImmediate(gameObject); else Destroy(gameObject);
        }
        
        void OnStockpileChanged_Delegate()
        {
            foreach(var req in constructionStages[_currentStageIndex].requirements)
            {
                if(_stockpile.GetItemCount(req.item) < req.amount)
                {
                    return;
                }
            }

            OnConstructionStageAvailable?.Invoke(this);
        }
    }
}