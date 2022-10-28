using System;
using System.IO;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.ShortcutManagement;
using UnityEngine;

/// <summary>
/// Select Tool for selecting objects in the scene view, without obtrusive handles or gizmos preventing selection.
///
/// There is no custom implementation for selecting objects, as Unity seems to default to selecting objects when no
/// other behavior is defined...
/// </summary>
[EditorTool("Select Tool")]
public class SelectTool : EditorTool
{
    private static Texture2D _icon;
    private static Texture2D _activeIcon;
    private static Texture2D _darkIcon;
    private static Texture2D _lightIcon;

    public override GUIContent toolbarIcon => new GUIContent
    {
        image = _icon,
        text = "Select Tool",
        tooltip = "Select Tool"
    };

    private void OnEnable()
    {
        LoadIcons();
        UpdateIcon(false);
    }

    public override void OnActivated()
    {
        UpdateIcon(true);
    }

    public override void OnWillBeDeactivated()
    {
        UpdateIcon(false);
    }

    [Shortcut("Select Tool", KeyCode.Q)]
    private static void Shortcut()
    {
        ToolManager.SetActiveTool<SelectTool>();
    }

    private void LoadIcons()
    {
        var script = MonoScript.FromScriptableObject(this);
        var path = AssetDatabase.GetAssetPath(script);

        path = Path.GetDirectoryName(path);

        try
        {
            _activeIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(path + "/SelectToolIcon-Active.png");
            _darkIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(path + "/SelectToolIcon-DarkTheme.png");
            _lightIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(path + "/SelectToolIcon-LightTheme.png");
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    private void UpdateIcon(bool active)
    {
        if (active)
        {
            _icon = _activeIcon;
        }
        else
        {
            _icon = EditorGUIUtility.isProSkin ? _darkIcon : _lightIcon;
        }
    }
}
