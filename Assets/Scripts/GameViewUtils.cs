using System.Reflection;
using UnityEditor;

public class GameViewUtils
{
    static readonly object gameViewSizesInstance;
    static readonly MethodInfo getGroup;
    private static int screenIndex = 16; // Because have 16 indexes in my list.
    private static int gameViewProfilesCount;

    static GameViewUtils()
    {
        // gameViewSizesInstance  = ScriptableSingleton<GameViewSizes>.instance;
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

    public static void SetSize(int index)
    {
        var gvWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
        var gvWnd = EditorWindow.GetWindow(gvWndType);
        var SizeSelectionCallback = gvWndType.GetMethod("SizeSelectionCallback",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        SizeSelectionCallback.Invoke(gvWnd, new object[] {index, null});
    }

    static object GetGroup(GameViewSizeGroupType type)
    {
        return getGroup.Invoke(gameViewSizesInstance, new object[] {(int) type});
    }

    [MenuItem("Tools/GameViewSize/Previous %&Q")]
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
    }

    private static void GetViewListSize()
    {
        var group = GetGroup(GameViewSizeGroupType.Android);
        var getDisplayTexts = group.GetType().GetMethod("GetDisplayTexts");
        gameViewProfilesCount = (getDisplayTexts.Invoke(group, null) as string[]).Length;
    }
}