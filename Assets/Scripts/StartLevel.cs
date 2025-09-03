using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Events; // Обязательно подключить для корутин

public class StartLevel : MonoBehaviour
{
    [Header("Панели и менеджеры")]
    [SerializeField] private GameObject _startPanel;
    private DialogueManager _dialogueManager;

    [Header("Элементы стартовой панели")]
    [SerializeField] private Image _maniacAvatarImage;
    [SerializeField] private TextMeshProUGUI _maniacNameText;
    [SerializeField] private TextMeshProUGUI _prologueText;
    [SerializeField] private Button _startButton;

    [Header("Настройки печатания")]
    [SerializeField] private float _typingSpeed = 0.05f;

    public UnityEvent OnStartButtonPressed;

    // Этот метод теперь будет запускать корутину
    public void Initialize(ManiacProfile profile, string prologue)
    {
        _startButton.gameObject.SetActive(false); // Прячем кнопку здесь
        _maniacAvatarImage.sprite = profile.maniacAvatar;
        _maniacNameText.text = profile.maniacName;
        StartCoroutine(TypePrologueText(prologue));
    }

    // Новая корутина для печатания текста пролога
    private IEnumerator TypePrologueText(string fullMessage)
    {
        // Опционально: можно добавить звук печатания
        SoundManager.instance.StartTypingSound(); 

        _prologueText.text = ""; // Очищаем текст перед началом
        foreach (char letter in fullMessage.ToCharArray())
        {
            _prologueText.text += letter;
            yield return new WaitForSeconds(_typingSpeed);
        }

        // Опционально: остановить звук печатания
        SoundManager.instance.StopTypingSound();
        _startButton.gameObject.SetActive(true);
    }

    public void TriggerStartEvent()
    {
        // Вызываем событие. Все, кто на него подписан, сработают.
        OnStartButtonPressed.Invoke();

        // Прячем стартовую панель
        gameObject.SetActive(false); // Прячем саму панель
    }
}