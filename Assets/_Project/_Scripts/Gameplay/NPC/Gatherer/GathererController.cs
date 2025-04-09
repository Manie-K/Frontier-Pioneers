using FrontierPioneers.Gameplay.InventorySystem;
using UnityEngine;

namespace FrontierPioneers.Gameplay.NPC.Gatherer
{
    public class GathererController : MonoBehaviour
    {
        public int BasicMiningEfficiency { get; private set; } = 1;
        public int SpecialMiningEfficiency { get; private set; } = 1;
        public Inventory Inventory { get; private set; }   
    }
}