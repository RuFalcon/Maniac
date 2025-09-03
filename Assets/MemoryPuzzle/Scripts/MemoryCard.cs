using UnityEngine;
using UnityEngine.UI;

public class MemoryCard : MonoBehaviour
{
    // ID, который будет определять пару. У одинаковых карточек будет одинаковый ID.
    public int cardID;

    [SerializeField] private GameObject cardFace; // Ссылка на "рубашку"

    // Этот метод будет вызван главным менеджером
    public void Setup(int id, Sprite faceSprite)
    {
        cardID = id;
        // Находим дочерний Image и устанавливаем ему спрайт "лица"
        cardFace.GetComponent<Image>().sprite = faceSprite;
    }

    public void FlipOpen()
    {
        // Показываем "лицо", прячем "рубашку"
        cardFace.SetActive(true);
    }

    public void FlipClose()
    {
        // Показываем "рубашку", прячем "лицо"
        cardFace.SetActive(false);
    }

    public void SetMatched()
    {
        // Делаем кнопку неинтерактивной и, возможно, полупрозрачной
        GetComponent<Button>().interactable = false;
        //GetComponent<Image>().color = new Color(1, 1, 1, 0.5f); // Делаем фон полупрозрачным
        cardFace.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
    }
}