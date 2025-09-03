# Example Unity Scene Configuration

This file describes how to set up a Unity scene with the Camera AI system. While Unity scenes are binary files (.unity), this configuration can guide manual setup or automated scene generation.

## Scene Hierarchy Structure

```
SampleScene
├── Main Camera
├── Directional Light
├── EventSystem
├── Canvas
│   ├── CameraPreview (RawImage)
│   ├── UI_CaptureButton (Button)
│   ├── AnalysisPanel (Panel) [Initially Inactive]
│   │   ├── Background (Image)
│   │   ├── CapturedPhotoDisplay (RawImage)
│   │   ├── DescriptionInputField (InputField TMP)
│   │   ├── AnalyzeButton (Button)
│   │   └── RetakeButton (Button)
│   ├── ResultsPanel (Panel) [Initially Inactive]
│   │   ├── Background (Image)
│   │   ├── ResultPhotoDisplay (RawImage)
│   │   ├── DescriptionText (TextMeshPro)
│   │   ├── AIResponseText (TextMeshPro)
│   │   ├── BackButton (Button)
│   │   └── SaveButton (Button)
│   ├── LoadingPanel (Panel) [Initially Inactive]
│   │   ├── Background (Image)
│   │   ├── LoadingText (TextMeshPro)
│   │   └── LoadingProgress (Slider)
│   ├── ErrorPanel (Panel) [Initially Inactive]
│   │   ├── Background (Image)
│   │   ├── ErrorText (TextMeshPro)
│   │   └── ErrorOkButton (Button)
│   ├── GalleryPanel (Panel) [Initially Inactive]
│   │   ├── Background (Image)
│   │   ├── GalleryTitle (TextMeshPro)
│   │   ├── GalleryScrollRect (ScrollRect)
│   │   │   └── Content (Empty GameObject)
│   │   └── GalleryBackButton (Button)
│   └── GalleryButton (Button)
└── CameraSystem (Empty GameObject)
    ├── CameraController (Script)
    ├── AIService (Script)
    ├── UIManager (Script)
    └── AppSetup (Script)
```

## Component Configurations

### Canvas Settings
- **Render Mode**: Screen Space - Overlay
- **Canvas Scaler**: Scale With Screen Size
- **Reference Resolution**: 1920 x 1080
- **Screen Match Mode**: Match Width Or Height (0.5)

### CameraPreview (RawImage)
- **Anchors**: Full screen or custom layout
- **Color**: White (for visibility)
- **Texture**: Will be set dynamically by CameraController

### UI Buttons
- **Source Image**: Default UI Sprite or custom button graphics
- **Text**: Use TextMeshPro components for better text rendering
- **Colors**: Set up button state colors (Normal, Highlighted, Pressed, Disabled)

### Input Field (Description)
- **Component**: InputField (TextMeshPro)
- **Placeholder Text**: "Describe what you want to analyze..."
- **Character Limit**: 500 (recommended)
- **Content Type**: Standard
- **Line Type**: Multi Line Submit

### Text Components
- **Font**: Default TextMeshPro font or custom font asset
- **Color**: High contrast colors for readability
- **Auto Size**: Enable for responsive text sizing

### Panels
- **Background**: Semi-transparent dark color (e.g., RGBA 0,0,0,200)
- **Initially Active**: Only main UI elements should be active at start

## CameraSystem GameObject Configuration

### CameraController Component
```
Photo Width: 1920
Photo Height: 1080
Target FPS: 30
Render Texture: Create new RenderTexture asset (1920x1080x24)
```

### AIService Component
```
AI API URL: https://api.openai.com/v1/chat/completions
Model: gpt-4-vision-preview
Max Tokens: 300
Temperature: 0.7
API Key: (Set through AppSetup at runtime)
```

### UIManager Component
All UI element references must be assigned:
```
Camera Preview: → CameraPreview RawImage
Capture Button: → UI_CaptureButton
Analysis Panel: → AnalysisPanel
Captured Photo Display: → CapturedPhotoDisplay
Description Input: → DescriptionInputField
Analyze Button: → AnalyzeButton
Retake Button: → RetakeButton
Results Panel: → ResultsPanel
Result Photo Display: → ResultPhotoDisplay
Description Text: → DescriptionText
AI Response Text: → AIResponseText
Back Button: → BackButton
Save Button: → SaveButton
Loading Panel: → LoadingPanel
Loading Text: → LoadingText
Loading Progress: → LoadingProgress
Error Panel: → ErrorPanel
Error Text: → ErrorText
Error Ok Button: → ErrorOkButton
Gallery Panel: → GalleryPanel
Gallery Scroll Rect: → GalleryScrollRect
Gallery Content: → Content (inside ScrollRect)
Gallery Button: → GalleryButton
Gallery Back Button: → GalleryBackButton
```

### AppSetup Component
```
Default API URL: https://api.openai.com/v1/chat/completions
Setup Panel: (Optional - assign if you want runtime setup UI)
API Key Input: (Optional - for setup UI)
API URL Input: (Optional - for setup UI)
Save Setup Button: (Optional - for setup UI)
Use Defaults Button: (Optional - for setup UI)
```

## Build Settings

### Android
- **Minimum API Level**: 24
- **Target API Level**: 33 (or latest)
- **Permissions**: Camera, Internet (automatically added)
- **Graphics API**: OpenGLES3, Vulkan

### iOS
- **Minimum iOS Version**: 12.0
- **Camera Usage Description**: "This app uses the camera to take photos for AI analysis"
- **Architecture**: ARM64

### Windows/Mac
- **Architecture**: x86_64
- **Graphics API**: Direct3D11 (Windows), Metal (Mac)

## Testing Checklist

1. **Scene Loads**: No errors in console
2. **Camera Preview**: Shows camera feed on device
3. **Permissions**: Camera permission requested and granted
4. **Photo Capture**: Button responds and captures photo
5. **UI Navigation**: All panels show/hide correctly
6. **AI Integration**: API requests work with valid key
7. **Error Handling**: Invalid inputs show error messages
8. **Gallery**: Photos save and display in gallery

## Common Issues

- **Missing TextMeshPro**: Import TMP Essential Resources
- **UI References**: Ensure all UIManager references are assigned
- **Camera Permissions**: Test on actual device, not just editor
- **API Keys**: Set through AppSetup, not hardcoded in scripts

This configuration provides a complete, functional scene for the Camera AI system.