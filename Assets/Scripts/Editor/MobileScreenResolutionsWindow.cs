using System.Collections.Generic;
using System.Linq;
using Domain;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [ExecuteInEditMode]
    // ReSharper disable once RequiredBaseTypesIsNotInherited
    public class MobileScreenResolutionsWindow : EditorWindow
    {
        private Dictionary<string, List<Phone>> _phonesByCompany = new Dictionary<string, List<Phone>>();
        private bool _testing;
        private float _currentTime;

        [MenuItem("Etermax/Editor/Resolutions/Open window")]
        private static void Init()
        {
            GetWindow(typeof(MobileScreenResolutionsWindow), false, "Resolutions").Show();
        }

        [MenuItem("Etermax/Editor/Resolutions/Set next #n")]
        private static void NExt()
        {
            GameViewUtils.SetNext();
        }

        [MenuItem("Etermax/Editor/Resolutions/Set previous #p")]
        private static void Previous()
        {
            GameViewUtils.SetPrevious();
        }

        private void OnEnable()
        {
            _currentTime = 0;
            _testing = false;
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
            PrintTestPanel();
            PrintAddAllResolutionsPanel();
            PrintResolutionsPanel();
        }

        private void PrintTestPanel()
        {
            var guiStyle = new GUIStyle() {wordWrap = true, padding = new RectOffset(10, 10, 10, 0)};
            GUILayout.Label("Test all resolutions after finishing your prefab.", guiStyle);

            if (GUILayout.Button("Start test"))
                StartTest();

            if (GUILayout.Button("Stop test"))
                StopTest();
        }

        private void PrintAddAllResolutionsPanel()
        {
            var guiStyle = new GUIStyle() {wordWrap = true, padding = new RectOffset(10, 10, 10, 0)};
            GUILayout.Label("Add all the following resolutions to the dropdown in the game window.", guiStyle);

            if (GUILayout.Button("Add resolutions"))
            {
                foreach (var key in _phonesByCompany.Keys)
                {
                    var phones = _phonesByCompany[key];

                    phones.ForEach(phone => Resize(phone.Resolution.Width, phone.Resolution.Height, phone.Name));
                }
            }
        }

        private void PrintResolutionsPanel()
        {
            foreach (var key in _phonesByCompany.Keys)
            {
                PrintAllCompanyPhones(key, _phonesByCompany[key]);
            }
        }

        private void PrintAllCompanyPhones(string company, List<Phone> phones)
        {
            GUILayout.Label(company, EditorStyles.boldLabel);
            GUILayout.Label("Devices");
            phones.ForEach(phone =>
                DisplayButton(phone.Name, phone.Resolution.Width, phone.Resolution.Height, phone.Tooltip));
        }

        private void DisplayButton(string text, int width, int height, string tooltip)
        {
            var guiContent = new GUIContent(text + " " + width + "x" + height, tooltip);
            var guiStyle = new GUIStyle("button") {alignment = TextAnchor.MiddleLeft};

            if (GUILayout.Button(guiContent, guiStyle) && !_testing)
                Resize(width, height, text);
        }

        private void StartTest()
        {
            _testing = true;
            _currentTime = 0.0f;
        }

        private void StopTest()
        {
            _testing = false;
        }

        private void Update()
        {
            if (!_testing)
                return;

            _currentTime += Time.fixedDeltaTime;

            if (_currentTime >= 7) // is not in seconds...is almost 2 3 seconds
            {
                _currentTime = 0;
                GameViewUtils.SetNext();
            }
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