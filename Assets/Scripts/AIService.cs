using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class AIService : MonoBehaviour
{
    [Header("AI Service Configuration")]
    [SerializeField] private string aiApiUrl = "https://api.openai.com/v1/chat/completions";
    [SerializeField] private string apiKey = ""; // Set this in inspector or through code
    [SerializeField] private string model = "gpt-4-vision-preview";
    [SerializeField] private int maxTokens = 300;
    [SerializeField] private float temperature = 0.7f;

    public event System.Action<string> OnResponseReceived;
    public event System.Action<string> OnError;
    public event System.Action<bool> OnLoadingStateChanged;

    private bool isProcessing = false;

    [System.Serializable]
    public class OpenAIMessage
    {
        public string role;
        public OpenAIContent[] content;

        public OpenAIMessage(string role, string text, string imageBase64 = null)
        {
            this.role = role;
            if (string.IsNullOrEmpty(imageBase64))
            {
                content = new OpenAIContent[] { new OpenAIContent("text", text) };
            }
            else
            {
                content = new OpenAIContent[] 
                {
                    new OpenAIContent("text", text),
                    new OpenAIContent("image_url", null, new OpenAIImageUrl($"data:image/jpeg;base64,{imageBase64}"))
                };
            }
        }
    }

    [System.Serializable]
    public class OpenAIContent
    {
        public string type;
        public string text;
        public OpenAIImageUrl image_url;

        public OpenAIContent(string type, string text, OpenAIImageUrl imageUrl = null)
        {
            this.type = type;
            this.text = text;
            this.image_url = imageUrl;
        }
    }

    [System.Serializable]
    public class OpenAIImageUrl
    {
        public string url;

        public OpenAIImageUrl(string url)
        {
            this.url = url;
        }
    }

    [System.Serializable]
    public class OpenAIRequest
    {
        public string model;
        public OpenAIMessage[] messages;
        public int max_tokens;
        public float temperature;

        public OpenAIRequest(string model, OpenAIMessage[] messages, int maxTokens, float temperature)
        {
            this.model = model;
            this.messages = messages;
            this.max_tokens = maxTokens;
            this.temperature = temperature;
        }
    }

    [System.Serializable]
    public class OpenAIResponse
    {
        public OpenAIChoice[] choices;
        public OpenAIError error;
    }

    [System.Serializable]
    public class OpenAIChoice
    {
        public OpenAIMessage message;
    }

    [System.Serializable]
    public class OpenAIError
    {
        public string message;
        public string type;
    }

    public void SendPhotoToAI(Texture2D photo, string description)
    {
        if (isProcessing)
        {
            OnError?.Invoke("AI service is already processing a request");
            return;
        }

        if (string.IsNullOrEmpty(apiKey))
        {
            OnError?.Invoke("API key not set. Please configure the AI service.");
            return;
        }

        StartCoroutine(ProcessAIRequest(photo, description));
    }

    private IEnumerator ProcessAIRequest(Texture2D photo, string description)
    {
        isProcessing = true;
        OnLoadingStateChanged?.Invoke(true);

        try
        {
            // Convert texture to base64
            string base64Image = ConvertToBase64(photo);
            
            // Create the prompt
            string prompt = $"Analyze this image. User description: {description}\n\nPlease provide a detailed analysis of what you see in the image, relating it to the user's description. Be descriptive and helpful.";

            // Create OpenAI request
            OpenAIMessage message = new OpenAIMessage("user", prompt, base64Image);
            OpenAIRequest request = new OpenAIRequest(model, new OpenAIMessage[] { message }, maxTokens, temperature);

            string jsonRequest = JsonUtility.ToJson(request);
            
            using (UnityWebRequest webRequest = new UnityWebRequest(aiApiUrl, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonRequest);
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                
                webRequest.SetRequestHeader("Content-Type", "application/json");
                webRequest.SetRequestHeader("Authorization", $"Bearer {apiKey}");

                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    string responseText = webRequest.downloadHandler.text;
                    ParseAIResponse(responseText);
                }
                else
                {
                    string errorMessage = $"Request failed: {webRequest.error}";
                    if (!string.IsNullOrEmpty(webRequest.downloadHandler.text))
                    {
                        errorMessage += $"\nResponse: {webRequest.downloadHandler.text}";
                    }
                    OnError?.Invoke(errorMessage);
                }
            }
        }
        catch (Exception e)
        {
            OnError?.Invoke($"Error processing AI request: {e.Message}");
        }
        finally
        {
            isProcessing = false;
            OnLoadingStateChanged?.Invoke(false);
        }
    }

    private void ParseAIResponse(string responseJson)
    {
        try
        {
            OpenAIResponse response = JsonUtility.FromJson<OpenAIResponse>(responseJson);
            
            if (response.error != null)
            {
                OnError?.Invoke($"AI API Error: {response.error.message}");
                return;
            }

            if (response.choices != null && response.choices.Length > 0)
            {
                string aiResponse = response.choices[0].message.content[0].text;
                OnResponseReceived?.Invoke(aiResponse);
            }
            else
            {
                OnError?.Invoke("No response received from AI service");
            }
        }
        catch (Exception e)
        {
            OnError?.Invoke($"Error parsing AI response: {e.Message}");
        }
    }

    private string ConvertToBase64(Texture2D texture)
    {
        // Resize image if too large (to reduce API costs and improve performance)
        Texture2D resizedTexture = ResizeTexture(texture, 1024, 1024);
        
        byte[] imageBytes = resizedTexture.EncodeToJPG(85); // 85% quality
        
        // Clean up if we created a resized texture
        if (resizedTexture != texture)
        {
            Destroy(resizedTexture);
        }
        
        return Convert.ToBase64String(imageBytes);
    }

    private Texture2D ResizeTexture(Texture2D source, int maxWidth, int maxHeight)
    {
        if (source.width <= maxWidth && source.height <= maxHeight)
        {
            return source;
        }

        float aspectRatio = (float)source.width / source.height;
        int newWidth, newHeight;

        if (aspectRatio > 1.0f) // Landscape
        {
            newWidth = maxWidth;
            newHeight = Mathf.RoundToInt(maxWidth / aspectRatio);
        }
        else // Portrait or square
        {
            newHeight = maxHeight;
            newWidth = Mathf.RoundToInt(maxHeight * aspectRatio);
        }

        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        Graphics.Blit(source, rt);
        
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;
        
        Texture2D resized = new Texture2D(newWidth, newHeight);
        resized.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        resized.Apply();
        
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);
        
        return resized;
    }

    public void SetApiKey(string key)
    {
        apiKey = key;
    }

    public void SetApiUrl(string url)
    {
        aiApiUrl = url;
    }

    public bool IsProcessing()
    {
        return isProcessing;
    }

    // Alternative method for other AI services (like local models or different APIs)
    public void SendToCustomAI(Texture2D photo, string description, string customEndpoint)
    {
        StartCoroutine(ProcessCustomAIRequest(photo, description, customEndpoint));
    }

    private IEnumerator ProcessCustomAIRequest(Texture2D photo, string description, string endpoint)
    {
        isProcessing = true;
        OnLoadingStateChanged?.Invoke(true);

        try
        {
            // Simple JSON structure for custom AI endpoints
            AIRequest request = new AIRequest(description, ConvertToBase64(photo));
            string jsonRequest = JsonUtility.ToJson(request);

            using (UnityWebRequest webRequest = new UnityWebRequest(endpoint, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonRequest);
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");

                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    AIResponse response = JsonUtility.FromJson<AIResponse>(webRequest.downloadHandler.text);
                    if (response.success)
                    {
                        OnResponseReceived?.Invoke(response.response);
                    }
                    else
                    {
                        OnError?.Invoke(response.error ?? "Unknown error from AI service");
                    }
                }
                else
                {
                    OnError?.Invoke($"Request failed: {webRequest.error}");
                }
            }
        }
        catch (Exception e)
        {
            OnError?.Invoke($"Error processing custom AI request: {e.Message}");
        }
        finally
        {
            isProcessing = false;
            OnLoadingStateChanged?.Invoke(false);
        }
    }
}