using System.Collections.Generic;
using System.Linq;
using Domain;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [ExecuteInEditMode]
    public class TestResolutionEditorWindow : EditorWindow
    {
        private Dictionary<string, List<Phone>> _phonesByCompany = new Dictionary<string, List<Phone>>();

        [MenuItem("Etermax/Test Resolution")]
        private static void Init()
        {
            GetWindow(typeof(TestResolutionEditorWindow), false, "Resolutions").Show();
        }

        [MenuItem("Etermax/Test 2")]
        private static void Init2()
        {
            var resolutionsJson = Resources.Load<TextAsset>("resolutions");
            UnityEngine.Debug.LogError(resolutionsJson);
            var phones = JsonUtility.FromJson<Phones>(resolutionsJson.text);

            var phonebycompany = phones.CommonPhones
                .GroupBy(phone => phone.Company)
                .ToDictionary(g => g.Key, g => g.ToList());

            UnityEngine.Debug.LogError(phonebycompany["Android"].Count);
            UnityEngine.Debug.LogError(phonebycompany["Android"][0].Name);
            UnityEngine.Debug.LogError(phonebycompany["Apple"].Count);
            UnityEngine.Debug.LogError(phonebycompany["Apple"][0].Name);
        }

        private void OnEnable()
        {
            LoadJson();
        }

        private void LoadJson()
        {
            var resolutionsJson = Resources.Load<TextAsset>("resolutions");
            var phones = JsonUtility.FromJson<Phones>(resolutionsJson.text);

            _phonesByCompany = phones.CommonPhones
                .GroupBy(phone => phone.Company)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        private void OnGUI()
        {
            foreach (var key in _phonesByCompany.Keys)
            {
                PrintAllCompanyPhones(key, _phonesByCompany[key]);
            }

            /*GUILayout.Label("Apple", EditorStyles.boldLabel);
            GUILayout.Label("Phones");
            PrintAllCompanyPhones("Apple");
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
            GUILayout.Label("Tablet");*/
        }

        private void PrintAllCompanyPhones(string company, List<Phone> phones)
        {
            GUILayout.Label(company, EditorStyles.boldLabel);
            GUILayout.Label("Phones");
            phones.ForEach(phone => DisplayButton(phone.Name, phone.Resolution.Width, phone.Resolution.Height, phone.Tooltip));
        }

        private void DisplayButton(string text, int width, int height, string tooltip)
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