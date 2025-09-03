using GamePush;
using TMPro;
using UnityEngine;

public class SocialManager : MonoBehaviour
{
    [SerializeField] GameObject socialPanel;
    [SerializeField] GameObject socialButton;
    string SHARE_TEXT;
    string URL;

    private async void Start()
    {
        await GP_Init.Ready;
        SHARE_TEXT = "Присоединяйся ко мне в игре 'Переписка с маньяком";
        URL = GP_App.Url();
        if (GP_Platform.Type() == Platform.VK || GP_Platform.Type() == Platform.OK)
        {
            socialButton.SetActive(true);
        }
    }

    public void OpenSocialPanel()
    {
        socialPanel.SetActive(true);
    }

    public void CloseSocialPanel()
    {
        socialPanel.SetActive(false);
    }

    public void ShareGP()
    {
        GP_Socials.Share(SHARE_TEXT, URL);
    }

    public void InviteGP()
    {
        GP_Socials.Invite(SHARE_TEXT, URL);
    }
}
