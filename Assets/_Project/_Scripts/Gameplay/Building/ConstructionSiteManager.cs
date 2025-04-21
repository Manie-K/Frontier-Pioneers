using System;
using System.Collections.Generic;
using FrontierPioneers.Gameplay.NPC.Builder;
using MG_Utilities;
using UnityEngine;

namespace FrontierPioneers.Gameplay.Building
{
    public class ConstructionSiteManager : Singleton<ConstructionSiteManager>
    {
        [SerializeField] Transform buildingsParent;
        public Transform BuildingsParent => buildingsParent;

        readonly List<ConstructionSiteController> _availableSites = new(); //Already have required items in stockpiles
        readonly List<ConstructionSiteController> _reservedSites = new();
        readonly List<ConstructionSiteController> _pausedSites = new();
        
        public event Action<ConstructionSiteController> OnConstructionSiteRegistered; 
        public event Action<ConstructionSiteController>  OnConstructionSiteDeregistered; 
        public event Action<ConstructionSiteController>  OnConstructionSiteReserved; 
        public event Action<ConstructionSiteController>  OnConstructionSitePaused; 
        public event Action<ConstructionSiteController>  OnConstructionSiteUnpaused; 
        
        public void RegisterConstructionSite(ConstructionSiteController constructionSite)
        {
            if(!_availableSites.Contains(constructionSite))
            {
                _availableSites.Add(constructionSite);
                OnConstructionSiteRegistered?.Invoke(constructionSite);
            }
        }
        public void DeregisterConstructionSite(ConstructionSiteController constructionSite)
        {
            if(_availableSites.Contains(constructionSite))
            {
                _availableSites.Remove(constructionSite);
                OnConstructionSiteDeregistered?.Invoke(constructionSite);
            }
        }

        public void PauseConstructionSite(ConstructionSiteController constructionSite)
        {
            if(_availableSites.Contains(constructionSite))
            {
                _availableSites.Remove(constructionSite);
                _pausedSites.Add(constructionSite);
                OnConstructionSitePaused?.Invoke(constructionSite);
            }
            else if(_reservedSites.Contains(constructionSite))
            {
                _reservedSites.Remove(constructionSite);
                _pausedSites.Add(constructionSite);
                OnConstructionSitePaused?.Invoke(constructionSite);
            }
        }

        public void UnpauseConstructionSite(ConstructionSiteController constructionSite)
        {
            if(_pausedSites.Contains(constructionSite))
            {
                _pausedSites.Remove(constructionSite);
                _availableSites.Add(constructionSite);
                OnConstructionSiteUnpaused?.Invoke(constructionSite);
            }
        }

        public void CancelConstructionSite(ConstructionSiteController constructionSite)
        {
            DeregisterConstructionSite(constructionSite);
        }
        
        public ConstructionSiteController RequestConstructionSite(BuilderController builder)
        {
            //TODO: Add better logic for picking a construction site, using priorities or distance etc.
            if(_availableSites.Count == 0) return null;
            
            var constructionSite = _availableSites[0];
            _availableSites.RemoveAt(0);
            _reservedSites.Add(constructionSite);
            OnConstructionSiteReserved?.Invoke(constructionSite);
            return constructionSite;
        }
    }
}