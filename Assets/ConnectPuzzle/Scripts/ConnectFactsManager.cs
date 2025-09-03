using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // Для Image (линии)
using GamePush;

public class ConnectFactsManager : MonoBehaviour
{
    public static ConnectFactsManager instance;

    [Header("Settings")]
    public GameObject linePrefab; // Префаб красной линии (просто Image)
    public Transform linesParent; // Панель, куда добавлять линии

    private FactCard firstSelectedCard;
    private FactCard secondSelectedCard;

    private int pairsFound = 0;
    private int totalPairs; // Будем вычислять автоматически

    [SerializeField] TextMeshProUGUI _titleText;
    [SerializeField] GameObject _buttonNextLevel;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // Считаем, сколько всего пар нужно найти
        FactCard[] allCards = FindObjectsOfType<FactCard>();
        totalPairs = allCards.Length / 2;
    }

    // Этот метод вызывается из FactCard, когда на карточку нажимают
    public void SelectCard(FactCard card)
    {
        SoundManager.instance.PlayThumbtackSound();
        if (firstSelectedCard == null)
        {
            // Это первая выбранная карточка
            firstSelectedCard = card;
            // Можно добавить эффект "выделения" (например, увеличить размер)
            firstSelectedCard.transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
        }
        else if (secondSelectedCard == null)
        {
            // Это вторая выбранная карточка
            secondSelectedCard = card;
            secondSelectedCard.transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);

            // Проверяем, совпадает ли пара
            CheckPair();
        }
    }

    private void CheckPair()
    {
        // Добавляем проверку, что вторая карточка - это не первая
        if (firstSelectedCard == secondSelectedCard)
        {
            // Игрок кликнул на ту же карточку дважды, сбрасываем выбор
            firstSelectedCard.transform.localScale = Vector3.one;
            firstSelectedCard = null;
            secondSelectedCard = null; // Убедимся, что secondSelectedCard тоже пуст
            return;
        }

        if (firstSelectedCard.pairID == secondSelectedCard.pairID)
        {
            SoundManager.instance.PlayChimeSound();
            // ПАРА ПРАВИЛЬНАЯ
            Debug.Log("Найдена верная пара! ID: " + firstSelectedCard.pairID);

            // Вызываем новый метод, который заблокирует обе карточки
            firstSelectedCard.SetMatched();
            secondSelectedCard.SetMatched();

            DrawLine(firstSelectedCard.transform, secondSelectedCard.transform);

            pairsFound++;
            CheckCompletion();
        }
        else
        {
            SoundManager.instance.PlayErrorSound();
            // ПАРА НЕПРАВИЛЬНАЯ
            Debug.Log("Ошибка!");
            // Возвращаем размер для обеих карточек
            firstSelectedCard.transform.localScale = Vector3.one;
            secondSelectedCard.transform.localScale = Vector3.one;
        }

        // Сбрасываем выбор
        firstSelectedCard = null;
        secondSelectedCard = null;
    }

    private void DrawLine(Transform pos1, Transform pos2)
    {
        GameObject lineInstance = Instantiate(linePrefab, linesParent);
        RectTransform lineRect = lineInstance.GetComponent<RectTransform>();

        // --- НОВЫЙ, БОЛЕЕ НАДЕЖНЫЙ КОД ---

        // 1. Преобразуем мировые координаты карточек в локальные координаты
        // относительно родительского объекта для линий (linesParent).
        // Это как бы спрашивает: "Если бы эта карточка была дочерним объектом linesParent,
        // какая у нее была бы позиция?"
        Vector2 startPoint = linesParent.InverseTransformPoint(pos1.position);
        Vector2 endPoint = linesParent.InverseTransformPoint(pos2.position);

        // 2. Вычисляем вектор и расстояние уже в локальных координатах.
        // Теперь эти значения будут правильными для UI.
        Vector2 differenceVector = endPoint - startPoint;

        // 3. Устанавливаем длину и толщину линии.
        lineRect.sizeDelta = new Vector2(differenceVector.magnitude, 5f); // 5f - толщина

        // 4. Устанавливаем позицию линии внутри родителя.
        // Используем anchoredPosition, так как это правильный способ
        // позиционирования UI-элементов внутри родителя.
        lineRect.anchoredPosition = startPoint;

        // 5. Вычисляем угол и поворачиваем.
        float angle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;
        lineRect.rotation = Quaternion.Euler(0, 0, angle);

        // Устанавливаем pivot в начало для правильного вращения
        lineRect.pivot = new Vector2(0, 0.5f);
    }

    private void CheckCompletion()
    {
        if (pairsFound == totalPairs)
        {
            Debug.Log("УРОВЕНЬ ПРОЙДЕН!");
            LevelIncrease();
            _buttonNextLevel.SetActive(true);
            _titleText.text = "Победа!";
            // Запускаем экран победы или переход дальше
        }
    }

    public void LevelIncrease()
    {
        if (GP_Player.GetInt("maniac_level") <= SceneManager.GetActiveScene().buildIndex)
        {
            GP_Player.Set("maniac_level", SceneManager.GetActiveScene().buildIndex);
            GP_Player.Sync();
            GameManager.available_level = GP_Player.GetInt("maniac_level");
        }
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