using FrontierPioneers.Gameplay.Building;
using FrontierPioneers.Gameplay.InventorySystem;
using UnityEngine;

namespace FrontierPioneers.Gameplay.NPC
{
    public abstract class NPCController : MonoBehaviour
    {
        public BuildingController Workplace { get; protected set; }
        public Inventory Inventory { get; protected set; }
        
        /// <summary>
        /// Checks if the NPC position is next to its workplace.
        /// </summary>
        public bool IsNextToWorkplace() => Vector3.Distance(transform.position, Workplace.transform.position) <= 2f;
    }
}