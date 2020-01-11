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