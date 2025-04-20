using FrontierPioneers.Gameplay.InventorySystem;
using UnityEngine;

namespace FrontierPioneers.Gameplay.Building
{
    /// <summary>
    /// This is <see cref="IStorage"/> storage that has a single <see cref="Inventory"/> inventory for both input and output.
    /// </summary>
    public class UnifiedStorage : MonoBehaviour, IStorage
    {
        [SerializeField] int inventoryCapacity = 20;
        public Inventory InputInventory => _inv;
        public Inventory OutputInventory => _inv;

        private Inventory _inv;
        void Awake()
        {
            _inv = new Inventory(inventoryCapacity);
        }
    }
}