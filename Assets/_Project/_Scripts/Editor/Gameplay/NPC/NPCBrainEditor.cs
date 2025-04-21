using FrontierPioneers.Gameplay.NPC;
using UnityEditor;

namespace FrontierPioneers.Editor.Gameplay.NPC
{
    [CustomEditor(typeof(NPCBrain), true)]
    public class NPCBrainEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            NPCBrain brain = (NPCBrain)target;
            
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Current state: ", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"{brain.CurrentState?.Name ?? "invalid"}", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
        }
        
    }
}