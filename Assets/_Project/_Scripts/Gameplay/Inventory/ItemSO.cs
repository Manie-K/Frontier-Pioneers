using System;
using UnityEngine;

namespace FrontierPioneers.Gameplay.InventorySystem
{
    /// <summary>
    /// It defines an item in the game. It contains the item's name, sprite, prefab, and inventory stack size.
    /// </summary>
    [CreateAssetMenu(fileName = "New Item", menuName = "FrontierPioneers/Item")]
    public class ItemSO : ScriptableObject, IComparable<ItemSO>
    {
        public new string name = "New Item";
        public Sprite sprite = null;
        public GameObject prefab = null;
        public int stackSize = 1;

        public int CompareTo(ItemSO other)
        {
            if(ReferenceEquals(this, other)) return 0;
            if(other is null) return -1;
            return String.Compare(name, other.name, StringComparison.Ordinal);
        }
        
        public override string ToString()
        {
            return name;
        }
    }
}
