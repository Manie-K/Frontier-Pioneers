using FrontierPioneers.Gameplay.InventorySystem;
using UnityEngine;

namespace FrontierPioneers.Gameplay.Building
{
    /// <summary>
    /// This is <see cref="IStorage"/> storage that has two separate <see cref="Inventory"/> inventories, one for input and one for output.
    /// </summary>
    public class SeparatedStorage : MonoBehaviour, IStorage
    {
        [SerializeField] int inputInventoryCapacity = 10;
        [SerializeField] int outputInventoryCapacity = 10;
        
        public Inventory InputInventory { get; private set; }
        public Inventory OutputInventory { get; private set; }

        void Awake()
        {
            InputInventory = new Inventory(inputInventoryCapacity);
            OutputInventory = new Inventory(outputInventoryCapacity);
        }
    }
}