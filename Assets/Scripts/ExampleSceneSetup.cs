using UnityEngine;

/// <summary>
/// Example script showing how to programmatically set up the camera AI system
/// This can be used as a reference for manual setup or automated scene creation
/// </summary>
public class ExampleSceneSetup : MonoBehaviour
{
    [Header("Example Configuration")]
    public string exampleApiKey = "your-openai-api-key-here";
    public string exampleApiUrl = "https://api.openai.com/v1/chat/completions";
    
    void Start()
    {
        SetupExample();
    }

    void SetupExample()
    {
        // Find the required components
        CameraController cameraController = FindObjectOfType<CameraController>();
        AIService aiService = FindObjectOfType<AIService>();
        UIManager uiManager = FindObjectOfType<UIManager>();
        AppSetup appSetup = FindObjectOfType<AppSetup>();

        // Configure the system
        if (appSetup != null && !string.IsNullOrEmpty(exampleApiKey))
        {
            appSetup.SetApiKey(exampleApiKey);
            appSetup.SetApiUrl(exampleApiUrl);
            Debug.Log("Example setup: AI service configured with example API key");
        }

        // Log setup status
        LogSetupStatus(cameraController, aiService, uiManager, appSetup);
    }

    void LogSetupStatus(CameraController cam, AIService ai, UIManager ui, AppSetup setup)
    {
        Debug.Log("=== Camera AI System Setup Status ===");
        Debug.Log($"CameraController: {(cam != null ? "✓ Found" : "✗ Missing")}");
        Debug.Log($"AIService: {(ai != null ? "✓ Found" : "✗ Missing")}");
        Debug.Log($"UIManager: {(ui != null ? "✓ Found" : "✗ Missing")}");
        Debug.Log($"AppSetup: {(setup != null ? "✓ Found" : "✗ Missing")}");
        
        if (cam != null && ai != null && ui != null && setup != null)
        {
            Debug.Log("✓ All components found! System should be ready to use.");
        }
        else
        {
            Debug.LogWarning("✗ Some components are missing. Please check the setup instructions.");
        }
    }

    // Public methods for testing individual components
    public void TestCameraCapture()
    {
        CameraController camera = FindObjectOfType<CameraController>();
        if (camera != null && camera.IsCameraReady())
        {
            camera.TakePhoto();
            Debug.Log("Test: Photo capture initiated");
        }
        else
        {
            Debug.LogWarning("Test: Camera not ready");
        }
    }

    public void TestAIService()
    {
        AIService ai = FindObjectOfType<AIService>();
        if (ai != null)
        {
            // Create a test texture (1x1 red pixel)
            Texture2D testTexture = new Texture2D(1, 1);
            testTexture.SetPixel(0, 0, Color.red);
            testTexture.Apply();

            ai.SendPhotoToAI(testTexture, "This is a test image");
            Debug.Log("Test: AI request sent");

            // Clean up
            Destroy(testTexture);
        }
        else
        {
            Debug.LogWarning("Test: AI service not found");
        }
    }
}