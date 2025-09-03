using UnityEngine;
using GamePush;
using System.Linq; // Убедитесь, что GamePush SDK подключен

public class SceneController : MonoBehaviour
{
    [Header("Данные уровня")]
    [SerializeField] private ManiacProfile _currentManiacProfile;
    [SerializeField][TextArea] private string _prologueText;
    [SerializeField] private DialogueNode _startDialogueNode; // Стартовый узел, если нет чекпойнта

    [Header("Ссылки на компоненты сцены")]
    [SerializeField] private GameObject _startLevelPanelObject; // Ссылка на GameObject стартовой панели
    private StartLevel _startLevelScript;
    private DialogueManager _dialogueManager;

    // В Awake находим ссылки на другие компоненты на сцене
    private void Awake()
    {
        // Используем FindObjectOfType, так как они должны быть одни на сцене
        _dialogueManager = FindObjectOfType<DialogueManager>();
        _startLevelScript = FindObjectOfType<StartLevel>();
    }

    // В Start запускаем всю основную логику
    async void Start()
    {
        // Ждем инициализации GamePush, чтобы избежать ошибок
        await GP_Init.Ready;

        // 1. Отправляем аналитику о начале уровня
        if (_currentManiacProfile != null && !string.IsNullOrEmpty(_currentManiacProfile.achievementTag))
        {
            GP_Analytics.Goal("LEVEL_START", _currentManiacProfile.achievementTag);
            Debug.Log("Отправлена цель в Метрику: LEVEL_START со значением " + _currentManiacProfile.achievementTag);
        }
        else
        {
            Debug.LogWarning("Не удалось отправить цель в Метрику: профиль маньяка или его тег не назначен.");
        }

        // 2. Передаем ID уровня в DialogueManager (нужно для сохранения чекпойнтов)
        if (_dialogueManager != null && _currentManiacProfile != null)
        {
            _dialogueManager.SetLevelId(_currentManiacProfile.achievementTag);
        }

        // 3. Проверяем, есть ли чекпойнт для этого уровня
        string levelId = _currentManiacProfile.achievementTag;
        string checkpointNodeId = GameStateManager.LoadCheckpoint(levelId);

        DialogueNode nodeToStart; // Узел, с которого начнется диалог

        if (checkpointNodeId != null)
        {
            // --- НАЙДЕН ЧЕКПОЙНТ ---
            Debug.Log("Найден чекпойнт: " + checkpointNodeId);
            DialogueNode checkpointNode = FindDialogueNodeById(checkpointNodeId);
            if (checkpointNode != null)
            {
                nodeToStart = checkpointNode; // Будем начинать с него
            }
            else
            {
                Debug.LogError("Не удалось найти ассет DialogueNode с именем: " + checkpointNodeId + ". Начинаем с начала.");
                nodeToStart = _startDialogueNode;
            }
        }
        else
        {
            // --- ЧЕКПОЙНТ НЕ НАЙДЕН ---
            nodeToStart = _startDialogueNode; // Будем начинать со стартового узла
        }

        // Устанавливаем стартовый узел в DialogueManager
        _dialogueManager.currentNode = nodeToStart;

        // 4. Проверяем, нужно ли показывать стартовый экран
        if (GameStateManager.IsRestart() || checkpointNodeId != null)
        {
            // --- ЭТО ПЕРЕЗАПУСК ИЛИ ЗАГРУЗКА С ЧЕКПОЙНТА ---
            _startLevelPanelObject.SetActive(false); // Прячем стартовую панель
            StartDialogueSequence(); // И сразу начинаем диалог
        }
        else
        {
            // --- ЭТО ПЕРВЫЙ ЗАПУСК УРОВНЯ ---
            _startLevelPanelObject.SetActive(true); // Показываем панель
            if (_startLevelScript != null)
            {
                _startLevelScript.Initialize(_currentManiacProfile, _prologueText);
                _startLevelScript.OnStartButtonPressed.AddListener(StartDialogueSequence);
            }
        }
    }

    // Метод для запуска диалога (вызывается либо сразу, либо по кнопке)
    private void StartDialogueSequence()
    {
        if (_dialogueManager != null)
        {
            _dialogueManager.StartDialogue();
        }
    }

    // Вспомогательный метод для поиска DialogueNode в папке Resources
    private DialogueNode FindDialogueNodeById(string nodeId)
    {
        var allNodes = Resources.LoadAll<DialogueNode>("Dialogues");
        return allNodes.FirstOrDefault(n => n.name == nodeId);
    }
}