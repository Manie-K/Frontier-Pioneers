using FrontierPioneers.Gameplay.Building;
using UnityEditor;
using UnityEngine;

namespace FrontierPioneers.Editor.Gameplay.Building
{
    [CustomEditor(typeof(ConstructionSiteController))]
    public class ConstructionSiteControllerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Separator();
            
            ConstructionSiteController script = (ConstructionSiteController)target;
            
            if(GUILayout.Button("Fill stockpile"))
            {
                script.Debug_FillStockpile();
            }
            if(GUILayout.Button("Construct next stage"))
            {
                script.ConstructNextStage();
            }
            if(GUILayout.Button("Construct final building"))
            {
                script.ConstructFinalBuildingInstantly();
            }
            EditorGUILayout.Separator();
        }
    }
}