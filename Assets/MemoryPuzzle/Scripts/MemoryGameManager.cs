using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // Для перемешивания
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using GamePush;

public class MemoryGameManager : MonoBehaviour
{
    [Header("Settings")]
    public GameObject cardPrefab;
    public Transform gridParent;
    public List<Sprite> cardFaces; // Сюда нужно добавить 8 уникальных спрайтов

    private List<MemoryCard> allCards = new List<MemoryCard>();
    private List<int> cardIDs = new List<int>();

    private MemoryCard firstSelected;
    private MemoryCard secondSelected;

    private int pairsFound = 0;
    private int totalPairs;

    [SerializeField] TextMeshProUGUI _titleText;
    [SerializeField] GameObject _buttonNextLevel;

    void Start()
    {
        totalPairs = cardFaces.Count;
        GenerateCardIDs();
        ShuffleCardIDs();
        SpawnCards();
    }

    void GenerateCardIDs()
    {
        // Создаем пары ID. Если у нас 8 спрайтов, нам нужны ID: 1,1, 2,2, 3,3 ... 8,8
        for (int i = 0; i < cardFaces.Count; i++)
        {
            cardIDs.Add(i);
            cardIDs.Add(i);
        }
    }

    void ShuffleCardIDs()
    {
        // Перемешиваем список ID случайным образом
        cardIDs = cardIDs.OrderBy(x => Random.value).ToList();
    }

    void SpawnCards()
    {
        for (int i = 0; i < cardIDs.Count; i++)
        {
            GameObject cardInstance = Instantiate(cardPrefab, gridParent);

            int currentID = cardIDs[i];
            Sprite currentFace = cardFaces[currentID];

            MemoryCard card = cardInstance.GetComponent<MemoryCard>();
            card.Setup(currentID, currentFace);

            card.GetComponent<Button>().onClick.AddListener(() => OnCardClicked(card));
            allCards.Add(card);
        }
    }

    public void OnCardClicked(MemoryCard card)
    {
        SoundManager.instance.PlayPuzzleClickSound();
        // Если уже выбраны 2 карты, ничего не делаем, ждем пока они перевернутся
        if (secondSelected != null)
        {
            return;
        }

        card.FlipOpen();

        if (firstSelected == null)
        {
            firstSelected = card;
        }
        else
        {
            secondSelected = card;
            StartCoroutine(CheckForMatch());
        }
    }

    private IEnumerator CheckForMatch()
    {
        // Ждем небольшую паузу, чтобы игрок успел увидеть вторую карту
        yield return new WaitForSeconds(0.7f);

        if (firstSelected.cardID == secondSelected.cardID)
        {
            SoundManager.instance.PlayChimeSound();
            // ПАРА СОВПАЛА
            firstSelected.SetMatched();
            secondSelected.SetMatched();
            // Можно добавить звук успеха

            pairsFound++;
            CheckForGameWin();
        }
        else
        {
            SoundManager.instance.PlayErrorSound();
            // ПАРА НЕ СОВПАЛА
            firstSelected.FlipClose();
            secondSelected.FlipClose();
            // Можно добавить звук ошибки
        }

        // Сбрасываем выбор
        firstSelected = null;
        secondSelected = null;
    }

    private void CheckForGameWin()
    {
        if (pairsFound == totalPairs)
        {
            LevelIncrease();
            _buttonNextLevel.SetActive(true);
            _titleText.text = "Победа!";
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