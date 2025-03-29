using FrontierPioneers.Gameplay.InventorySystem;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    [TestFixture]
    public class ItemSOTests
    {
        [Test]
        public void CompareTo_SameItem_ReturnsZero()
        {
            var item1 = ScriptableObject.CreateInstance<ItemSO>();
            item1.name = "Test Item";
            var item2 = item1;

            int result = item1.CompareTo(item2);

            Assert.AreEqual(0, result);
        }
        
        [Test]
        public void CompareTo_NullItem_ReturnsNegativeOne()
        {
            var item1 = ScriptableObject.CreateInstance<ItemSO>();
            item1.name = "Test Item";
            ItemSO item2 = null;

            int result = item1.CompareTo(item2);

            Assert.AreEqual(-1, result);
        }
        
        [Test]
        public void CompareTo_FurtherAlphabeticalItem_ReturnsNegative()
        {
            var item1 = ScriptableObject.CreateInstance<ItemSO>();
            item1.name = "Apple";
            
            var item2 = ScriptableObject.CreateInstance<ItemSO>();
            item2.name = "Banana";

            int result = item1.CompareTo(item2);

            Assert.Less(result, 0);
        }

        [Test]
        public void CompareTo_EarlierAlphabeticalItem_ReturnsPositive()
        {
            var item1 = ScriptableObject.CreateInstance<ItemSO>();
            item1.name = "Banana";

            var item2 = ScriptableObject.CreateInstance<ItemSO>();
            item2.name = "Apple";

            int result = item1.CompareTo(item2);

            Assert.Greater(result, 0);
        }
    }
}
