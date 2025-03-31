using FrontierPioneers.Gameplay.InventorySystem;
using UnityEngine;

namespace FrontierPioneers.Gameplay.Resources
{
    public class Gatherer : MonoBehaviour
    {
        public int BasicMiningEfficiency { get; private set; } = 1;
        public int SpecialMiningEfficiency { get; private set; } = 1;
        public Inventory Inventory { get; private set; }
    }
}