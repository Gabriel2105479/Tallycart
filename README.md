# TallyCart - Unity Camera AI Integration

A comprehensive Unity package for camera access, high-quality photo capture, and AI-powered image analysis.

## 🚀 Features

- **High-Quality Camera Access**: Cross-platform camera integration with permission handling
- **AI Image Analysis**: Integration with OpenAI GPT-4 Vision and custom AI models  
- **User-Friendly UI**: Complete interface for photo capture, description input, and result display
- **Photo Gallery**: Save and browse captured photos with AI responses
- **Cross-Platform**: Works on Android, iOS, Windows, and macOS
- **Modular Design**: Use individual components or the complete system

## 📋 Quick Start

1. **Import Scripts**: Copy all files from `Assets/Scripts/` to your Unity project
2. **Setup Scene**: Follow the detailed instructions in `SETUP_INSTRUCTIONS.md`
3. **Configure AI**: Set your OpenAI API key or configure custom AI endpoint
4. **Build & Test**: Deploy to your target platform and test camera functionality

## 📂 Included Scripts

### Core Components
- **`CameraController.cs`**: Handles camera access, permissions, and photo capture
- **`AIService.cs`**: Manages AI API communication and image analysis
- **`UIManager.cs`**: Controls the user interface and user interactions
- **`AppSetup.cs`**: Handles configuration and API key management

### Supporting Classes
- **`PhotoData.cs`**: Data models for photo information and AI requests/responses
- **`GalleryItem.cs`**: UI component for photo gallery items
- **`ExampleSceneSetup.cs`**: Example setup and testing utilities

## 🛠️ Requirements

- Unity 2021.3 LTS or newer
- TextMeshPro package
- Camera permissions for target platforms
- Internet access for AI functionality
- OpenAI API key (or custom AI endpoint)

## 📱 Platform Support

### Android
- Minimum API Level 24
- Camera and Internet permissions required
- Automatic permission requests

### iOS  
- iOS 12.0 or higher
- Camera usage description required
- Automatic permission handling

### Desktop
- Windows 10/11, macOS 10.15+
- Uses main camera or webcam
- No additional permissions needed

## 🔧 Configuration

### OpenAI Setup (Default)
```csharp
AppSetup appSetup = FindObjectOfType<AppSetup>();
appSetup.SetApiKey("your-openai-api-key");
```

### Custom AI Service
```csharp
AIService aiService = FindObjectOfType<AIService>();
aiService.SendToCustomAI(photo, description, "https://your-ai-endpoint.com/analyze");
```

## 📖 Documentation

- **[Complete Setup Guide](SETUP_INSTRUCTIONS.md)**: Detailed step-by-step setup instructions
- **Code Comments**: All scripts include comprehensive inline documentation
- **Example Implementation**: Reference setup in `ExampleSceneSetup.cs`

## 🎯 Usage Example

```csharp
// Take a photo
CameraController camera = FindObjectOfType<CameraController>();
camera.TakePhoto();

// Send to AI (triggered by UI or programmatically)
AIService ai = FindObjectOfType<AIService>();
ai.SendPhotoToAI(photoTexture, "Describe what you see in this image");
```

## 🚨 Important Notes

- **API Keys**: Never commit API keys to version control
- **Permissions**: Test camera permissions on target devices
- **Network**: Ensure stable internet connection for AI features
- **Performance**: Consider image compression for large photos

## 🔐 Security Considerations

- API keys are stored securely using Unity's PlayerPrefs
- Images are processed locally before sending to AI
- Network requests include proper error handling
- No sensitive data is logged in production builds

## 🐛 Troubleshooting

### Camera Issues
- Verify camera permissions granted
- Check device has functional camera
- Try different resolution settings
- Review Unity Console for error messages

### AI Integration Issues  
- Confirm API key is valid and has sufficient credits
- Verify internet connectivity
- Check API endpoint URL format
- Review response format matches expected structure

### UI Problems
- Ensure all UI references are assigned in UIManager
- Verify Canvas has GraphicRaycaster component
- Check that EventSystem exists in scene
- Confirm TextMeshPro package is imported

## 🤝 Contributing

This is a modular system designed for easy extension:

1. **Custom AI Providers**: Extend `AIService.cs` with new API integrations
2. **UI Enhancements**: Add new panels and features to `UIManager.cs`  
3. **Camera Features**: Extend `CameraController.cs` with filters or effects
4. **Platform Support**: Add platform-specific optimizations

## 📄 License

This project is provided as-is for educational and development purposes. Please ensure compliance with any AI service terms of use and platform-specific requirements.

## 🆘 Support

For setup issues:
1. Check the detailed `SETUP_INSTRUCTIONS.md`
2. Review Unity Console for error messages
3. Verify all component references are properly assigned
4. Test individual components using `ExampleSceneSetup.cs`

For AI integration issues:
1. Verify API key and endpoint configuration
2. Check network connectivity and API service status
3. Review request/response format in browser dev tools
4. Test with minimal example data first

---

**Happy coding! 📸🤖**