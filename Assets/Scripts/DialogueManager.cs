using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using GamePush;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    [Header("Node")]
    public DialogueNode currentNode;

    [Header("UI")]
    public ScrollRect chatScrollRect;
    public RectTransform contentPanel;
    public GameObject choicesPanel;
    public List<Button> choiceButtons;
    public GameObject failurePanel;
    public TextMeshProUGUI failureOutcomeText;
    public GameObject failureButtons;
    public VictoryScreenManager victoryScreenManager;

    [Header("Prefabs")]
    public GameObject maniacMessagePrefab;
    public GameObject playerMessagePrefab;
    public GameObject typingIndicatorPrefab;

    [Header("Settings")]
    public float typingSpeed = 0.04f;
    public float thinkingTime = 1.2f;

    [Header("UI Header")]
    public Image headerAvatarImage;
    public TextMeshProUGUI headerManiacName;

    private string _levelId;

    public void SetLevelId(string levelId)
    {
        _levelId = levelId;
    }

    public void StartDialogue()
    {
        choicesPanel.SetActive(false);
        if (currentNode != null)
        {
            SetupHeader(currentNode.maniacProfile);
            StartCoroutine(DisplayNode(currentNode));
        }
    }

    private IEnumerator DisplayNode(DialogueNode node)
    {
        yield return new WaitForSeconds(1.0f);
        // 1. Показываем сообщения маньяка
        foreach (string message in node.maniacMessages)
        {
            // Показываем индикатор "печатает..."
            SoundManager.instance.PlaySonarSound();
            GameObject indicator = Instantiate(typingIndicatorPrefab, contentPanel);
            indicator.GetComponent<MessageDisplay>().SetupAvatar(node.maniacProfile.maniacAvatar);
            StartCoroutine(ScrollToBottom());

            // Ждем, пока маньяк "думает"
            yield return new WaitForSeconds(thinkingTime);
            Destroy(indicator); // Удаляем индикатор

            // Создаем пузырь сообщения
            GameObject messageInstance = Instantiate(maniacMessagePrefab, contentPanel);

            // Получаем компонент MessageDisplay, который теперь отвечает за настройку
            MessageDisplay messageDisplay = messageInstance.GetComponent<MessageDisplay>();

            // Настраиваем аватар. Текст пока не трогаем, он будет печататься
            messageDisplay.avatarImage.sprite = node.maniacProfile.maniacAvatar;

            // Получаем ссылку на текстовое поле для эффекта печатания
            TextMeshProUGUI textComponent = messageDisplay.messageText;

            // Запускаем корутину печатания и прокрутку
            StartCoroutine(ScrollToBottom());
            yield return StartCoroutine(TypeMessage(textComponent, message));
        }

        // 2. Показываем варианты ответов игроку
        if (node.playerChoices.Count > 0)
        {
            SetupChoices(node);
        }
        else
        {
            GameStateManager.ClearCheckpoint(_levelId);
            Debug.Log("Конец ветки диалога.");
            // Здесь можно показать экран победы или поражения, если нет выбора
            StartCoroutine(Handlevictory());
        }
    }

    private void SetupChoices(DialogueNode node)
    {
        choicesPanel.SetActive(true);
        for (int i = 0; i < choiceButtons.Count; i++)
        {
            if (i < node.playerChoices.Count)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = node.playerChoices[i].choiceText;

                int choiceIndex = i;
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(node.playerChoices[choiceIndex]));
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnChoiceSelected(DialogueChoice choice)
    {
        SoundManager.instance.PlayButtonClickSound();
        SoundManager.instance.PlaySwooshSound();
        choicesPanel.SetActive(false);

        // Показываем ответ игрока
        GameObject playerMessageInstance = Instantiate(playerMessagePrefab, contentPanel);
        MessageDisplay messageDisplay = playerMessageInstance.GetComponent<MessageDisplay>();
        if (messageDisplay != null)
        {
            // Устанавливаем текст и АВАТАР ИЗ GameStateManager
            messageDisplay.Setup(GameStateManager.instance.currentPlayerAvatar, choice.choiceText);
        }
        //playerMessageInstance.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;
        StartCoroutine(ScrollToBottom());

        if (choice.isCorrectChoice)
        {
            SoundManager.instance.PlayChimeSound();
            // --- ЛОГИКА ПРАВИЛЬНОГО ОТВЕТА (остается прежней) ---
            currentNode = choice.nextNode;
            if (currentNode != null)
            {
                GameStateManager.SaveCheckpoint(_levelId, currentNode.name);
                SetupHeader(currentNode.maniacProfile);
                StartCoroutine(DisplayNode(currentNode));
            }
            else
            {
                GameStateManager.ClearCheckpoint(_levelId);
                Debug.Log("Диалог успешно завершен!");
                // Показать экран победы
                StartCoroutine(Handlevictory());
            }
        }
        else
        {
            SoundManager.instance.PlayErrorSound();
            // --- НОВАЯ ЛОГИКА НЕПРАВИЛЬНОГО ОТВЕТА ---
            StartCoroutine(HandleFailure(currentNode, choice));
        }
    }

    private void SetupHeader(ManiacProfile profile)
    {
        if (profile == null) return;
        headerAvatarImage.sprite = profile.maniacAvatar;
        headerManiacName.text = profile.maniacName;
    }

    private IEnumerator HandleFailure(DialogueNode node, DialogueChoice failedChoice)
    {
        // 1. Показываем саркастический ответ маньяка, если он есть
        if (!string.IsNullOrEmpty(failedChoice.maniacResponseOnFail))
        {
            // Показываем индикатор "печатает..."
            GameObject indicator = Instantiate(typingIndicatorPrefab, contentPanel);
            indicator.GetComponent<MessageDisplay>().SetupAvatar(node.maniacProfile.maniacAvatar);
            StartCoroutine(ScrollToBottom());
            yield return new WaitForSeconds(thinkingTime);
            Destroy(indicator);

            // Создаем пузырь сообщения
            GameObject messageInstance = Instantiate(maniacMessagePrefab, contentPanel);

            // Получаем компонент MessageDisplay
            MessageDisplay messageDisplay = messageInstance.GetComponent<MessageDisplay>();

            // Устанавливаем ПРАВИЛЬНЫЙ аватар из профиля маньяка в текущем узле
            messageDisplay.avatarImage.sprite = node.maniacProfile.maniacAvatar;

            // Получаем ссылку на текстовое поле для эффекта печатания
            TextMeshProUGUI textComponent = messageDisplay.messageText;

            // Запускаем корутину печатания и прокрутку
            StartCoroutine(ScrollToBottom());
            yield return StartCoroutine(TypeMessage(textComponent, failedChoice.maniacResponseOnFail));
        }

        // Небольшая пауза перед финалом
        yield return new WaitForSeconds(2.0f);

        // 2. Показываем экран поражения
        //failureOutcomeText.text = failedChoice.failureOutcomeText;
        failurePanel.SetActive(true);

        // Здесь можно добавить кнопку "Начать заново", которая перезагрузит сцену
        // Например: Button restartButton = failurePanel.GetComponentInChildren<Button>();
        // restartButton.onClick.AddListener(() => UnityEngine.SceneManagement.SceneManager.LoadScene(0));
        //yield return new WaitForSeconds(2.0f);
        //failureButtons.SetActive(true);

        failurePanel.GetComponent<GameOver>().Initialize(failedChoice.failureOutcomeText);
        Debug.Log("Игра окончена! Неправильный выбор.");
    }

    private IEnumerator Handlevictory()
    {
        if (currentNode != null && !string.IsNullOrEmpty(currentNode.maniacProfile.achievementTag))
        {
            // Вызываем метод GamePush для открытия достижения
            GP_Achievements.Unlock(currentNode.maniacProfile.achievementTag);

            Debug.Log("Попытка открыть достижение: " + currentNode.maniacProfile.achievementTag);
        }
        yield return new WaitForSeconds(1.5f);

        if (victoryScreenManager != null && currentNode != null)
        {
            // --- ВОТ КЛЮЧЕВОЕ ИЗМЕНЕНИЕ ---

            // 1. Получаем нужные данные из текущего узла
            Sprite avatar = currentNode.maniacProfile.maniacAvatar;
            string message = currentNode.victoryMessage;

            LevelIncrease();

            // 2. Включаем панель победы
            victoryScreenManager.gameObject.SetActive(true);

            // 3. Передаем в Initialize уже готовые данные, а не весь профиль
            victoryScreenManager.Initialize(avatar, message);
        }
        else
        {
            Debug.LogError("VictoryScreenManager или currentNode не назначен!");
        }
    }

    public void LevelIncrease()
    {
        if (GP_Player.GetInt("maniac_level") <= SceneManager.GetActiveScene().buildIndex)
        {
            GP_Player.Set("maniac_level", SceneManager.GetActiveScene().buildIndex - 1);
            GP_Player.Sync();
            GameManager.available_level = GP_Player.GetInt("maniac_level");
        }
    }

    // Корутина для эффекта печатания
    public IEnumerator TypeMessage(TextMeshProUGUI textComponent, string fullMessage)
    {
        SoundManager.instance.StartTypingSound();
        textComponent.text = "";
        foreach (char letter in fullMessage.ToCharArray())
        {
            textComponent.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        SoundManager.instance.StopTypingSound();
        
    }

    // Корутина для авто-скролла
    private IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        chatScrollRect.verticalNormalizedPosition = 0f;
    }
}