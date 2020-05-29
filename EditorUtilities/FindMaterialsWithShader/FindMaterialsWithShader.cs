using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Tool for finding materials that uses specific shaders.
/// </summary>
public class FindMaterialsWithShader : EditorWindow
{
    private Shader m_shader;

    private List<Material> m_materials = new List<Material>();

    private Vector2 m_scroll;

    [MenuItem("Tools/Find Materials with Shader")]
    private static void Init()
    {
        var window = (FindMaterialsWithShader) GetWindow(typeof(FindMaterialsWithShader));
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        GUILayout.Label("Find Materials with Shader", EditorStyles.boldLabel);

        m_shader = (Shader) EditorGUILayout.ObjectField("Shader", m_shader, typeof(Shader), false);

        EditorGUILayout.Space();

        if (m_materials.Count > 0)
        {
            m_scroll = EditorGUILayout.BeginScrollView(m_scroll, false, false);

            foreach (var material in m_materials)
            {
                EditorGUILayout.ObjectField(material, typeof(Material), false);
            }
            
            EditorGUILayout.EndScrollView();
        }
        else
        {
            EditorGUILayout.LabelField("No materials found");
        }

        EditorGUILayout.Space();
        GUILayout.FlexibleSpace();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Find Materials", GUILayout.Height(30)))
        {
            m_materials = FindMaterials();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.EndVertical();
    }

    private List<Material> FindMaterials()
    {
        var materials = new List<Material>();

        var guids = AssetDatabase.FindAssets("t:material");

        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var asset = (Material) AssetDatabase.LoadAssetAtPath(path, typeof(Material));

            if (asset != null)
            {
                if (asset.shader == m_shader)
                {
                    materials.Add(asset);
                }
            }
        }

        return materials;
    }
}
