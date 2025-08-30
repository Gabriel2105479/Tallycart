using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("Camera Preview")]
    public RawImage cameraPreview;
    public Button captureButton;
    
    [Header("Photo Analysis UI")]
    public GameObject analysisPanel;
    public RawImage capturedPhotoDisplay;
    public TMP_InputField descriptionInput;
    public Button analyzeButton;
    public Button retakeButton;
    
    [Header("Results UI")]
    public GameObject resultsPanel;
    public RawImage resultPhotoDisplay;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI aiResponseText;
    public Button backButton;
    public Button saveButton;
    
    [Header("Loading UI")]
    public GameObject loadingPanel;
    public TextMeshProUGUI loadingText;
    public Slider loadingProgress;
    
    [Header("Error UI")]
    public GameObject errorPanel;
    public TextMeshProUGUI errorText;
    public Button errorOkButton;

    [Header("Gallery")]
    public GameObject galleryPanel;
    public ScrollRect galleryScrollRect;
    public Transform galleryContent;
    public GameObject galleryItemPrefab;
    public Button galleryButton;
    public Button galleryBackButton;

    private CameraController cameraController;
    private AIService aiService;
    private PhotoData currentPhoto;
    private List<PhotoData> photoGallery = new List<PhotoData>();

    void Start()
    {
        // Get components
        cameraController = FindObjectOfType<CameraController>();
        aiService = FindObjectOfType<AIService>();

        // Initialize UI
        InitializeUI();
        SetupEventListeners();
    }

    void InitializeUI()
    {
        // Hide all panels initially
        if (analysisPanel) analysisPanel.SetActive(false);
        if (resultsPanel) resultsPanel.SetActive(false);
        if (loadingPanel) loadingPanel.SetActive(false);
        if (errorPanel) errorPanel.SetActive(false);
        if (galleryPanel) galleryPanel.SetActive(false);

        // Set default text
        if (descriptionInput) descriptionInput.text = "";
        if (loadingText) loadingText.text = "Processing...";
        
        // Initialize capture button state
        UpdateCaptureButtonState();
    }

    void SetupEventListeners()
    {
        // Camera events
        if (cameraController != null)
        {
            cameraController.OnPhotoTaken += OnPhotoTaken;
            cameraController.OnError += OnCameraError;
        }

        // AI service events
        if (aiService != null)
        {
            aiService.OnResponseReceived += OnAIResponseReceived;
            aiService.OnError += OnAIError;
            aiService.OnLoadingStateChanged += OnLoadingStateChanged;
        }

        // Button events
        if (captureButton) captureButton.onClick.AddListener(CapturePhoto);
        if (analyzeButton) analyzeButton.onClick.AddListener(AnalyzePhoto);
        if (retakeButton) retakeButton.onClick.AddListener(RetakePhoto);
        if (backButton) backButton.onClick.AddListener(BackToCapture);
        if (saveButton) saveButton.onClick.AddListener(SavePhoto);
        if (errorOkButton) errorOkButton.onClick.AddListener(HideError);
        if (galleryButton) galleryButton.onClick.AddListener(ShowGallery);
        if (galleryBackButton) galleryBackButton.onClick.AddListener(HideGallery);
    }

    void Update()
    {
        // Update camera preview
        UpdateCameraPreview();
        UpdateCaptureButtonState();
    }

    void UpdateCameraPreview()
    {
        if (cameraController != null && cameraPreview != null)
        {
            Texture cameraTexture = cameraController.GetCameraTexture();
            if (cameraTexture != null)
            {
                cameraPreview.texture = cameraTexture;
                cameraPreview.gameObject.SetActive(true);
            }
            else
            {
                cameraPreview.gameObject.SetActive(false);
            }
        }
    }

    void UpdateCaptureButtonState()
    {
        if (captureButton != null && cameraController != null)
        {
            captureButton.interactable = cameraController.IsCameraReady() && !IsProcessing();
        }
    }

    void CapturePhoto()
    {
        if (cameraController != null && cameraController.IsCameraReady())
        {
            cameraController.TakePhoto();
        }
        else
        {
            ShowError("Camera is not ready. Please check camera permissions.");
        }
    }

    void OnPhotoTaken(Texture2D photo)
    {
        currentPhoto = new PhotoData(photo, "");
        
        // Show analysis panel
        if (analysisPanel) analysisPanel.SetActive(true);
        if (capturedPhotoDisplay) capturedPhotoDisplay.texture = photo;
        
        // Clear description input
        if (descriptionInput) descriptionInput.text = "";
        
        // Focus on description input
        if (descriptionInput) descriptionInput.Select();

        Debug.Log($"Photo captured: {photo.width}x{photo.height}");
    }

    void AnalyzePhoto()
    {
        if (currentPhoto?.photo == null)
        {
            ShowError("No photo to analyze");
            return;
        }

        if (aiService == null)
        {
            ShowError("AI service not available");
            return;
        }

        string description = descriptionInput?.text ?? "";
        if (string.IsNullOrWhiteSpace(description))
        {
            ShowError("Please enter a description");
            return;
        }

        currentPhoto.description = description;
        aiService.SendPhotoToAI(currentPhoto.photo, description);
    }

    void OnAIResponseReceived(string response)
    {
        if (currentPhoto != null)
        {
            currentPhoto.aiResponse = response;
            ShowResults();
        }
    }

    void ShowResults()
    {
        if (currentPhoto == null) return;

        // Hide analysis panel
        if (analysisPanel) analysisPanel.SetActive(false);

        // Show results panel
        if (resultsPanel) resultsPanel.SetActive(true);
        
        // Display results
        if (resultPhotoDisplay) resultPhotoDisplay.texture = currentPhoto.photo;
        if (descriptionText) descriptionText.text = $"Description: {currentPhoto.description}";
        if (aiResponseText) aiResponseText.text = currentPhoto.aiResponse;
    }

    void RetakePhoto()
    {
        // Hide analysis panel
        if (analysisPanel) analysisPanel.SetActive(false);
        
        // Clean up current photo
        if (currentPhoto?.photo != null)
        {
            Destroy(currentPhoto.photo);
        }
        currentPhoto = null;
    }

    void BackToCapture()
    {
        // Hide results panel
        if (resultsPanel) resultsPanel.SetActive(false);
        
        // Don't destroy the photo yet, user might want to save it
    }

    void SavePhoto()
    {
        if (currentPhoto != null)
        {
            photoGallery.Add(currentPhoto);
            
            // Show confirmation
            ShowError("Photo saved to gallery!", false);
            
            // Go back to capture
            BackToCapture();
            
            // Create new photo data for next capture
            currentPhoto = null;
        }
    }

    void OnLoadingStateChanged(bool isLoading)
    {
        if (loadingPanel) loadingPanel.SetActive(isLoading);
        
        // Update button states
        if (analyzeButton) analyzeButton.interactable = !isLoading;
        if (retakeButton) retakeButton.interactable = !isLoading;
    }

    void OnCameraError(string error)
    {
        ShowError($"Camera Error: {error}");
    }

    void OnAIError(string error)
    {
        ShowError($"AI Error: {error}");
    }

    void ShowError(string message, bool isError = true)
    {
        if (errorPanel && errorText)
        {
            errorText.text = message;
            errorText.color = isError ? Color.red : Color.green;
            errorPanel.SetActive(true);
        }
        
        Debug.Log($"UI Message: {message}");
    }

    void HideError()
    {
        if (errorPanel) errorPanel.SetActive(false);
    }

    void ShowGallery()
    {
        if (galleryPanel) galleryPanel.SetActive(true);
        UpdateGalleryDisplay();
    }

    void HideGallery()
    {
        if (galleryPanel) galleryPanel.SetActive(false);
    }

    void UpdateGalleryDisplay()
    {
        if (galleryContent == null || galleryItemPrefab == null) return;

        // Clear existing items
        foreach (Transform child in galleryContent)
        {
            Destroy(child.gameObject);
        }

        // Create gallery items
        foreach (PhotoData photo in photoGallery)
        {
            GameObject item = Instantiate(galleryItemPrefab, galleryContent);
            GalleryItem galleryItem = item.GetComponent<GalleryItem>();
            if (galleryItem != null)
            {
                galleryItem.Setup(photo, OnGalleryItemSelected);
            }
        }
    }

    void OnGalleryItemSelected(PhotoData photo)
    {
        currentPhoto = photo;
        ShowResults();
        HideGallery();
    }

    bool IsProcessing()
    {
        return aiService != null && aiService.IsProcessing();
    }

    // Public methods for external access
    public void SetAIApiKey(string apiKey)
    {
        if (aiService != null)
        {
            aiService.SetApiKey(apiKey);
        }
    }

    public void ClearGallery()
    {
        foreach (PhotoData photo in photoGallery)
        {
            if (photo.photo != null)
            {
                Destroy(photo.photo);
            }
        }
        photoGallery.Clear();
        UpdateGalleryDisplay();
    }

    public int GetGalleryCount()
    {
        return photoGallery.Count;
    }

    void OnDestroy()
    {
        // Clean up event listeners
        if (cameraController != null)
        {
            cameraController.OnPhotoTaken -= OnPhotoTaken;
            cameraController.OnError -= OnCameraError;
        }

        if (aiService != null)
        {
            aiService.OnResponseReceived -= OnAIResponseReceived;
            aiService.OnError -= OnAIError;
            aiService.OnLoadingStateChanged -= OnLoadingStateChanged;
        }

        // Clean up photos
        if (currentPhoto?.photo != null)
        {
            Destroy(currentPhoto.photo);
        }

        foreach (PhotoData photo in photoGallery)
        {
            if (photo.photo != null)
            {
                Destroy(photo.photo);
            }
        }
    }
}