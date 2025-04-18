using System.Collections.Generic;
using MG_Utilities;
using UnityEngine;

namespace FrontierPioneers.Gameplay.Building
{
    public class ConstructionSiteManager : Singleton<ConstructionSiteManager>
    {
        [SerializeField] Transform buildingsParent;
        public Transform BuildingsParent => buildingsParent;
        
        readonly List<ConstructionSiteController> _constructionSites = new();
        
        public void RegisterConstructionSite(ConstructionSiteController constructionSite)
        {
            if (!_constructionSites.Contains(constructionSite))
            {
                _constructionSites.Add(constructionSite);
            }
        }
        
        public void UnregisterConstructionSite(ConstructionSiteController constructionSite)
        {
            if (_constructionSites.Contains(constructionSite))
            {
                _constructionSites.Remove(constructionSite);
            }
        }
    }
}