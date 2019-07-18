using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

public class DebugButtonAttribute : Attribute
{
}

#if UNITY_EDITOR

[CustomEditor(typeof(MonoBehaviour), true), CanEditMultipleObjects]
public class DrawDebugButtonAttributes : Editor
{
    private class ParamTuple
    {
        public object Obj;
        public ParameterInfo ParamInfo;
    }

    private class MethodCallInfo
    {
        public MethodInfo MethodInfo;
        public List<ParamTuple> Parameters;
    }

    private List<MethodCallInfo> _methodCallInfo;

    public void OnEnable()
    {
        var methods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (var method in methods)
        {
            if (method.GetCustomAttributes(typeof(DebugButtonAttribute), false).Any())
            {
                if (_methodCallInfo == null)
                    _methodCallInfo = new List<MethodCallInfo>();

                var mci = new MethodCallInfo
                {
                    MethodInfo = method,
                    Parameters = new List<ParamTuple>()
                };

                foreach (var p in method.GetParameters().ToList())
                {
                    mci.Parameters.Add(new ParamTuple {Obj = p.DefaultValue, ParamInfo = p});
                }

                _methodCallInfo.Add(mci);
            }
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (_methodCallInfo == null || !_methodCallInfo.Any())
        {
            return;
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);

        foreach (var info in _methodCallInfo)
        {
            EditorGUILayout.Space();
            HorizontalLine();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            var methodName = FormatName(info.MethodInfo.Name);

            EditorGUILayout.PrefixLabel(methodName, "Button", EditorStyles.boldLabel);

            if (GUILayout.Button(methodName))
            {
                var result = info.MethodInfo.Invoke(target, info.Parameters.Select(p => p.Obj).ToArray());
                if (result != null)
                {
                    Debug.Log(info.MethodInfo.Name + " returned: " + result);
                }
            }

            EditorGUILayout.EndHorizontal();

            if (info.Parameters.Count > 0)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.BeginVertical();
                foreach (var p in info.Parameters)
                {
                    var param = p;
                    var paramName = FormatName(param.ParamInfo.Name);

                    if (param.ParamInfo.ParameterType.IsValueType == false)
                    {
                        if (param.ParamInfo.ParameterType.BaseType == typeof(MonoBehaviour))
                        {
                            param.Obj = (MonoBehaviour) EditorGUILayout.ObjectField(paramName, param.Obj as MonoBehaviour, typeof(MonoBehaviour), true);
                        }
                        else
                        {
                            if (CheckNull<GameObject>(ref param, f => (GameObject) EditorGUILayout.ObjectField(paramName, f, typeof(GameObject), true)))
                                continue;
                            if (CheckNull<Transform>(ref param, f => (Transform) EditorGUILayout.ObjectField(paramName, f, typeof(Transform), true)))
                                continue;
                            if (CheckNull<Renderer>(ref param, f => (Renderer) EditorGUILayout.ObjectField(paramName, f, typeof(Renderer), true)))
                                continue;
                            if (CheckNull<MeshFilter>(ref param, f => (MeshFilter) EditorGUILayout.ObjectField(paramName, f, typeof(MeshFilter), true)))
                                continue;
                            if (CheckNull<Camera>(ref param, f => (Camera) EditorGUILayout.ObjectField(paramName, f, typeof(Camera), true)))
                                continue;
                            if (CheckNull<ParticleSystem>(ref param, f => (ParticleSystem) EditorGUILayout.ObjectField(paramName, f, typeof(ParticleSystem), true)))
                                continue;
                            if (CheckNull<Animator>(ref param, f => (Animator) EditorGUILayout.ObjectField(paramName, f, typeof(Animator), true)))
                                continue;
                            if (CheckNull<Rigidbody>(ref param, f => (Rigidbody) EditorGUILayout.ObjectField(paramName, f, typeof(Rigidbody), true)))
                                continue;
                            if (CheckNull<Rigidbody2D>(ref param, f => (Rigidbody2D) EditorGUILayout.ObjectField(paramName, f, typeof(Rigidbody2D), true)))
                                continue;
                            if (CheckNull<Collider>(ref param, f => (Collider) EditorGUILayout.ObjectField(paramName, f, typeof(Collider), true)))
                                continue;
                            if (CheckNull<Collider2D>(ref param, f => (Collider2D) EditorGUILayout.ObjectField(paramName, f, typeof(Collider2D), true)))
                                continue;
                            if (CheckNull<Texture2D>(ref param, f => (Texture2D) EditorGUILayout.ObjectField(paramName, f, typeof(Texture2D), true)))
                                continue;
                            if (CheckNull<Sprite>(ref param, f => (Sprite) EditorGUILayout.ObjectField(paramName, f, typeof(Sprite), true)))
                                continue;
                            if (CheckNull<AudioClip>(ref param, f => (AudioClip) EditorGUILayout.ObjectField(paramName, f, typeof(AudioClip), true)))
                                continue;
                            if (CheckNull<ScriptableObject>(ref param, f => (ScriptableObject) EditorGUILayout.ObjectField(paramName, f, typeof(ScriptableObject), true)))
                                continue;
                        }
                    }
                    else
                    {
                        if (CheckNull<bool>(ref param, value => EditorGUILayout.Toggle(paramName, value)))
                            continue;
                        if (CheckNull<int>(ref param, value => EditorGUILayout.IntField(paramName, value)))
                            continue;
                        if (CheckNull<float>(ref param, value => EditorGUILayout.FloatField(paramName, value)))
                            continue;
                        if (CheckNull<string>(ref param, value => EditorGUILayout.TextField(paramName, value)))
                            continue;
                        if (CheckNull<Vector2>(ref param, value => EditorGUILayout.Vector2Field(paramName, value)))
                            continue;
                        if (CheckNull<Vector3>(ref param, value => EditorGUILayout.Vector3Field(paramName, value)))
                            continue;
                        if (CheckNull<Vector4>(ref param, value => EditorGUILayout.Vector4Field(paramName, value)))
                            continue;
                        if (CheckNull<Color>(ref param, value => EditorGUILayout.ColorField(paramName, value)))
                            continue;
                        if (CheckNull<LayerMask>(ref param, value => EditorGUILayout.LayerField(paramName, value)))
                            continue;
                    }
                }

                EditorGUILayout.EndVertical();

                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
        }

        EditorGUILayout.EndVertical();
    }

    private bool CheckNull<T>(ref ParamTuple tup, Func<T, T> drawMethod)
    {
        if (tup.ParamInfo.ParameterType == typeof(T))
        {
            if (DBNull.Value.Equals(tup.Obj))
                tup.Obj = default(T);

            tup.Obj = drawMethod((T) tup.Obj);
            return true;
        }

        return false;
    }

    private string FormatName(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return "";
        }

        str = str.First().ToString().ToUpper() + str.Substring(1);

        return string.Concat(str.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
    }

    private void HorizontalLine()
    {
        var rect = EditorGUILayout.GetControlRect(false, 1);
        rect.height = 1;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
    }
}

#endif
