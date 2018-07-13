using System.Collections;
using System.IO;
using UnityEngine;

public class ScreenshotUtility : MonoBehaviour
{
    [Header("Keys")]
    public KeyCode ScreenshotKey = KeyCode.Space;
    public KeyCode PauseKey = KeyCode.P;

    [Header("Camera")]
    public Camera OptionalCamera;

    [Header("Resolution")]
    public int Width = 1920;
    public int Height = 1080;

    [Header("File Output")]
    public string Filename = "Screenshot";
    public string ScreenshotFolder;
    public Format ImageFormat = Format.PNG;
    public bool TransparentBackground;

    public enum Format
    {
        PNG,
        JPEG,
        EXR
    }

    private float _prePauseTimeScale = 1f;

    private void Update()
    {
        // screenshot
        if (Input.GetKeyDown(ScreenshotKey))
        {
            StartCoroutine(Screenshot());
        }

        // pause
        if (Input.GetKeyDown(PauseKey))
        {
            if (Time.timeScale > 0)
            {
                _prePauseTimeScale = Time.timeScale;
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = _prePauseTimeScale;
            }
        }
    }

    private IEnumerator Screenshot()
    {
        yield return new WaitForEndOfFrame();

        // get camera
        var cam = OptionalCamera ? OptionalCamera : Camera.main;
        if (cam == null)
            yield break;

        // has directory been set?
        if (string.IsNullOrEmpty(ScreenshotFolder))
        {
            Debug.LogError("Please choose a screenshot directory!");
            yield break;
        }

        // does directory exists?
        if (Directory.Exists(ScreenshotFolder) == false)
        {
            Debug.LogError("Screenshot directory does not exist! Please create it.");
            yield break;
        }

        // set camera clear flags based on alpha channel setting
        var clearFlags = cam.clearFlags;
        switch (ImageFormat)
        {
            case Format.PNG:
            case Format.EXR:
                cam.clearFlags = TransparentBackground ? CameraClearFlags.Depth : clearFlags;
                break;
        }

        // create render texture
        var renderTexture = new RenderTexture(Width, Height, 32);
        cam.targetTexture = renderTexture;

        // determine output texture format
        TextureFormat textureFormat;
        switch (ImageFormat)
        {
            case Format.PNG:
                textureFormat = TransparentBackground ? TextureFormat.RGBA32 : TextureFormat.RGB24;
                break;
            case Format.EXR:
                textureFormat = TextureFormat.RGBAFloat;
                break;
            case Format.JPEG:
                textureFormat = TextureFormat.RGB24;
                break;
            default:
                textureFormat = TextureFormat.RGB24;
                break;
        }

        var captureTexture = new Texture2D(Width, Height, textureFormat, false);

        // capture to render texture
        cam.Render();
        RenderTexture.active = renderTexture;
        captureTexture.ReadPixels(new Rect(0, 0, Width, Height), 0, 0);
        captureTexture.Apply();
        cam.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        // encode render texture to format
        byte[] bytes;
        string ext;
        switch (ImageFormat)
        {
            case Format.PNG:
                bytes = captureTexture.EncodeToPNG();
                ext = "png";
                break;
            case Format.JPEG:
                bytes = captureTexture.EncodeToJPG();
                ext = "jpg";
                break;
            case Format.EXR:
                bytes = captureTexture.EncodeToEXR(Texture2D.EXRFlags.CompressZIP);
                ext = "exr";
                break;
            default:
                bytes = captureTexture.EncodeToPNG();
                ext = "png";
                break;
        }

        // write file
        var fullpath = GetFullPath(ext);
        File.WriteAllBytes(fullpath, bytes);
        Debug.Log(string.Format("Screenshot: {0}", fullpath));
        DestroyImmediate(captureTexture);

        // reset camera clear flags
        cam.clearFlags = clearFlags;
    }

    private string GetFullPath(string ext)
    {
        var path = string.Format("{0}/{1}_{2:yyyy-MM-dd HH.mm.ss}.{3}",
            ScreenshotFolder, Filename, System.DateTime.Now, ext);

        return path;
    }
}