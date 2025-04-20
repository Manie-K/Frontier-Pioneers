using FrontierPioneers.Gameplay.InventorySystem;
using UnityEngine;

namespace FrontierPioneers.Gameplay.NPC
{
    public abstract class NPCController : MonoBehaviour
    {
        public Inventory Inventory { get; protected set; }
    }
}