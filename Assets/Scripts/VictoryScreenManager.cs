using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using GamePush;

public class VictoryScreenManager : MonoBehaviour
{
    [Header("UI Элементы")]
    [SerializeField] private Image _maniacAvatarImage;
    [SerializeField] private TextMeshProUGUI _victoryText;
    [SerializeField] private GameObject _buttonsPanel;

    [Header("Настройки печатания")]
    [SerializeField] private float _typingSpeed = 0.05f;
    [SerializeField] private Animator _stampAnimator;

    // Метод Initialize теперь принимает готовый Sprite, а не весь профиль
    public void Initialize(Sprite maniacAvatar, string victoryMessage)
    {
        _buttonsPanel.SetActive(false);

        // Настраиваем аватар
        if (_maniacAvatarImage != null)
        {
            _maniacAvatarImage.sprite = maniacAvatar;
        }
        // Запускаем печать текста
        StartCoroutine(TypeVictoryText(victoryMessage));
    }

    private IEnumerator TypeVictoryText(string fullMessage)
    {
        SoundManager.instance.StartTypingSound();
        _victoryText.text = "";
        foreach (char letter in fullMessage.ToCharArray())
        {
            _victoryText.text += letter;
            yield return new WaitForSeconds(_typingSpeed);
        }
        SoundManager.instance.StopTypingSound();
        SoundManager.instance.PlayStampSound();
        _stampAnimator.SetTrigger("Stamp");
        ShowReview();
        _buttonsPanel.SetActive(true);

    }

    private void ShowReview()
    {
        bool result = GP_App.CanReview();
        if (GP_App.CanReview() && !GP_App.IsAlreadyReviewed() && GameStateManager.ShouldRequestReview())
        {
            GP_App.ReviewRequest();
        }
    }

    public void RestartLevel()
    {
        if (!IAPManager.instance.areAdsRemoved)
        {
            GP_Ads.ShowFullscreen();
        }
        GameStateManager.PrepareForRestart();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoHome()
    {
        if (!IAPManager.instance.areAdsRemoved)
        {
            GP_Ads.ShowFullscreen();
        }
        SceneManager.LoadScene(0);
    }
}