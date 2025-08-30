using System.Collections;
using UnityEngine;
using UnityEngine.Android;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public int photoWidth = 1920;
    public int photoHeight = 1080;
    public int targetFPS = 30;

    [Header("References")]
    public RenderTexture renderTexture;
    
    private Camera mainCamera;
    private WebCamTexture webCamTexture;
    private bool isInitialized = false;
    
    public event System.Action<Texture2D> OnPhotoTaken;
    public event System.Action<string> OnError;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }
        
        RequestCameraPermission();
    }

    void RequestCameraPermission()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
            StartCoroutine(WaitForPermission());
        }
        else
        {
            InitializeCamera();
        }
    }

    IEnumerator WaitForPermission()
    {
        while (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            yield return new WaitForSeconds(0.1f);
        }
        InitializeCamera();
    }

    void InitializeCamera()
    {
        if (WebCamTexture.devices.Length == 0)
        {
            OnError?.Invoke("No camera devices found");
            return;
        }

        // Use back camera if available
        WebCamDevice? backCamera = null;
        foreach (WebCamDevice device in WebCamTexture.devices)
        {
            if (!device.isFrontFacing)
            {
                backCamera = device;
                break;
            }
        }

        string deviceName = backCamera?.name ?? WebCamTexture.devices[0].name;
        webCamTexture = new WebCamTexture(deviceName, photoWidth, photoHeight, targetFPS);
        
        // Create render texture if not assigned
        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(photoWidth, photoHeight, 24);
        }

        webCamTexture.Play();
        isInitialized = true;
        
        Debug.Log($"Camera initialized: {deviceName}, Resolution: {photoWidth}x{photoHeight}");
    }

    public void TakePhoto()
    {
        if (!isInitialized || webCamTexture == null || !webCamTexture.isPlaying)
        {
            OnError?.Invoke("Camera not initialized or not playing");
            return;
        }

        StartCoroutine(CapturePhoto());
    }

    IEnumerator CapturePhoto()
    {
        yield return new WaitForEndOfFrame();

        // Method 1: Capture from WebCamTexture (for mobile)
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            Texture2D photo = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.RGB24, false);
            photo.SetPixels(webCamTexture.GetPixels());
            photo.Apply();
            
            OnPhotoTaken?.Invoke(photo);
        }
        // Method 2: Capture from main camera (for standalone/editor)
        else if (mainCamera != null)
        {
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = renderTexture;
            
            mainCamera.targetTexture = renderTexture;
            mainCamera.Render();
            
            Texture2D photo = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
            photo.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            photo.Apply();
            
            mainCamera.targetTexture = null;
            RenderTexture.active = currentRT;
            
            OnPhotoTaken?.Invoke(photo);
        }
        else
        {
            OnError?.Invoke("No camera available for photo capture");
        }
    }

    public bool IsCameraReady()
    {
        return isInitialized && ((webCamTexture != null && webCamTexture.isPlaying) || mainCamera != null);
    }

    public Texture GetCameraTexture()
    {
        return webCamTexture != null ? (Texture)webCamTexture : null;
    }

    void OnDestroy()
    {
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
            Destroy(webCamTexture);
        }
        
        if (renderTexture != null)
        {
            renderTexture.Release();
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (webCamTexture != null)
        {
            if (pauseStatus)
                webCamTexture.Pause();
            else if (isInitialized)
                webCamTexture.Play();
        }
    }
}