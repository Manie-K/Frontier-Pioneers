using System.Collections.Generic;
using System.Linq;
using FrontierPioneers.Gameplay.Building;
using FrontierPioneers.Gameplay.InventorySystem;
using UnityEditor;
using UnityEngine;

namespace FrontierPioneers.Editor.Gameplay.Inventory
{
    [CustomEditor(typeof(SeparatedStorage))]
    public class SeparatedStorageEditor : UnityEditor.Editor
    {
        private bool _showInputInventory = true;
        private bool _showOutputInventory = true;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            SeparatedStorage storage = (SeparatedStorage)target;
            List<InventorySlot> inputInvList = storage.InputInventory?.GetItemsAsList(); 
            List<InventorySlot> outputInvList = storage.OutputInventory?.GetItemsAsList(); 
            
            if (inputInvList == null || outputInvList == null)
            {
                EditorGUILayout.LabelField("Inventories not initialized.", EditorStyles.helpBox);
                return;
            }
            
            EditorGUILayout.Space(3);
            _showInputInventory = EditorGUILayout.Foldout(_showInputInventory, "Input Inventory", true);
            if (_showInputInventory)
            {
                DrawInventorySection(inputInvList);
            }
            
            _showOutputInventory = EditorGUILayout.Foldout(_showOutputInventory, "Output Inventory", true);
            if (_showOutputInventory)
            {
                DrawInventorySection(outputInvList);
            }
        }
        
        void DrawInventorySection(List<InventorySlot> slots)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.Space(1);
            EditorGUILayout.LabelField("Content", EditorStyles.boldLabel);

            if (slots.Count(s => s.Item == null) == slots.Count)
            {
                EditorGUILayout.LabelField("Empty");
            }
            else
            {
                foreach (var slot in slots)
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