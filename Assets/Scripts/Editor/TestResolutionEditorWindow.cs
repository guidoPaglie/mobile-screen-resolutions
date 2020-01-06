using UnityEditor;
using UnityEngine;

namespace Editor
{
    [ExecuteInEditMode]
    public class TestResolutionEditorWindow : EditorWindow
    {
        [MenuItem("Etermax/Test Resolution")]
        private static void Init()
        {
            GetWindow(typeof(TestResolutionEditorWindow)).Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Apple", EditorStyles.boldLabel);
            GUILayout.Label("Phones");
            DisplayButton("iPhone X", 1125, 2436, "Same resolution as iPhone XS");
            DisplayButton("iPhone XR", 828, 1792);
            DisplayButton("iPhone XS MAX", 1242, 2688);
            DisplayButton("iPhone 8", 750, 1334, "Same resolution as iPhone 7 & 6");
            DisplayButton("iPhone 8 Plus", 1080, 1920, "Same resolution as iPhone 7 & 6 Plus");
            GUILayout.Label("Tablet");
            DisplayButton("iPad", 1536, 2048, "Same resolution as iPad 3 & 4, Air 1 & 2, Mini 2 & 3");
            DisplayButton("iPad Pro", 2048, 2732);

            GUILayout.Label("Android", EditorStyles.boldLabel);
            GUILayout.Label("Phones");
            DisplayButton("S10", 1440, 3040, "Same as S10+");
            DisplayButton("S10e", 1080, 2280);
            DisplayButton("S9", 1440, 2960, "Same as S9+, S8+, S8, S7");
            DisplayButton("Pixel XL", 1440, 2560);
            GUILayout.Label("Tablet");
        }

        private void DisplayButton(string text, int width, int height, string tooltip = "")
        {
            var guiContent = new GUIContent(text + $" {width}x{height}", tooltip);
            var guiStyle = new GUIStyle("button") {alignment = TextAnchor.MiddleLeft};

            if (GUILayout.Button(guiContent, guiStyle))
                Resize(width, height, text);
        }

        private void Resize(int width, int height, string text)
        {
            // TODO Refactor to not search twice.
            var index = GameViewUtils.FindSize(GameViewSizeGroupType.Android, width, height);
            if (index == -1)
                GameViewUtils.AddCustomSize(width, height, text);

            GameViewUtils.SetSize(GameViewUtils.FindSize(GameViewSizeGroupType.Android, width, height));
        }
    }
}