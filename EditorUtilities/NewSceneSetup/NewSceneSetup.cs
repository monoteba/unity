using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class NewSceneSetup : Editor
{
    static NewSceneSetup()
    {
        EditorSceneManager.newSceneCreated += OnNewScene;
    }

    private static void OnNewScene(Scene scene, UnityEditor.SceneManagement.NewSceneSetup setup, NewSceneMode mode)
    {
        Camera.main.backgroundColor = Color.black;
        Camera.main.clearFlags = CameraClearFlags.SolidColor;
        Camera.main.transform.position = Vector3.zero;

        var light = FindObjectOfType<Light>();
        light.color = Color.white;
        light.transform.position = Vector3.one;

        RenderSettings.skybox = null;
        RenderSettings.ambientIntensity = 0f;
        RenderSettings.ambientLight = Color.black;
        RenderSettings.ambientMode = AmbientMode.Flat;
        Lightmapping.bakedGI = false;
    }
}
