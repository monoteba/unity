using UnityEditor;
using UnityEngine;

public class CopyAnimationEvents : EditorWindow
{
    private AnimationClip _clipToCopy;
    private AnimationClip _clipToPaste;

    [MenuItem("Assets/Copy Animation Events", false, 30)]
    public static void ShowWindow()
    {
        // create window and set title and size
        var window = GetWindow<CopyAnimationEvents>();
        window.titleContent = new GUIContent("Copy Events");
        window.minSize = new Vector2(200, 100);
        window.maxSize = new Vector2(4000, 4000);
    }

    private void OnGUI()
    {
        // window layout
        GUILayout.Label("Copy Animation Events", EditorStyles.boldLabel);

        var labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.alignment = TextAnchor.MiddleRight;

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Copy", labelStyle, GUILayout.Width(70));
        _clipToCopy = EditorGUILayout.ObjectField(_clipToCopy, typeof(AnimationClip), true) as AnimationClip;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Paste", labelStyle, GUILayout.Width(70));
        _clipToPaste = EditorGUILayout.ObjectField(_clipToPaste, typeof(AnimationClip), true) as AnimationClip;
        EditorGUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Copy"))
        {
            if (_clipToCopy != null && _clipToPaste != null)
            {
                CopyAnimationEventsModel.CopyAnimationEvents(_clipToCopy, _clipToPaste);
            }
        }

        GUILayout.Space(6);
    }
}

public class CopyAnimationEventsModel : AssetPostprocessor
{
    public static void CopyAnimationEvents(AnimationClip clipToCopy, AnimationClip clipToPaste)
    {
        var events = clipToCopy.events;

        var assetPath = AssetDatabase.GetAssetPath(clipToPaste);
        var assetImporter = AssetImporter.GetAtPath(assetPath);
        var modelImporter = assetImporter as ModelImporter;


        // if animation is not from a model (like fbx), paste the events to the clip directly
        if (modelImporter == null)
        {
            Undo.RecordObject(clipToPaste, "Copy Animation Events");
            AnimationUtility.SetAnimationEvents(clipToPaste, clipToCopy.events);
            return;
        }

        // loop through all clips in model importer, and assign events to clips that match the name
        // note: this can cause issues with clips of the same name on the same asset
        var clips = GetModelImporterClips(modelImporter);
        foreach (var clip in clips)
        {
            if (clip.name == clipToPaste.name)
            {
                clip.events = events;
            }
        }
        // assign the modified clips back to the model importer clips, and re-import the asset
        Undo.RecordObject(modelImporter, "Copy Animation Events");
        modelImporter.clipAnimations = clips;
        AssetDatabase.ImportAsset(assetPath);
    }

    private static ModelImporterClipAnimation[] GetModelImporterClips(ModelImporter importer)
    {
        // import custom clips
        var clips = importer.clipAnimations;
        if (clips.Length == 0)
        {
            // if no custom clips, import default
            clips = importer.defaultClipAnimations;
        }
        return clips;
    }
}