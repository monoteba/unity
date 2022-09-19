using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

/// <summary>
/// Multi-touch pinch to zoom using Unity's new Input System
/// </summary>
public class PinchToZoom : MonoBehaviour
{
    private Camera m_camera;
    private bool m_pinchStarted;
    private Vector2 m_previousPinchCenter;
    private float m_previousDistance;

    private void Awake()
    {
        EnhancedTouchSupport.Enable();
        m_camera = Camera.main;
    }

    private void OnDestroy()
    {
        EnhancedTouchSupport.Disable();
    }

    private void Update()
    {
        if (Touch.activeTouches.Count < 2)
        {
            m_pinchStarted = false;
            return;
        }

        Touch touch1 = Touch.activeTouches[0];
        Touch touch2 = Touch.activeTouches[1];

        Vector2 pinchCenter = (touch1.screenPosition + touch2.screenPosition) / 2.0f;
        float distance = Vector2.Distance(touch1.screenPosition, touch2.screenPosition);

        if (m_pinchStarted)
        {
            Vector2 delta = pinchCenter - m_previousPinchCenter;
            float scale = Mathf.Max(0, distance / m_previousDistance);

            MoveCamera(m_camera, delta);
            ZoomCamera(m_camera, scale);
        }

        m_previousPinchCenter = pinchCenter;
        m_previousDistance = distance;
        m_pinchStarted = true;
    }

    private static void MoveCamera(Camera camera, Vector2 delta)
    {
        Vector3 position = camera.transform.position;
        float depth = Mathf.Abs(position.z);
        
        Vector3 start = camera.ScreenToWorldPoint(new Vector3(0, 0, depth));
        Vector3 end = camera.ScreenToWorldPoint(new Vector3(delta.x, delta.y, depth));
        position -= end - start;
        
        camera.transform.position = position;
    }

    private static void ZoomCamera(Camera camera, float scale)
    {
        if (camera.orthographic)
        {
            // Inverse scale of the orthographic size
            camera.orthographicSize = Mathf.Max(0.1f, 1f / scale * camera.orthographicSize);
        }
        else
        {
            // Move the camera by the scale
            Transform t = camera.transform;
            Vector3 position = t.position;
            position.z /= scale;
            t.position = position;
        }
    }
}
