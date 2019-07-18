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
        var methods = target.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

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
        EditorGUILayout.LabelField("Debug Methods", EditorStyles.boldLabel);

        foreach (var info in _methodCallInfo)
        {
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

                foreach (var t in info.Parameters)
                {
                    EditorGUILayout.BeginVertical();
                    var p = t;
                    var paramName = FormatName(p.ParamInfo.Name);
                    CheckNull<float>(ref p, value => EditorGUILayout.FloatField(paramName, value));
                    CheckNull<int>(ref p, value => EditorGUILayout.IntField(paramName, value));
                    CheckNull<bool>(ref p, value => EditorGUILayout.Toggle(paramName, value));
                    CheckNull<string>(ref p, value => EditorGUILayout.TextField(paramName, value));
                    CheckNull<Vector2>(ref p, value => EditorGUILayout.Vector2Field(paramName, value));
                    CheckNull<Vector3>(ref p, value => EditorGUILayout.Vector3Field(paramName, value));
                    CheckNull<Vector4>(ref p, value => EditorGUILayout.Vector4Field(paramName, value));
                    CheckNull<Color>(ref p, value => EditorGUILayout.ColorField(paramName, value));
                    CheckNull<LayerMask>(ref p, value => EditorGUILayout.LayerField(paramName, value));
                    CheckNull<AnimationCurve>(ref p, value => EditorGUILayout.CurveField(paramName, value));
                    CheckNull<Transform>(ref p,
                        value => (Transform) EditorGUILayout.ObjectField(paramName, value, typeof(Transform), true));
                    CheckNull<ScriptableObject>(ref p,
                        value => (ScriptableObject) EditorGUILayout.ObjectField(paramName, value,
                            typeof(UnityEngine.Object), true));
                    CheckNull<MonoBehaviour>(ref p,
                        value => (MonoBehaviour) EditorGUILayout.ObjectField(paramName, value,
                            typeof(UnityEngine.Object), true));

                    EditorGUILayout.EndVertical();
                }

                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
        }

        EditorGUILayout.EndVertical();
    }

    private void CheckNull<T>(ref ParamTuple tup, Func<T, T> drawMethod)
    {
        if (typeof(T) == tup.ParamInfo.ParameterType)
        {
            if (DBNull.Value.Equals(tup.Obj))
                tup.Obj = default(T);

            tup.Obj = drawMethod((T) tup.Obj);
        }
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
}

#endif
