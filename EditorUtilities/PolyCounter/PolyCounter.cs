using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PolyCounter : EditorWindow
{
    public enum MeshComponent
    {
        Vertices,
        Triangles
    }

    public MeshComponent component;
    public int threshold = 5000;

    private class ItemCount
    {
        public ItemCount(GameObject gameObject, int count)
        {
            this.gameObject = gameObject;
            this.count = count;
        }
        
        public GameObject gameObject;
        public int count;
    }

    private List<ItemCount> m_items = new List<ItemCount>();
    private Vector2 m_scroll;
    
    [MenuItem("Tools/Poly Counter")]
    private static void Init()
    {
        var window = (PolyCounter) GetWindow(typeof(PolyCounter));
        window.minSize = new Vector2(400, 200);
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Find objects with a high poly count", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        component = (MeshComponent) EditorGUILayout.EnumPopup("Components", component);
        threshold = Mathf.Max(EditorGUILayout.IntField("Min Count", threshold), 3);
        
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical();
        
        m_scroll = EditorGUILayout.BeginScrollView(m_scroll, GUILayout.MaxHeight(400));
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Game Object", EditorStyles.boldLabel, GUILayout.MinWidth(0));
        EditorGUILayout.LabelField(component.ToString(), EditorStyles.boldLabel, GUILayout.MinWidth(0));
        EditorGUILayout.EndVertical();
        
        foreach (var item in m_items)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.ObjectField(item.gameObject, typeof(GameObject));
                EditorGUILayout.IntField(item.count);
            }
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndScrollView();
        
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Select Objects"))
        {
            Search();
        }

        EditorGUILayout.EndVertical();
    }

    private void Search()
    {
        var filters = FindObjectsOfType<MeshFilter>();
        var gos = new List<GameObject>();
        m_items = new List<ItemCount>();

        switch (component)
        {
            case MeshComponent.Vertices:
                foreach (var filter in filters)
                {
                    var count = filter.sharedMesh.vertexCount;
                    if (count >= threshold)
                    {
                        m_items.Add(new ItemCount(filter.gameObject, count));
                        gos.Add(filter.gameObject);
                    }
                }
                break;
            case MeshComponent.Triangles:
                foreach (var filter in filters)
                {
                    var count = filter.sharedMesh.triangles.Length; 
                    if (count >= threshold)
                    {
                        m_items.Add(new ItemCount(filter.gameObject, count));
                        gos.Add(filter.gameObject);
                    }
                }
                break;
        }

        m_items = m_items.OrderByDescending(item => item.count).ToList();

        if (m_items.Any())
        {
            Selection.objects = gos.ToArray();
            EditorGUIUtility.PingObject(m_items.First().gameObject);
        }
    }
}