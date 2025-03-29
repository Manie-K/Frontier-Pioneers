using System;
using UnityEngine;

namespace FrontierPioneers.Gameplay.Inventory
{
    [CreateAssetMenu(fileName = "New Item", menuName = "FrontierPioneers/Item")]
    public class ItemSO : ScriptableObject, IComparable<ItemSO>
    {
        public new string name;
        public Sprite sprite;
        public GameObject prefab;
        public int stackSize = 1;

        public int CompareTo(ItemSO other)
        {
            if(ReferenceEquals(this, other)) return 0;
            if(other is null) return -1;
            return -1 * String.Compare(name, other.name, StringComparison.Ordinal);
        }
    }
}
