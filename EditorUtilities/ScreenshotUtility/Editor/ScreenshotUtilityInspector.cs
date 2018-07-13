using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ScreenshotUtility))]
public class ScreenshotUtilityInspector : Editor
{
    private SerializedProperty _screenshotKey;
    private SerializedProperty _pauseKey;
    private SerializedProperty _optionalCamera;

    private SerializedProperty _width;
    private SerializedProperty _height;

    private SerializedProperty _filename;
    private SerializedProperty _screenshotFolder;
    private SerializedProperty _imageFormat;
    private SerializedProperty _transparentBackground;

    private void OnEnable()
    {
        _screenshotKey = serializedObject.FindProperty("ScreenshotKey");
        _pauseKey = serializedObject.FindProperty("PauseKey");
        _optionalCamera = serializedObject.FindProperty("OptionalCamera");

        _width = serializedObject.FindProperty("Width");
        _height = serializedObject.FindProperty("Height");

        _filename = serializedObject.FindProperty("Filename");
        _screenshotFolder = serializedObject.FindProperty("ScreenshotFolder");
        _imageFormat = serializedObject.FindProperty("ImageFormat");
        _transparentBackground = serializedObject.FindProperty("TransparentBackground");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // display script field
        var prop = serializedObject.FindProperty("m_Script");
        GUI.enabled = false;
        EditorGUILayout.PropertyField(prop, true);
        GUI.enabled = true;

        EditorGUILayout.Space();

        // begin change check...
        EditorGUI.BeginChangeCheck();

        // key presses
        EditorGUILayout.BeginVertical();
        EditorGUILayout.PropertyField(_screenshotKey);
        EditorGUILayout.PropertyField(_pauseKey);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // optional camera
        EditorGUILayout.BeginVertical();
        EditorGUILayout.PropertyField(_optionalCamera);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // width and height
        EditorGUILayout.BeginVertical();
        EditorGUILayout.PropertyField(_width);
        EditorGUILayout.PropertyField(_height);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // filename and output
        EditorGUILayout.BeginVertical();
        EditorGUILayout.PropertyField(_filename);
        EditorGUILayout.PropertyField(_imageFormat);
        var imageFormat = (ScreenshotUtility.Format) _imageFormat.enumValueIndex;
        switch (imageFormat)
        {
            case ScreenshotUtility.Format.PNG:
            case ScreenshotUtility.Format.EXR:
                EditorGUILayout.PropertyField(_transparentBackground, new GUIContent("Transparent BG"));
                break;
        }

        // output folder path
        _screenshotFolder.stringValue = EditorGUILayout.TextField("Folder", _screenshotFolder.stringValue);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(" ");
        if (GUILayout.Button("Choose Folder"))
        {
            var path = EditorUtility.OpenFolderPanel("Choose Screenshot Folder", "", "");
            if (!string.IsNullOrEmpty(path))
                _screenshotFolder.stringValue = path;
        }

        _screenshotFolder.stringValue = _screenshotFolder.stringValue.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);


        var invalidDirectory = string.IsNullOrEmpty(_screenshotFolder.stringValue) ||
                               Directory.Exists(_screenshotFolder.stringValue) == false;

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // warning message
        if (invalidDirectory)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.HelpBox("Please choose an existing directory for saving the screenshots.", MessageType.Warning);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        // open folder
        EditorGUILayout.BeginVertical();
        GUI.enabled = !invalidDirectory;

        if (GUILayout.Button("Open Screenshot Folder", GUILayout.Height(22)))
        {
            Process.Start(_screenshotFolder.stringValue);
        }

        GUI.enabled = true;
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        // end change check
        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();
    }
}