using System;
using System.Collections.Generic;
using FrontierPioneers.Gameplay.InventorySystem;
using UnityEngine;

namespace FrontierPioneers.Gameplay.Building
{
    [Serializable]
    public class ConstructionStage
    {
        [Serializable] public class ConstructionStageRequirement
        {
            ConstructionStageRequirement(){}
            public ItemSO item;
            public int amount;
        }
        
        public GameObject visualPrefab;
        public float buildTime;
        public List<ConstructionStageRequirement> requirements;
    }
}