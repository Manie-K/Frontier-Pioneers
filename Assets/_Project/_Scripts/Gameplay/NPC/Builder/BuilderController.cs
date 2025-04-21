using System;
using System.Collections;
using FrontierPioneers.Gameplay.Building;
using FrontierPioneers.Gameplay.InventorySystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace FrontierPioneers.Gameplay.NPC.Builder
{
    public class BuilderController : NPCController
    {
        [SerializeField] int inventoryCapacity = 10;
        [SerializeField] BuildingController workplace;
        [SerializeField] ParticleSystem constructionParticlesPrefab;

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
            StopCoroutine(_constructionCoroutine);
            _constructionCoroutine = null;
        }

        IEnumerator ConstructionCoroutine(ConstructionSiteController site, float constructionTime)
        {
            var particles = Instantiate(constructionParticlesPrefab, transform);
            particles.Play();
            
            yield return new WaitForSeconds(constructionTime);
            
            particles.Stop();
            Destroy(particles);
            
            site.ConstructNextStage();
            OnConstructionFinished?.Invoke();
        }
    }
}
