using GamePush;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] private GameObject _buttonsPanel;
    [SerializeField] private float _typingSpeed = 0.05f;
    [SerializeField] private TextMeshProUGUI _gameOverText;


    public void Initialize(string gameoverMessage)
    {
        _buttonsPanel.SetActive(false);
        // Запускаем печать текста
        StartCoroutine(TypeGameOverText(gameoverMessage));
    }
    private IEnumerator TypeGameOverText(string fullMessage)
    {
        SoundManager.instance.PlayGameOverSound();
        SoundManager.instance.StartTypingSound();
        _gameOverText.text = "";
        foreach (char letter in fullMessage.ToCharArray())
        {
            _gameOverText.text += letter;
            yield return new WaitForSeconds(_typingSpeed);
        }
        SoundManager.instance.StopTypingSound();

        _buttonsPanel.SetActive(true);

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
