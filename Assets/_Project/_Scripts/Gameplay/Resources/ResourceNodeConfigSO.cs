using FrontierPioneers.Gameplay.InventorySystem;
using UnityEngine;

namespace FrontierPioneers.Gameplay.Resources
{
    /// <summary>
    /// It is used to configure the resource node.
    /// It defines the base item and the special item that can be gathered from the resource node,
    /// as well as the amount of them.
    /// </summary>
    [CreateAssetMenu(fileName = "ResourceNodeConfig", menuName = "FrontierPioneers/Resources/ResourceNodeConfig")]
    public class ResourceNodeConfigSO : ScriptableObject
    {
        [Header("Resource Node Config")]
        [Header("Basic Resource")]
        [Tooltip("Main resource that can be gathered from the resource node.")]
        public ItemSO basicResource;
        public int basicResourceAmount;
        public int basicResourceAmountPerGather;
        
        [Header("Special Resource")]
        [Tooltip("Special resource, which is gathered in lower amounts and is visually depleted.")]
        public ItemSO specialResource = null;
        public int specialResourceAmount = 0;
        public int specialResourceAmountPerGather = 0;
    }
}