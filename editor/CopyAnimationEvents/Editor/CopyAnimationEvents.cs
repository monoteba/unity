using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CopyAnimationEvents : EditorWindow
{
    public AnimationClip[] ClipsToCopy;
    public AnimationClip[] ClipsToPaste;

    private Vector2 _scrollPos = Vector2.zero;

    [MenuItem("Assets/Copy Animation Events", false, 30)]
    public static void ShowWindow()
    {
        // create window and set title and size
        var window = GetWindow<CopyAnimationEvents>();
        window.titleContent = new GUIContent("Copy Events");
        window.minSize = new Vector2(400, 200);
        window.maxSize = new Vector2(4000, 4000);
    }

    public static Object[] DropZone(string title, int width, int height)
    {
        var style = new GUIStyle(GUI.skin.box);
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.white;
        style.fixedWidth = width;
        style.fixedHeight = height;

        GUILayout.Box(title, style);

        var mousePos = Event.current.mousePosition;
        var rect = GUILayoutUtility.GetLastRect();

        if (rect.Contains(mousePos))
        {
            EventType eventType = Event.current.type;
            if (eventType == EventType.DragUpdated || eventType == EventType.DragPerform)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (eventType == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    Event.current.Use();
                    return DragAndDrop.objectReferences;
                }
            }
        }

        return null;
    }


    private void OnGUI()
    {
        // create drop zones
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        var halfWidth = (int)(EditorGUIUtility.currentViewWidth / 2) - 15;

        // drop zones
        EditorGUILayout.BeginHorizontal();
        var dropClipsToCopy = DropZone("Drop clips to copy from", halfWidth, 50);
        var dropClipsToPaste = DropZone("Drop clips to paste to", halfWidth, 50);
        EditorGUILayout.EndHorizontal();

        // clear buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear Copy", GUILayout.Width(halfWidth)))
        {
            ClipsToCopy = new AnimationClip[0];
        }
        if (GUILayout.Button("Clear Paste", GUILayout.Width(halfWidth)))
        {
            ClipsToPaste = new AnimationClip[0];
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // only add type of AnimationClip to array
        if (dropClipsToCopy != null)
        {
            List<AnimationClip> clips = ClipsToCopy.ToList();
            foreach (var clip in dropClipsToCopy)
            {
                var c = clip as AnimationClip;
                if (c != null)
                    clips.Add(c);
            }
            ClipsToCopy = clips.ToArray();
        }

        if (dropClipsToPaste != null)
        {
            List<AnimationClip> clips = ClipsToPaste.ToList();
            foreach (var clip in dropClipsToPaste)
            {
                var c = clip as AnimationClip;
                if (c != null)
                    clips.Add(c);
            }
            ClipsToPaste = clips.ToArray();
        }

        ScriptableObject target = this;
        var so = new SerializedObject(target);
        so.Update();

        var clipsToCopyProperty = so.FindProperty("ClipsToCopy");
        var clipsToPasteProperty = so.FindProperty("ClipsToPaste");

        // layout two columns with array of clips to copy and paste
        EditorGUILayout.BeginHorizontal(GUILayout.Width(halfWidth * 2));

        // copy column
        EditorGUILayout.BeginVertical();
        EditorGUILayout.PropertyField(clipsToCopyProperty.FindPropertyRelative("Array.size"), new GUIContent("Number of clips"), GUILayout.Width(halfWidth));
        for (int i = 0; i < clipsToCopyProperty.arraySize; i++)
        {
            EditorGUILayout.PropertyField(clipsToCopyProperty.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.Width(halfWidth));
        }
        EditorGUILayout.EndVertical();

        // paste column
        EditorGUILayout.BeginVertical();
        EditorGUILayout.PropertyField(clipsToPasteProperty.FindPropertyRelative("Array.size"), new GUIContent("Number of clips"), GUILayout.Width(halfWidth));
        for (int i = 0; i < clipsToPasteProperty.arraySize; i++)
        {
            EditorGUILayout.PropertyField(clipsToPasteProperty.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.Width(halfWidth));
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();

        so.ApplyModifiedProperties();

        GUILayout.FlexibleSpace();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space();
        if (GUILayout.Button("Copy"))
        {
            if (ClipsToCopy.Length > 0 && ClipsToPaste.Length > 0)
            {
                CopyAnimationEventsModel.CopyAnimationEvents(ClipsToCopy, ClipsToPaste);
            }
        }
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
    }
}

public class CopyAnimationEventsModel : AssetPostprocessor
{
    public static void CopyAnimationEvents(AnimationClip[] clipsToCopy, AnimationClip[] clipsToPaste)
    {
        AssetDatabase.StartAssetEditing();

        Undo.SetCurrentGroupName("Copy Animation Events");
        var undoGroup = Undo.GetCurrentGroup();

        for (int i = 0; i < clipsToCopy.Length; i++)
        {
            if (clipsToCopy[i] == null || clipsToPaste[i] == null)
                continue;

            // get events from copy clip
            AnimationEvent[] events = null;

            var assetPath = AssetDatabase.GetAssetPath(clipsToCopy[i]);
            var assetImporter = AssetImporter.GetAtPath(assetPath);
            var modelImporter = assetImporter as ModelImporter;

            if (modelImporter == null)
            {
                events = AnimationUtility.GetAnimationEvents(clipsToCopy[i]);
            }
            else
            {
                var modelClips = GetModelImporterClips(modelImporter);
                foreach (var clip in modelClips)
                {
                    if (clip.name == clipsToCopy[i].name)
                    {
                        events = clip.events;
                    }
                }
            }

            // get model importer for paste clip
            assetPath = AssetDatabase.GetAssetPath(clipsToPaste[i]);
            assetImporter = AssetImporter.GetAtPath(assetPath);
            modelImporter = assetImporter as ModelImporter;


            // if animation is not from a model (like fbx), paste the events to the clip directly
            if (modelImporter == null)
            {
                Undo.RecordObject(clipsToPaste[i], "Copy Animation Events");
                AnimationUtility.SetAnimationEvents(clipsToPaste[i], events);
            }
            else
            {
                // loop through all clips in model importer, and assign events to clips that match the name
                // note: this can cause issues with clips of the same name on the same asset
                var clips = GetModelImporterClips(modelImporter);
                foreach (var clip in clips)
                {
                    if (clip.name == clipsToPaste[i].name)
                    {
                        clip.events = events;
                    }
                }
                // assign the modified clips back to the model importer clips, and re-import the asset
                Undo.RecordObject(modelImporter, "Copy Animation Events");
                modelImporter.clipAnimations = clips;
                modelImporter.SaveAndReimport();
            }
        }
        AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh();

        Undo.CollapseUndoOperations(undoGroup);
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