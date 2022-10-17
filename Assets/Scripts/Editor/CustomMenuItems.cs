using UnityEditor;

// ReSharper disable once CheckNamespace
namespace RollABall.Editor
{
    public static class CustomMenuItems 
    {
        [MenuItem("Tools/Recoloring")]
        private static void MenuOptions()
        {
            EditorWindow.GetWindow(typeof(RecoloringWindow), true, "Recoloring Window");
        }
    }
}


