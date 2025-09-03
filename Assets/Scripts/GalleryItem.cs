using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GalleryItem : MonoBehaviour
{
    [Header("UI Components")]
    public RawImage thumbnailImage;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI timestampText;
    public Button selectButton;

    private PhotoData photoData;
    private System.Action<PhotoData> onSelected;

    public void Setup(PhotoData photo, System.Action<PhotoData> onSelectedCallback)
    {
        photoData = photo;
        onSelected = onSelectedCallback;

        // Set thumbnail
        if (thumbnailImage && photo.photo != null)
        {
            thumbnailImage.texture = photo.photo;
        }

        // Set description (truncated)
        if (descriptionText)
        {
            string desc = photo.description;
            if (desc.Length > 50)
            {
                desc = desc.Substring(0, 47) + "...";
            }
            descriptionText.text = desc;
        }

        // Set timestamp
        if (timestampText)
        {
            timestampText.text = photo.timestamp.ToString("MM/dd HH:mm");
        }

        // Setup button
        if (selectButton)
        {
            selectButton.onClick.AddListener(OnSelectClicked);
        }
    }

    void OnSelectClicked()
    {
        onSelected?.Invoke(photoData);
    }
}