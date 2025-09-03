# Unity Camera AI Integration Setup

This package provides Unity C# scripts for camera access, high-quality photo capture, and AI-powered image analysis. The system allows users to take photos with descriptions and receive AI-generated responses.

## Features

- ✅ High-quality camera access with permission handling
- ✅ Photo capture with customizable resolution
- ✅ AI integration (OpenAI GPT-4 Vision or custom APIs)
- ✅ User-friendly UI with photo gallery
- ✅ Cross-platform support (Mobile & Desktop)
- ✅ Error handling and loading states

## Quick Setup

### 1. Unity Project Requirements

- **Unity Version**: 2021.3 LTS or newer
- **Target Platforms**: Android, iOS, Windows, macOS
- **Required Unity Packages**:
  - TextMeshPro (Window → TextMeshPro → Import TMP Essential Resources)
  - UI Toolkit (should be included by default)

### 2. Scene Setup

#### Step 1: Create Main Camera
1. Ensure your scene has a Main Camera
2. The camera will be used for desktop photo capture if mobile camera is unavailable

#### Step 2: Create UI Canvas
1. Create a Canvas (Right-click in Hierarchy → UI → Canvas)
2. Set Canvas Scaler to "Scale With Screen Size"
3. Set Reference Resolution to 1920x1080

#### Step 3: Setup Core GameObjects
1. Create an empty GameObject named "CameraSystem"
2. Add the following scripts to it:
   - `CameraController.cs`
   - `AIService.cs`
   - `UIManager.cs`
   - `AppSetup.cs`

### 3. UI Hierarchy Setup

Create the following UI structure under your Canvas:

```
Canvas
├── CameraPreview (RawImage)
├── CaptureButton (Button)
├── AnalysisPanel (Panel)
│   ├── CapturedPhoto (RawImage)
│   ├── DescriptionInput (InputField - TextMeshPro)
│   ├── AnalyzeButton (Button)
│   └── RetakeButton (Button)
├── ResultsPanel (Panel)
│   ├── ResultPhoto (RawImage)
│   ├── DescriptionText (TextMeshPro - Text)
│   ├── AIResponseText (TextMeshPro - Text)
│   ├── BackButton (Button)
│   └── SaveButton (Button)
├── LoadingPanel (Panel)
│   ├── LoadingText (TextMeshPro - Text)
│   └── LoadingProgress (Slider - Optional)
├── ErrorPanel (Panel)
│   ├── ErrorText (TextMeshPro - Text)
│   └── ErrorOkButton (Button)
├── GalleryPanel (Panel)
│   ├── GalleryScrollRect (ScrollRect)
│   │   └── Content (Empty GameObject with ContentSizeFitter)
│   └── GalleryBackButton (Button)
└── GalleryButton (Button)
```

### 4. Component Configuration

#### UIManager Configuration
1. Select the CameraSystem GameObject
2. In the UIManager component, assign all the UI references:
   - Camera Preview → CameraPreview RawImage
   - Capture Button → CaptureButton
   - Analysis Panel → AnalysisPanel
   - And so on for all UI elements...

#### CameraController Configuration
1. Set Photo Width: 1920 (or desired width)
2. Set Photo Height: 1080 (or desired height)
3. Set Target FPS: 30

#### AIService Configuration
1. Set AI API URL (default: OpenAI GPT-4 Vision)
2. Configure model name and parameters
3. API key will be set at runtime through AppSetup

## 5. AI Service Setup

### Option A: OpenAI GPT-4 Vision (Recommended)
1. Get an API key from https://platform.openai.com/
2. The default setup is configured for OpenAI
3. Set your API key in the AppSetup component or through the in-app setup UI

### Option B: Custom AI Service
1. Modify the `SendToCustomAI` method in `AIService.cs`
2. Update the JSON request/response format to match your API
3. Set your custom endpoint URL

### Example Custom AI Integration:
```csharp
// Your custom AI endpoint should accept:
{
    "description": "User description text",
    "imageBase64": "base64-encoded-image-data"
}

// And return:
{
    "response": "AI generated response",
    "success": true,
    "error": null
}
```

## 6. Platform-Specific Setup

### Android
1. **Player Settings**:
   - Set minimum API level to 24 or higher
   - Enable "Internet" permission
   - Enable "Camera" permission
   
2. **Build Settings**:
   - Add current scene to "Scenes in Build"
   - Set target device to Android

### iOS
1. **Player Settings**:
   - Set minimum iOS version to 12.0 or higher
   - Add camera usage description in iOS settings
   
2. **Info.plist** (automatically handled by Unity):
   - NSCameraUsageDescription: "This app uses camera to take photos for AI analysis"

### Desktop (Windows/Mac)
1. Camera permissions are handled automatically
2. Uses Unity's main camera for photo capture if webcam unavailable

## 7. Testing

### Editor Testing
1. The system will work in Unity Editor using the main camera
2. Webcam access may be limited in editor mode
3. Use "Game" view for testing camera preview

### Device Testing
1. Build and deploy to your target device
2. Grant camera permissions when prompted
3. Test photo capture and AI integration

## 8. API Key Configuration

### Method 1: Through Code
```csharp
AppSetup appSetup = FindObjectOfType<AppSetup>();
appSetup.SetApiKey("your-api-key-here");
```

### Method 2: Through Inspector
1. Select CameraSystem GameObject
2. In AppSetup component, set "Default Api Key"
3. The key will be saved in PlayerPrefs

### Method 3: Runtime UI (If Setup Panel is configured)
1. The app will show a setup panel on first launch
2. Users can enter their API key and URL
3. Settings are automatically saved

## 9. Customization

### Photo Quality
- Modify `photoWidth` and `photoHeight` in CameraController
- Adjust JPEG quality in `ConvertToBase64` method (default: 85%)

### AI Model Settings
- Change `model`, `maxTokens`, `temperature` in AIService
- Modify the prompt in `ProcessAIRequest` method

### UI Styling
- Customize colors, fonts, and layouts in Unity's UI system
- All UI elements use standard Unity components

## 10. Troubleshooting

### Common Issues

**Camera not working:**
- Check camera permissions
- Ensure device has a camera
- Try different resolution settings

**AI requests failing:**
- Verify API key is correct
- Check internet connection
- Ensure API URL is valid
- Check Unity Console for detailed error messages

**UI not responding:**
- Verify all UI references are assigned in UIManager
- Check that Canvas has GraphicRaycaster component
- Ensure EventSystem exists in scene

**Performance issues:**
- Reduce photo resolution
- Lower camera FPS
- Optimize UI update frequency

### Debug Information
Enable detailed logging by checking Unity Console. All major operations log their status and any errors.

## 11. Extension Points

### Adding New AI Services
1. Extend the `AIService.cs` script
2. Add new methods following the pattern of `SendToCustomAI`
3. Create corresponding UI options if needed

### Custom Photo Filters
1. Modify the photo processing in `CameraController.cs`
2. Add image processing before sending to AI

### Additional UI Features
1. Add new panels to the UI hierarchy
2. Extend `UIManager.cs` with new functionality
3. Follow the existing event-driven pattern

## 12. Production Considerations

### Security
- Never hardcode API keys in builds
- Use secure storage for sensitive configuration
- Implement proper error handling for network requests

### Performance
- Implement photo compression for large images
- Consider caching responses to reduce API calls
- Profile memory usage with large photo galleries

### User Experience
- Add haptic feedback for photo capture
- Implement proper loading animations
- Provide clear error messages and recovery options

## Support

For additional help:
1. Check Unity Console for detailed error messages
2. Verify all component references are properly assigned
3. Test individual components in isolation
4. Review the demo scene setup for reference implementation

This system is designed to be modular and extensible. Each script can be used independently or as part of the complete system.