using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace FrontierPioneers.Gameplay.InventorySystem
{
    public class Inventory
    {
        //Note: For the time being, I won't differentiate between predefined and flexible inventory. If the need arises in the future,
        //this class will work as FlexibleInventory.
        
        readonly List<InventorySlot> _inventory;
        readonly int _capacity;
        
        public event Action OnInventoryChanged;
        
        /// <summary>
        /// Creates an inventory with given capacity (slots count).
        /// </summary>
        /// <param name="capacity">
        /// Number of slots in inventory.
        /// </param>
        public Inventory(int capacity)
        {
            _capacity = capacity;
            _inventory = new List<InventorySlot>(capacity);
            for (int i = 0; i < capacity; i++)
            {
                _inventory.Add(new InventorySlot());
            }
            
            OnInventoryChanged += SortInventory;
        }

        ~Inventory()
        {
            OnInventoryChanged -= SortInventory;
        }
        
        /// <summary>
        /// Adds given amount of item to the inventory.
        /// It either adds specified amount, or adds 0 if it can't add all of it.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="count">The amount of item to add.</param>
        /// <returns>True if given amount of item was added successfully; otherwise, false.</returns>
        public bool AddItem(ItemSO item, int count)
        {
            if(item == null)
            {
                Debug.LogError("Trying to add null item to inventory.");
                return false;
            }
            if(count <= 0)
            {
                Debug.LogError($"Trying to add non-positive ({count}) of {item} to inventory.");
                return false;
            }
            
            if(!CanAddItem(item, count))
            {
                Debug.Log($"Can't add {count} of {item} to inventory, it has only {GetItemCapacity(item)} space left.");
                return false;
            }
            
            SortInventory();
            
            foreach(var slot in _inventory)
            {
                if(slot.Item == item)
                {
                    int spaceLeft = item.stackSize - slot.Quantity;
                    if(spaceLeft >= count)
                    {
                        slot.Quantity += count;
                        break;
                    }
                    else
                    {
                        slot.Quantity += spaceLeft;
                        count -= spaceLeft;
                    }
                }
                else if(slot.Item == null)
                {
                    slot.Item = item;
                    if(count <= item.stackSize)
                    {
                        slot.Quantity += count;
                        break;
                    }
                    else
                    {
                        slot.Quantity += item.stackSize;
                        count -= item.stackSize;
                    }
                }
            }
            
            OnInventoryChanged?.Invoke();
            return true;
        }


        /// <summary>
        /// Removes given amount of item from the inventory.
        /// It either removes specified amount, or removes 0 if it can't remove all of it.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <param name="count">The amount of item to remove.</param>
        /// <returns>True if given amount of item was removed successfully; otherwise, false.</returns>
        public bool RemoveItem(ItemSO item, int count)
        {
            if(item == null)
            {
                Debug.LogError("Trying to remove null item from inventory.");
                return false;
            }
            if(count <= 0)
            {
                Debug.LogError($"Trying to remove non-positive ({count}) of {item} to inventory.");
                return false;
            }
            
            if(!HasItem(item, count))
            {
                Debug.Log($"Can't remove {count} of {item} from inventory, it has only {GetItemCount(item)}.");
                return false;
            }

            SortInventory();

            for(int i = _inventory.Count - 1; i >= 0; i--)
            {
                var slot = _inventory[i];
                if(slot.Item == item)
                {
                    int itemAmount = slot.Quantity;
                    if(itemAmount > count)
                    {
                        slot.Quantity -= count;
                        break;
                    }
                    else if(itemAmount == count)
                    {
                        slot.Item = null;
                        slot.Quantity = 0;
                        break;
                    }
                    else
                    {
                        count -= itemAmount;
                        slot.Item = null;
                        slot.Quantity = 0;
                    }
                }
            }

            OnInventoryChanged?.Invoke();
            return true;
        }

        /// <summary>
        /// Checks if inventory contains given amount of item.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <param name="count">The amount of item to check.</param>
        /// <returns>True if given amount of item exists; otherwise, false.</returns>
        public bool HasItem(ItemSO item, int count)
        {
            if(item == null)
            {
                Debug.LogError("Trying to check null item in inventory.");
                return false;
            }
            if(count <= 0)
            {
                Debug.LogError($"Trying to check non-positive ({count}) of {item} to inventory.");
                return false;
            }
            
            int amountInInventory = GetItemCount(item);
            return amountInInventory >= count;
        }

        /// <summary>
        /// Checks how much of this item can be added.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>Amount of space for this item.</returns>
        public int GetItemCapacity(ItemSO item)
        {
            if(item == null)
            {
                Debug.LogError("Trying to check null item in inventory.");
                return 0;
            }
            int capacity = 0;

            foreach(var slot in _inventory)
            {
                if(slot.Item == item)
                {
                    capacity += item.stackSize - slot.Quantity;
                }
                else if(slot.Item == null)
                {
                    capacity += item.stackSize;
                }
            }
            
            return capacity;
        }

        /// <summary>
        /// Checks if this amount of item can be added.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <param name="count">Amount to check.</param>
        /// <returns>True if given amount can be added; otherwise, false.</returns>
        public bool CanAddItem(ItemSO item, int count)
        {
            if(item == null)
            {
                Debug.LogError("Trying to check null item in inventory.");
                return false;
            }
            if(count <= 0)
            {
                Debug.LogError($"Trying to check non-positive ({count}) of {item}.");
                return false;
            }
            
            int roomInInventory = GetItemCapacity(item);
            return roomInInventory >= count;
        }

        /// <summary>
        /// Checks how much of this item can be removed.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>Amount of this item currently in inventory.</returns>
        public int GetItemCount(ItemSO item)
        {
            if(item == null)
            {
                Debug.LogError("Trying to check null item in inventory.");
                return 0;
            }
            int count = 0;
            foreach(var slot in _inventory)
            {
                if(slot.Item == item)
                {
                    count += slot.Quantity;
                }
            }
            return count;
        }

        /// <summary>
        /// Removes all of this item from inventory.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>Amount of item removed from inventory</returns>
        public int RemoveWholeItem(ItemSO item)
        {
            if(item == null)
            {
                Debug.LogError("Trying to remove whole of null item from inventory.");
                return 0;
            }
            int count = 0;

            foreach(var slot in _inventory)
            {
                if(slot.Item == item)
                {
                    count += slot.Quantity;
                    slot.Item = null;
                    slot.Quantity = 0;
                }
            }
            
            OnInventoryChanged?.Invoke();
            return count;
        }

        /// <summary>
        /// Sets all slots in inventory to null.
        /// </summary>
        public void Clear()
        {
            for(int i = 0; i < _capacity; i++)
            {
                _inventory[i].Item = null;
                _inventory[i].Quantity = 0;
            }
            
            OnInventoryChanged?.Invoke();
        }
        
        public bool IsEmpty => _inventory.All(slot => slot.Item == null);
        
        /// <summary>
        /// Returns all items in inventory.
        /// </summary>
        /// <returns>
        /// Returns a dictionary of items and their quantities.
        /// </returns>
        public Dictionary<ItemSO, int> GetItemsAsDictionary()
        {
            var dict = new Dictionary<ItemSO, int>();
            foreach(var slot in _inventory.Where(slot => slot.Item != null))
            {
                if (dict.ContainsKey(slot.Item))
                {
                    dict[slot.Item] += slot.Quantity;
                }
                else
                {
                    dict.Add(slot.Item, slot.Quantity);
                }
            }

            return dict;
        }

        /// <summary>
        /// Returns all slots from inventory.
        /// </summary>
        /// <returns>
        /// Returns list of slots.
        /// </returns>
        public List<InventorySlot> GetItemsAsList()
        {
            List<InventorySlot> slots = new List<InventorySlot>(_capacity);
            for(int i = 0; i < _capacity; i++)
            {
                slots.Add(new InventorySlot(_inventory[i].Item, _inventory[i].Quantity));
            }
            
            return slots;
        }

        private void SortInventory()
        {
            _inventory.Sort(InventorySlot.InventorySlotComparison);
        }
    }
    
    public class InventorySlot
    {
        public ItemSO Item { get; set; }
        public int Quantity { get; set; }

        public InventorySlot()
        {
            Item = null;
            Quantity = 0;
        }
        public InventorySlot(ItemSO item, int quantity)
        {
            Item = item;
            Quantity = quantity;
        }
        public static int InventorySlotComparison(InventorySlot slot1, InventorySlot slot2)
        {
            // +1  -> slot2 first
            // 0  -> equal
            // -1 -> slot1 first
            if(slot1.Item == null && slot2.Item == null)
            {
                return 0;
            }
            else if(slot1.Item == null && slot2.Item != null)
            {
                return 1;
            }
            else if(slot1.Item.Equals(slot2.Item))
            {
                return slot2.Quantity.CompareTo(slot1.Quantity);
            }
            else
            {
                return slot1.Item.CompareTo(slot2.Item);
            }
        }
    }
}
