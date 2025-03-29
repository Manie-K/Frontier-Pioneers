using UnityEngine;
using System.Collections.Generic;
using FrontierPioneers.Gameplay.InventorySystem;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class InventoryTests 
    {
        private Inventory _inventory;
        private ItemSO _goldItem;
        private ItemSO _appleItem;
        private ItemSO _swordItem;
        
        [SetUp]
        public void Setup()
        {
            _goldItem = ScriptableObject.CreateInstance<ItemSO>();
            _goldItem.name = "Gold";
            _goldItem.stackSize = 5;
            
            _appleItem = ScriptableObject.CreateInstance<ItemSO>();
            _appleItem.name = "Apple";
            _appleItem.stackSize = 10;
            
            _swordItem = ScriptableObject.CreateInstance<ItemSO>();
            _swordItem.name = "Sword";
            _swordItem.stackSize = 1;
            
            _inventory = new Inventory(5);
        }

        //TODO: Tests for GetItemCount, GetItemCapacity, AddItem with capacity, RemoveItem with count etc.
        
        [Test]
        public void AddItem_AddsItemCorrectly()
        {
            bool result = _inventory.AddItem(_goldItem, 3);
            Assert.IsTrue(result);
            Assert.AreEqual(_inventory.GetItemCount(_goldItem), 3);
        }

        [Test]
        public void RemoveItem_RemovesItemCorrectly()
        {
            _inventory.AddItem(_goldItem, 5);
            bool result = _inventory.RemoveItem(_goldItem, 3);
            Assert.IsTrue(result);
            Assert.AreEqual(_inventory.GetItemCount(_goldItem), 2);
        }

        [Test]
        public void RemoveItem_FailsToRemoveMoreThanAvailable()
        {
            _inventory.AddItem(_goldItem, 2);
            bool result = _inventory.RemoveItem(_goldItem, 3);
            Assert.IsFalse(result);
            Assert.AreEqual(_inventory.GetItemCount(_goldItem), 2);
        }

        [Test]
        public void HasItem_ReturnsTrueIfEnoughOfItemExists()
        {
            _inventory.AddItem(_goldItem, 8);
            bool result = _inventory.HasItem(_goldItem, 7);
            Assert.IsTrue(result);
        }

        [Test]
        public void HasItem_ReturnsFalseIfNotEnoughOfItemExists()
        {
            _inventory.AddItem(_goldItem, 1);
            bool result = _inventory.HasItem(_goldItem, 3);
            Assert.IsFalse(result);
        }
        
        [Test]
        public void HasItem_ReturnsFalseIfItemDoesNotExist()
        {
            bool result = _inventory.HasItem(_goldItem, 3);
            Assert.IsFalse(result);
        }
        
        [Test]
        public void GetItemCount_ReturnsCorrectCount()
        {
            _inventory.AddItem(_goldItem, 6);
            _inventory.AddItem(_goldItem, 4);
            int count = _inventory.GetItemCount(_goldItem);
            Assert.AreEqual(count, 10);
        }

        [Test]
        public void RemoveWholeItem_RemovesAllOfOnlyThisItem()
        {
            _inventory.AddItem(_goldItem, 13);
            _inventory.AddItem(_appleItem, 2);
            int removedCount = _inventory.RemoveWholeItem(_goldItem);
            Assert.AreEqual(removedCount, 13);
            Assert.AreEqual(_inventory.GetItemCount(_goldItem), 0);
            Assert.AreEqual(_inventory.GetItemCount(_appleItem), 2);
        }
        
        [Test]
        public void GetItemsAsDictionary_NonEmptyInventory()
        {
            _inventory.AddItem(_goldItem, 5);
            _inventory.AddItem(_appleItem, 8);
            _inventory.AddItem(_swordItem, 3);

            Dictionary<ItemSO, int> inventoryDictionary = _inventory.GetItemsAsDictionary();
            Assert.AreEqual(inventoryDictionary.Count, 3);
            Assert.AreEqual(inventoryDictionary[_goldItem], 5);
            Assert.AreEqual(inventoryDictionary[_appleItem], 8);
            Assert.AreEqual(inventoryDictionary[_swordItem], 3);
        }
        
        [Test]
        public void GetItemsAsDictionary_EmptyInventory()
        {
            Dictionary<ItemSO, int> inventoryDictionary = _inventory.GetItemsAsDictionary();
            Assert.AreEqual(inventoryDictionary.Count, 0);
            Assert.AreEqual(inventoryDictionary, new Dictionary<ItemSO, int>());
        }

        [Test]
        public void GetItemsAsList_NonEmptyInventory()
        {
            _inventory.AddItem(_goldItem, 25);
            
            List<InventorySlot> inventorySlots = _inventory.GetItemsAsList();
            foreach(var slot in inventorySlots)
            {
                Assert.AreEqual(slot.Item, _goldItem);
                Assert.AreEqual(slot.Quantity, 5);
            }
        }
        
        [Test]
        public void GetItemsAsList_EmptyInventory()
        {
            List<InventorySlot> inventorySlots = _inventory.GetItemsAsList();
            foreach(var slot in inventorySlots)
            {
                Assert.AreEqual(slot.Item, null);
                Assert.AreEqual(slot.Quantity, 0);
            }
        }
        
        [Test]
        public void SortInventory_NullShouldBeLast()
        {
            _inventory.AddItem(_goldItem, 1);
            _inventory.AddItem(_appleItem, 1);
            _inventory.AddItem(_swordItem, 1);

            List<InventorySlot> sortedInventory = _inventory.GetItemsAsList();
            Assert.AreNotEqual(sortedInventory[0].Item, null, "1st item should not be null");
            Assert.AreNotEqual(sortedInventory[1].Item, null, "2nd item should not be null");
            Assert.AreNotEqual(sortedInventory[2].Item, null, "3rd item should not be null");
            Assert.AreEqual(sortedInventory[3].Item, null, "4th item should be null");
            Assert.AreEqual(sortedInventory[4].Item, null, "5th item should be null");
        }

        [Test]
        public void SortInventory_SameItemSOHigherQuantityShouldBeFirst()
        {
            _inventory.AddItem(_goldItem, 8); //Stack size is 5
            
            List<InventorySlot> sortedInventory = _inventory.GetItemsAsList();
            Assert.AreEqual(sortedInventory[0].Item, _goldItem);
            Assert.AreEqual(sortedInventory[0].Quantity, 5);
            Assert.AreEqual(sortedInventory[1].Item, _goldItem);
            Assert.AreEqual(sortedInventory[1].Quantity, 3);
        }
        
        [Test]
        public void SortInventory_SameItemSOShouldBeNextToEachOther()
        {
            _inventory.AddItem(_goldItem, 9); //Stack size is 5
            _inventory.AddItem(_appleItem, 7); //Apple should be first, because Apple is alphabetically first than Gold
            
            List<InventorySlot> sortedInventory = _inventory.GetItemsAsList();
            Assert.AreEqual(sortedInventory[0].Item, _appleItem);
            Assert.AreEqual(sortedInventory[0].Quantity, 7);
            Assert.AreEqual(sortedInventory[1].Item, _goldItem);
            Assert.AreEqual(sortedInventory[1].Quantity, 5);
            Assert.AreEqual(sortedInventory[2].Item, _goldItem);
            Assert.AreEqual(sortedInventory[2].Quantity, 4);
        }
    }
}
