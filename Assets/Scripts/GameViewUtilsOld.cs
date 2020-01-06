using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

//  sizesType.GetMethod("SaveToHDD").Invoke(gameViewSizesInstance, null);

public static class GameViewUtilsOld
{
    static object gameViewSizesInstance;
    static MethodInfo getGroup;

    static GameViewUtilsOld()
    {
        // gameViewSizesInstance  = ScriptableSingleton<GameViewSizes>.instance;
        var sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
        var singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        var instanceProp = singleType.GetProperty("instance");
        getGroup = sizesType.GetMethod("GetGroup");
        gameViewSizesInstance = instanceProp.GetValue(null, null);
    }

    public enum GameViewSizeType
    {
        AspectRatio,
        FixedResolution
    }

    public static void SetSelectedSize(int index)
    {
        var gvWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
        var selectedSizeIndexProp = gvWndType.GetProperty("selectedSizeIndex",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        var gvWnd = EditorWindow.GetWindow(gvWndType);
        selectedSizeIndexProp.SetValue(gvWnd, index, null);

        var gameCanvases = GameObject.FindObjectsOfType<Canvas>();
        foreach (var gameCanvas in gameCanvases)
        {
            EditorUtility.SetDirty(gameCanvas);
        }
        
        var SizeSelectionCallback = gvWndType.GetMethod("SizeSelectionCallback",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        SizeSelectionCallback.Invoke(gvWnd, new object[] {index,null});
    }

    //Works after x secs
    public static void SetMinScale()
    {
        Assembly assembly = typeof(EditorWindow).Assembly;
        Type type = assembly.GetType("UnityEditor.GameView");
        EditorWindow v = EditorWindow.GetWindow(type);

        //whatever scale you want when you click on play
        var minScale = (float) type.GetProperty("minScale", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(v);

        var areaField = type.GetField("m_ZoomArea", BindingFlags.Instance | BindingFlags.NonPublic);
        var areaObj = areaField.GetValue(v);

        var scaleField = areaObj.GetType().GetField("m_Scale", BindingFlags.Instance | BindingFlags.NonPublic);
        scaleField.SetValue(areaObj, new Vector2(minScale, minScale));
    }

    // This one also works but after cliking in window and after x secs
    public static void SetMinScale2()
    {
        var gvWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");

        UnityEngine.Object[] objectsOfTypeAll = UnityEngine.Resources.FindObjectsOfTypeAll(gvWndType);
        EditorWindow gameview = objectsOfTypeAll.Length <= 0 ? null : (EditorWindow) objectsOfTypeAll[0];
        if (gvWndType != null && gameview != null)
        {
            MethodInfo snapZoomMethod = gvWndType.GetMethod("SnapZoom", BindingFlags.Instance | BindingFlags.NonPublic);
            PropertyInfo minScaleProperty =
                gvWndType.GetProperty("minScale", BindingFlags.Instance | BindingFlags.NonPublic);
            if (minScaleProperty != null && snapZoomMethod != null)
            {
                float minScale = (float) minScaleProperty.GetValue(gameview, null);
                snapZoomMethod.Invoke(gameview, new object[] {minScale});
                UnityEngine.Debug.LogError("minScale: " + minScale);
            }
        }

        gameview.Focus();
    }

    public static void SetMinScale3()
    {
        var gvWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");

        UnityEngine.Object[] objectsOfTypeAll = UnityEngine.Resources.FindObjectsOfTypeAll(gvWndType);
        EditorWindow gameview = objectsOfTypeAll.Length <= 0 ? null : (EditorWindow) objectsOfTypeAll[0];
        if (gvWndType != null && gameview != null)
        {
            MethodInfo snapZoomMethod = gvWndType.GetMethod("SnapZoom", BindingFlags.Instance | BindingFlags.NonPublic);
            PropertyInfo minScaleProperty =
                gvWndType.GetProperty("minScale", BindingFlags.Instance | BindingFlags.NonPublic);
            if (minScaleProperty != null && snapZoomMethod != null)
            {
                float minScale = (float) minScaleProperty.GetValue(gameview, null);
                snapZoomMethod.Invoke(gameview, new object[] {minScale});
                UnityEngine.Debug.LogError("minScale: " + minScale);
            }
        }
    }
    
    [MenuItem("Test/SizeDimensionsQuery")]
    public static void SizeDimensionsQueryTest()
    {
        Debug.Log(SizeExists(GameViewSizeGroupType.Android, 123, 456));
    }

    public static void AddCustomSize(GameViewSizeType viewSizeType, GameViewSizeGroupType sizeGroupType, int width,
        int height, string text)
    {
        // GameViewSizes group = gameViewSizesInstance.GetGroup(sizeGroupTyge);
        // group.AddCustomSize(new GameViewSize(viewSizeType, width, height, text);

        var group = GetGroup(sizeGroupType);
        var addCustomSize = getGroup.ReturnType.GetMethod("AddCustomSize"); // or group.GetType().
        var gvsType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSize");
        var gvstType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizeType");
        var ctor = gvsType.GetConstructor(new Type[] {gvstType, typeof(int), typeof(int), typeof(string)});
        var newSize = ctor.Invoke(new object[] {(int) viewSizeType, width, height, text});
        addCustomSize.Invoke(group, new object[] {newSize});
    }

    public static bool SizeExists(GameViewSizeGroupType sizeGroupType, string text)
    {
        return FindSize(sizeGroupType, text) != -1;
    }

    public static int FindSize(GameViewSizeGroupType sizeGroupType, string text)
    {
        // GameViewSizes group = gameViewSizesInstance.GetGroup(sizeGroupType);
        // string[] texts = group.GetDisplayTexts();
        // for loop...

        var group = GetGroup(sizeGroupType);
        var getDisplayTexts = group.GetType().GetMethod("GetDisplayTexts");
        var displayTexts = getDisplayTexts.Invoke(group, null) as string[];
        for (int i = 0; i < displayTexts.Length; i++)
        {
            string display = displayTexts[i];
            // the text we get is "Name (W:H)" if the size has a name, or just "W:H" e.g. 16:9
            // so if we're querying a custom size text we substring to only get the name
            // You could see the outputs by just logging
            // Debug.Log(display);
            int pren = display.IndexOf('(');
            if (pren != -1)
                display = display.Substring(0,
                    pren - 1); // -1 to remove the space that's before the prens. This is very implementation-depdenent
            if (display == text)
                return i;
        }

        return -1;
    }

    public static bool SizeExists(GameViewSizeGroupType sizeGroupType, int width, int height)
    {
        return FindSize(sizeGroupType, width, height) != -1;
    }

    public static int FindSize(GameViewSizeGroupType sizeGroupType, int width, int height)
    {
        // goal:
        // GameViewSizes group = gameViewSizesInstance.GetGroup(sizeGroupType);
        // int sizesCount = group.GetBuiltinCount() + group.GetCustomCount();
        // iterate through the sizes via group.GetGameViewSize(int index)

        var group = GetGroup(sizeGroupType);
        var groupType = group.GetType();
        var getBuiltinCount = groupType.GetMethod("GetBuiltinCount");
        var getCustomCount = groupType.GetMethod("GetCustomCount");
        int sizesCount = (int) getBuiltinCount.Invoke(group, null) + (int) getCustomCount.Invoke(group, null);
        var getGameViewSize = groupType.GetMethod("GetGameViewSize");
        var gvsType = getGameViewSize.ReturnType;
        var widthProp = gvsType.GetProperty("width");
        var heightProp = gvsType.GetProperty("height");
        var indexValue = new object[1];
        for (int i = 0; i < sizesCount; i++)
        {
            indexValue[0] = i;
            var size = getGameViewSize.Invoke(group, indexValue);
            int sizeWidth = (int) widthProp.GetValue(size, null);
            int sizeHeight = (int) heightProp.GetValue(size, null);
            if (sizeWidth == width && sizeHeight == height)
                return i;
        }

        return -1;
    }

    static object GetGroup(GameViewSizeGroupType type)
    {
        return getGroup.Invoke(gameViewSizesInstance, new object[] {(int) type});
    }

    [MenuItem("Test/LogCurrentGroupType")]
    public static void LogCurrentGroupType()
    {
        Debug.Log(GetCurrentGroupType());
    }

    public static GameViewSizeGroupType GetCurrentGroupType()
    {
        var getCurrentGroupTypeProp = gameViewSizesInstance.GetType().GetProperty("currentGroupType");
        return (GameViewSizeGroupType) (int) getCurrentGroupTypeProp.GetValue(gameViewSizesInstance, null);
    }
}