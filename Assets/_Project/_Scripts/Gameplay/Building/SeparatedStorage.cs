using FrontierPioneers.Gameplay.InventorySystem;
using UnityEngine;

namespace FrontierPioneers.Gameplay.Building
{
    public class SeparatedStorage : MonoBehaviour, IStorage
    {
        [SerializeField] int inputInventoryCapacity = 10;
        [SerializeField] int outputInventoryCapacity = 10;
        
        public Inventory InputInventory => _inputInventory;
        public Inventory OutputInventory => _outputInventory;
        
        Inventory _inputInventory;
        Inventory _outputInventory;
        
        void Awake()
        {
            _inputInventory = new Inventory(inputInventoryCapacity);
            _outputInventory = new Inventory(outputInventoryCapacity);
        }
    }
}