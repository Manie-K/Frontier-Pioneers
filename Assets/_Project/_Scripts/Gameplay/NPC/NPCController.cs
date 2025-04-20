using FrontierPioneers.Gameplay.Building;
using FrontierPioneers.Gameplay.InventorySystem;
using UnityEngine;

namespace FrontierPioneers.Gameplay.NPC
{
    public abstract class NPCController : MonoBehaviour
    {
        public BuildingController Workplace { get; protected set; }
        public Inventory Inventory { get; protected set; }
    }
}