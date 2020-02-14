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
        private List<Phone> _phones = new List<Phone>();
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

            _phones = phones.CommonPhones;
        }

        private void OnGUI()
        {
            PrintTestPanel();
            PrintAddAllResolutionsPanel();
            PrintAllPhones();
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
                foreach (var phone in _phones)
                {
                    Resize(phone.Resolution.Width, phone.Resolution.Height, phone.Name);
                }
            }
        }

        private void PrintAllPhones()
        {
            GUILayout.Label("Devices", EditorStyles.boldLabel);

            _phones = _phones.OrderBy(phone => (float) phone.Resolution.Width / phone.Resolution.Height).ToList();

            var importantResolutions = new List<Phone>()
                {_phones[0], _phones[_phones.Count / 2], _phones[_phones.Count - 1]};
            
            DisplayImportantResolution("Smaller", importantResolutions[0]);
            DisplayImportantResolution("Medium", importantResolutions[1]);
            DisplayImportantResolution("Smaller", importantResolutions[2]);
            

            GUILayout.Label("Others", EditorStyles.boldLabel);
            var otherResolutions = new List<Phone>();
            otherResolutions.AddRange(_phones.Except(importantResolutions));
            otherResolutions.ForEach(phone => DisplayButton(phone.Name, phone.Resolution.Width, phone.Resolution.Height, phone.Tooltip));
        }

        private void DisplayImportantResolution(string text, Phone phone1)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(text);
            DisplayButton(phone1);
            GUILayout.EndHorizontal();
        }

        private void DisplayButton(Phone phone)
        {
            DisplayButton(phone.Name, phone.Resolution.Width, phone.Resolution.Height, phone.Tooltip);
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