using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AppSetup : MonoBehaviour
{
    [Header("Setup Configuration")]
    [SerializeField] private string defaultApiKey = "";
    [SerializeField] private string defaultApiUrl = "https://api.openai.com/v1/chat/completions";
    
    [Header("Setup UI (Optional)")]
    public GameObject setupPanel;
    public TMP_InputField apiKeyInput;
    public TMP_InputField apiUrlInput;
    public Button saveSetupButton;
    public Button useDefaultsButton;

    private AIService aiService;
    private const string API_KEY_PREF = "AIApiKey";
    private const string API_URL_PREF = "AIApiUrl";

    void Start()
    {
        aiService = FindObjectOfType<AIService>();
        
        // Load saved preferences
        LoadSavedSettings();
        
        // Setup UI if available
        SetupUI();
        
        // Configure AI service
        ConfigureAIService();
    }

    void LoadSavedSettings()
    {
        // Load from PlayerPrefs
        string savedApiKey = PlayerPrefs.GetString(API_KEY_PREF, defaultApiKey);
        string savedApiUrl = PlayerPrefs.GetString(API_URL_PREF, defaultApiUrl);

        if (apiKeyInput) apiKeyInput.text = savedApiKey;
        if (apiUrlInput) apiUrlInput.text = savedApiUrl;
    }

    void SetupUI()
    {
        if (saveSetupButton) saveSetupButton.onClick.AddListener(SaveSettings);
        if (useDefaultsButton) useDefaultsButton.onClick.AddListener(UseDefaults);
        
        // Show setup panel if no API key is configured
        if (setupPanel && string.IsNullOrEmpty(PlayerPrefs.GetString(API_KEY_PREF, "")))
        {
            setupPanel.SetActive(true);
        }
        else if (setupPanel)
        {
            setupPanel.SetActive(false);
        }
    }

    void ConfigureAIService()
    {
        if (aiService != null)
        {
            string apiKey = PlayerPrefs.GetString(API_KEY_PREF, defaultApiKey);
            string apiUrl = PlayerPrefs.GetString(API_URL_PREF, defaultApiUrl);
            
            aiService.SetApiKey(apiKey);
            aiService.SetApiUrl(apiUrl);
            
            Debug.Log("AI Service configured");
        }
    }

    public void SaveSettings()
    {
        string apiKey = apiKeyInput?.text ?? defaultApiKey;
        string apiUrl = apiUrlInput?.text ?? defaultApiUrl;

        // Save to PlayerPrefs
        PlayerPrefs.SetString(API_KEY_PREF, apiKey);
        PlayerPrefs.SetString(API_URL_PREF, apiUrl);
        PlayerPrefs.Save();

        // Update AI service
        ConfigureAIService();

        // Hide setup panel
        if (setupPanel) setupPanel.SetActive(false);

        Debug.Log("Settings saved successfully");
    }

    public void UseDefaults()
    {
        if (apiKeyInput) apiKeyInput.text = defaultApiKey;
        if (apiUrlInput) apiUrlInput.text = defaultApiUrl;
    }

    public void ShowSetupPanel()
    {
        if (setupPanel) setupPanel.SetActive(true);
    }

    public void HideSetupPanel()
    {
        if (setupPanel) setupPanel.SetActive(false);
    }

    // Public methods for runtime configuration
    public void SetApiKey(string apiKey)
    {
        PlayerPrefs.SetString(API_KEY_PREF, apiKey);
        ConfigureAIService();
    }

    public void SetApiUrl(string apiUrl)
    {
        PlayerPrefs.SetString(API_URL_PREF, apiUrl);
        ConfigureAIService();
    }

    public string GetCurrentApiKey()
    {
        return PlayerPrefs.GetString(API_KEY_PREF, defaultApiKey);
    }

    public string GetCurrentApiUrl()
    {
        return PlayerPrefs.GetString(API_URL_PREF, defaultApiUrl);
    }
}