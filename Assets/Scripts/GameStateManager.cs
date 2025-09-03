using GamePush;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // <<-- ОБЯЗАТЕЛЬНО ДОБАВИТЬ

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager instance;

    // --- ДАННЫЕ ДЛЯ АВАТАРА ---
    [Header("Player Avatar Data")]
    [SerializeField] private Sprite _defaultAvatarSprite; // Спрайт аватара по умолчанию
    [SerializeField] private List<AvatarData> _allAvatarAssets; // Список ВСЕХ ассетов AvatarData
    private const string SELECTED_AVATAR_TAG_KEY = "SelectedAvatarTag";

    public Sprite currentPlayerAvatar { get; private set; }

    // Эту переменную мы будем использовать для сравнения
    private static string _sceneToRestart = "";

    // --- НОВЫЕ ПЕРЕМЕННЫЕ ---
    private static int _maniacsDefeatedCount = 0; // Счетчик побед
    private const int REVIEW_TRIGGER_COUNT = 3; // Показываем окно после каждых 3 побед


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadPlayerAvatar();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Загружает сохраненный выбор аватара
    private async void LoadPlayerAvatar()
    {
        await GP_Init.Ready;
        // Получаем тег сохраненного аватара. Если его нет, используем тег первого аватара в списке.
        string savedTag = PlayerPrefs.GetString(SELECTED_AVATAR_TAG_KEY, _allAvatarAssets[0].productTag);

        // Находим ассет AvatarData по тегу
        AvatarData selectedAvatarData = _allAvatarAssets.Find(avatar => avatar.productTag == savedTag);

        if (selectedAvatarData != null)
        {
            currentPlayerAvatar = selectedAvatarData.avatarSprite;
        }
        else
        {
            // Если что-то пошло не так, ставим дефолтный
            currentPlayerAvatar = _defaultAvatarSprite;
        }
        Debug.Log("Аватар игрока загружен: " + savedTag);
    }

    // Устанавливает новый аватар и сохраняет выбор
    public void SetPlayerAvatar(string avatarTag)
    {
        // Сохраняем тег
        PlayerPrefs.SetString(SELECTED_AVATAR_TAG_KEY, avatarTag);
        PlayerPrefs.Save();

        // Обновляем текущий спрайт
        LoadPlayerAvatar();
    }

    // Этот метод будет вызываться при каждой победе
    public static void IncrementManiacsDefeated()
    {
        _maniacsDefeatedCount++;
        Debug.Log("Количество побежденных маньяков: " + _maniacsDefeatedCount);
    }

    // Этот метод проверяет, пора ли показывать окно оценки
    public static bool ShouldRequestReview()
    {
        // Если счетчик не равен нулю и делится на 3 без остатка
        //if (_maniacsDefeatedCount > 0 && _maniacsDefeatedCount % REVIEW_TRIGGER_COUNT == 0)
        if(SceneManager.GetActiveScene().buildIndex % REVIEW_TRIGGER_COUNT == 0)
        {
            return true;
        }
        return false;
    }

    // Новый публичный метод, который будет вызываться перед перезагрузкой
    public static void PrepareForRestart()
    {
        // Запоминаем имя текущей сцены
        _sceneToRestart = SceneManager.GetActiveScene().name;
    }

    // Новый публичный метод для проверки, является ли запуск рестартом
    public static bool IsRestart()
    {
        // Если имя текущей сцены совпадает с той, что мы запомнили - это рестарт
        if (SceneManager.GetActiveScene().name == _sceneToRestart)
        {
            // Сбрасываем имя, чтобы следующий запуск этой же сцены (из меню) был "первым"
            _sceneToRestart = "";
            return true;
        }

        // Если имена не совпали - это первый запуск нового уровня
        return false;
    }

    // Сохраняет ID последнего достигнутого узла для текущего уровня
    public static void SaveCheckpoint(string levelId, string nodeId)
    {
        // Создаем уникальный ключ вида "Checkpoint_riddler_defeated"
        string key = "Checkpoint_" + levelId;
        PlayerPrefs.SetString(key, nodeId);
        PlayerPrefs.Save();
        Debug.Log("Чекпойнт сохранен: " + key + " = " + nodeId);
    }

    // Загружает ID узла для текущего уровня. Возвращает null, если чекпойнта нет.
    public static string LoadCheckpoint(string levelId)
    {
        string key = "Checkpoint_" + levelId;
        if (PlayerPrefs.HasKey(key))
        {
            return PlayerPrefs.GetString(key);
        }
        return null;
    }

    // Удаляет чекпойнт для текущего уровня (вызывается после победы)
    public static void ClearCheckpoint(string levelId)
    {
        if (string.IsNullOrEmpty(levelId)) return;

        string key = "Checkpoint_" + levelId;

        // Проверяем, есть ли такой ключ, перед удалением
        if (PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.DeleteKey(key);
            // PlayerPrefs.Save(); // Необязательно вызывать Save() сразу, можно при выходе из игры
            Debug.Log("Чекпойнт для уровня '" + levelId + "' успешно удален.");
        }
    }
}