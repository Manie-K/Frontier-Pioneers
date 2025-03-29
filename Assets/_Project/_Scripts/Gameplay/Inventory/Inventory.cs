using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FrontierPioneers.Gameplay.Inventory
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
        /// Returns all items in inventory.
        /// </summary>
        /// <returns>
        /// Returns a dictionary of items and their quantities.
        /// </returns>
        public Dictionary<ItemSO, int> GetItems()
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

        private void SortInventory() => _inventory.Sort(
            (slot1, slot2) =>
            {
                // +1  -> slot2 first
                // 0  -> equal
                // -1 -> slot1 first

                if(slot1.Item == slot2.Item)
                {
                    return slot2.Quantity - slot1.Quantity;
                }
                else
                {
                    return slot1.Item.CompareTo(slot2.Item);
                }
            });
    }
    
    public class InventorySlot
    {
        public ItemSO Item { get; set; } = null;
        public int Quantity { get; set; } = 0;
    }
}
