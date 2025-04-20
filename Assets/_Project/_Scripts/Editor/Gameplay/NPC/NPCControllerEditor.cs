using System.Collections.Generic;
using System.Linq;
using FrontierPioneers.Gameplay.InventorySystem;
using FrontierPioneers.Gameplay.NPC;
using UnityEditor;
using UnityEngine;

namespace FrontierPioneers.Editor.Gameplay.NPC
{
    [CustomEditor(typeof(NPCController), true)]
    public class NPCControllerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            NPCController controller = (NPCController)target;
            var inventory = controller.Inventory;
            if (inventory == null)
            {
                EditorGUILayout.LabelField("Inventory not initialized.", EditorStyles.helpBox);
                return;
            }
            List<InventorySlot> slots = inventory.GetItemsAsList();
            
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