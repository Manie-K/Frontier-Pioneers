using System;
using System.Collections;
using FrontierPioneers.Gameplay.Building;
using FrontierPioneers.Gameplay.InventorySystem;
using UnityEngine;

namespace FrontierPioneers.Gameplay.NPC.Builder
{
    public class BuilderController : NPCController
    {
        [SerializeField] int inventoryCapacity = 10;
        [SerializeField] BuildingController workplace;
        [SerializeField] ParticleSystem constructionParticles;

        public event Action OnConstructionFinished; 
        
        Coroutine _constructionCoroutine;
        
        void Awake()
        {
            Inventory = new Inventory(inventoryCapacity);
            Workplace = workplace;
        }

        /// <summary>
        /// ! To only be called from the BuilderBrain !
        /// </summary>
        public void StartConstruction(ConstructionSiteController site, float constructionTime)
        {
            if(_constructionCoroutine != null) 
                Debug.LogError("Already started construction");
            else if(site == null) 
                Debug.LogError("Construction site which was to be build is null");
            else 
                _constructionCoroutine = StartCoroutine(ConstructionCoroutine(site, constructionTime));
        }

        /// <summary>
        /// ! To only be called from the BuilderBrain !
        /// </summary>
        public void StopConstruction()
        {
            if(_constructionCoroutine != null)
                StopCoroutine(_constructionCoroutine);
        }

        IEnumerator ConstructionCoroutine(ConstructionSiteController site, float constructionTime)
        {
            constructionParticles?.Play();
            yield return new WaitForSeconds(constructionTime);
            constructionParticles?.Stop();
            site.ConstructNextStage();
            OnConstructionFinished?.Invoke();
        }
    }
}
