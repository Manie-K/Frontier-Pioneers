using FrontierPioneers.Gameplay.Resources;
using UnityEditor;

namespace FrontierPioneers.Editor.Gameplay.Resources 
{
    [CustomEditor(typeof(VisualDepletion))]
    public class VisualDepletionEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Hierarchy note!" , EditorStyles.boldLabel);
            EditorGUILayout.LabelField("First child of this object ('chunks') should have mineral mesh renderers as children.");
            EditorGUILayout.LabelField("Parent of this object should be a resource node.");
        }
    }
}