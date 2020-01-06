using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

// This page was used to check most important resolutions
// https://mediag.com/blog/popular-screen-resolutions-designing-for-all/

[ExecuteInEditMode]
public class TestResolutionEditorWindow : EditorWindow
{
    [MenuItem("Etermax/Test Resolution")]
    private static void Init()
    {
        GetWindow(typeof(TestResolutionEditorWindow)).Show();
    }

    [MenuItem("Etermax/Test")]
    private static void Test()
    {
        Assembly assembly = typeof(EditorWindow).Assembly;
        Type type = assembly.GetType("UnityEditor.GameView");
        EditorWindow v = GetWindow(type);
  
        //whatever scale you want when you click on play
        var defaultScale = (float)type.GetProperty("minScale", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(v);
  
        var areaField = type.GetField("m_ZoomArea", BindingFlags.Instance | BindingFlags.NonPublic);
        var areaObj = areaField.GetValue(v);
  
        var scaleField = areaObj.GetType().GetField("m_Scale", BindingFlags.Instance | BindingFlags.NonPublic);
        scaleField.SetValue(areaObj, new Vector2(defaultScale, defaultScale));
    }

    [MenuItem("Etermax/Debug Min Scale")]
    private static void MinScale()
    {
        Assembly assembly = typeof(EditorWindow).Assembly;
        Type type = assembly.GetType("UnityEditor.GameView");
        EditorWindow v = GetWindow(type);
  
        //whatever scale you want when you click on play
        var minScale = (float)type.GetProperty("minScale", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(v);
  
        UnityEngine.Debug.LogError("min scale: " + minScale);
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
        var index = GameViewUtilsOld.FindSize(GameViewSizeGroupType.Android, width, height);
        if (index == -1)
            GameViewUtilsOld.AddCustomSize(GameViewUtilsOld.GameViewSizeType.FixedResolution, GameViewSizeGroupType.Android,
                width, height, text);

        GameViewUtils.SetSize(GameViewUtilsOld.FindSize(GameViewSizeGroupType.Android, width, height));

       // GameViewUtils.SetMinScale3();
        //m_supposedToCheckTime = true;
    }
    
    bool m_supposedToCheckTime = false;
    float m_time = 0.0f;
     
    void Update()
    {
        if (m_supposedToCheckTime)
        {
            m_time += 0.01f;
         
            if (m_time >= .05f)
            {
                m_time = 0.0f;
                m_supposedToCheckTime = false;
                
                GameViewUtilsOld.SetMinScale2();
            }
        }
    }
}