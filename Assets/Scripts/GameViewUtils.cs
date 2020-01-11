using System;
using System.Reflection;
using UnityEditor;

public static class GameViewUtils
{
    private static readonly object gameViewSizesInstance;
    private static readonly MethodInfo getGroup;
    private static int screenIndex = 18;
    private static int gameViewProfilesCount;

    static GameViewUtils()
    {
        var sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
        var singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        var instanceProp = singleType.GetProperty("instance");
        getGroup = sizesType.GetMethod("GetGroup");
        gameViewSizesInstance = instanceProp.GetValue(null, null);
    }

    private enum GameViewSizeType
    {
        AspectRatio,
        FixedResolution
    }

    public static int FindSize(GameViewSizeGroupType sizeGroupType, int width, int height)
    {
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

    public static void AddCustomSize(int width, int height, string text)
    {
        var group = GetGroup(GameViewSizeGroupType.Android);
        var addCustomSize = getGroup.ReturnType.GetMethod("AddCustomSize"); // or group.GetType().
        var gvsType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSize");
        var gvstType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizeType");
        var ctor = gvsType.GetConstructor(new Type[] {gvstType, typeof(int), typeof(int), typeof(string)});
        var newSize = ctor.Invoke(new object[] {(int) GameViewSizeType.FixedResolution, width, height, text});
        addCustomSize.Invoke(group, new object[] {newSize});
    }

    public static void SetSize(int index)
    {
        var gvWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
        var gvWnd = EditorWindow.GetWindow(gvWndType);
        var SizeSelectionCallback = gvWndType.GetMethod("SizeSelectionCallback",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        SizeSelectionCallback.Invoke(gvWnd, new object[] {index, null});
        
        /*var gameCanvases = GameObject.FindObjectsOfType<Canvas>();
        foreach (var gameCanvas in gameCanvases)
        {
            EditorUtility.SetDirty(gameCanvas);
        }*/
    }

    static object GetGroup(GameViewSizeGroupType type)
    {
        return getGroup.Invoke(gameViewSizesInstance, new object[] {(int) type});
    }

   /* [MenuItem("Tools/GameViewSize/Previous %&Q")]
    private static void SetPrevious()
    {
        GetViewListSize();
        if (screenIndex - 1 >= 16)
        {
            screenIndex -= 1;
        }
        else
        {
            screenIndex = gameViewProfilesCount - 1;
        }

        SetSize(screenIndex);
    }

    [MenuItem("Tools/GameViewSize/Next  %&E")]
    private static void SetNext()
    {
        GetViewListSize();
        if (screenIndex + 1 < gameViewProfilesCount)
        {
            screenIndex += 1;
        }
        else
        {
            screenIndex = 16;
        }

        SetSize(screenIndex);
    }*/

    private static void GetViewListSize()
    {
        var group = GetGroup(GameViewSizeGroupType.Android);
        var getDisplayTexts = group.GetType().GetMethod("GetDisplayTexts");
        gameViewProfilesCount = (getDisplayTexts.Invoke(group, null) as string[]).Length;
    }
}