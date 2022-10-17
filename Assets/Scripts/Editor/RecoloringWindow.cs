using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace RollABall.Editor
{
    public class RecoloringWindow : EditorWindow
    {
        private const string Header = "Recoloring";
        private Color SelectedColor { get; set; }
        
        private int SelectedObjectsCount { get; set; }
        
        [ExecuteAlways]
        private void OnGUI()
        {
            GUILayout.Label(Header, EditorStyles.boldLabel);
            GUILayout.Label("Select objects, choose color and press button", EditorStyles.label);
            GUILayout.Label($"Selected: {SelectedObjectsCount}", EditorStyles.label);
            
            SelectedColor = EditorGUILayout.ColorField("Choose color", SelectedColor);

            SelectedObjectsCount = Selection.gameObjects.Length;
            
            if (GUILayout.Button("Change colors"))
            {
                ChangeColors();
            }
        }
        
        private void ChangeColors()
        {
            if (!Selection.activeGameObject) return;
            
            foreach (var go in Selection.gameObjects)
            {
                var renderer = go.GetComponent<Renderer>();

                if (renderer != null)
                    renderer.sharedMaterial.color = SelectedColor;
            }
        }
    }
}