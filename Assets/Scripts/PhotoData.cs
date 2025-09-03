using System;
using UnityEngine;

[Serializable]
public class PhotoData
{
    public Texture2D photo;
    public string description;
    public string aiResponse;
    public DateTime timestamp;

    public PhotoData(Texture2D photo, string description)
    {
        this.photo = photo;
        this.description = description;
        this.timestamp = DateTime.Now;
        this.aiResponse = "";
    }
}

[Serializable]
public class AIRequest
{
    public string description;
    public string imageBase64;

    public AIRequest(string description, string imageBase64)
    {
        this.description = description;
        this.imageBase64 = imageBase64;
    }
}

[Serializable]
public class AIResponse
{
    public string response;
    public bool success;
    public string error;
}