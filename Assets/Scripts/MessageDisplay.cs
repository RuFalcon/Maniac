using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MessageDisplay : MonoBehaviour
{
    public Image avatarImage;
    public TextMeshProUGUI messageText;

    // Этот метод будет настраивать наш префаб
    public void Setup(Sprite avatar, string message)
    {
        if (avatarImage != null)
            avatarImage.sprite = avatar;

        if (messageText != null)
            messageText.text = message;
    }

    public void SetupAvatar(Sprite avatar)
    {
        if (avatarImage != null)
            avatarImage.sprite = avatar;
    }
}