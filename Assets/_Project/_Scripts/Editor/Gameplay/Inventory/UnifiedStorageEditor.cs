using System.Collections.Generic;
using System.Linq;
using FrontierPioneers.Gameplay.Building;
using FrontierPioneers.Gameplay.InventorySystem;
using UnityEditor;
using UnityEngine;

namespace FrontierPioneers.Editor.Gameplay.Inventory
{
    [CustomEditor(typeof(UnifiedStorage))]
    public class UnifiedStorageEditor : UnityEditor.Editor
    {
        private bool _showInventory = true;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            UnifiedStorage storage = (UnifiedStorage)target;
            List<InventorySlot> inventoryList = storage.InputInventory?.GetItemsAsList(); //The same as output inventory
            
            if(inventoryList == null)
            {
                EditorGUILayout.LabelField("Inventory not initialized.");
                return;
                
            }
            
            EditorGUILayout.Space(3);
            _showInventory = EditorGUILayout.Foldout(_showInventory, "Inventory", true);
            if(_showInventory)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.Space(1);
                EditorGUILayout.LabelField("Content", EditorStyles.boldLabel);

                if (inventoryList.Count(s => s.Item == null) == inventoryList.Count)
                {
                    EditorGUILayout.LabelField("Empty");
                }
                else
                {
                    foreach (var slot in inventoryList)
                    {
                        if(slot.Item == null)
                            break;
                    
                        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.Width(180));
                        EditorGUILayout.LabelField(slot.Item.ToString(),GUILayout.Width(80), GUILayout.Height(36));
                    
                        if(slot.Item.sprite != null)
                        {
                            GUILayout.Label(AssetPreview.GetAssetPreview(slot.Item.sprite), 
                                GUILayout.Width(40),
                                GUILayout.Height(36));
                        }
                    
                        EditorGUILayout.LabelField(slot.Quantity.ToString(), GUILayout.Width(50), GUILayout.Height(36));
                        EditorGUILayout.EndHorizontal();
                    }
                }

                EditorGUI.indentLevel--;
            }
                
        }
    }
}