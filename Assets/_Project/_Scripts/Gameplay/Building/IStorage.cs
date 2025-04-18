using FrontierPioneers.Gameplay.InventorySystem;

namespace FrontierPioneers.Gameplay.Building
{
    public interface IStorage
    {
        public Inventory InputInventory { get; }
        public Inventory OutputInventory { get; }
    }
}