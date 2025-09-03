using UnityEngine;
using UnityEngine.UI;

public class FactCard : MonoBehaviour
{
    // ID пары. У связанных карточек (маньяк и его предмет) должен быть одинаковый ID.
    // Например, "Загадочник" = 1, и "Карта" = 1.
    public int pairID;
    [SerializeField] private Image _iconImage;

    private Button button;
    private bool isMatched = false; // Флаг, что пара уже найдена

    void Awake()
    {
        button = GetComponent<Button>();
        // При нажатии на кнопку, вызываем метод SelectCard у нашего менеджера
        button.onClick.AddListener(OnCardSelected);
    }

    private void OnCardSelected()
    {
        // Если пара уже найдена, ничего не делаем
        if (isMatched)
        {
            return;
        }

        // Если все в порядке, сообщаем менеджеру о выборе
        ConnectFactsManager.instance.SelectCard(this);
    }

    // Метод для "блокировки" карточки, когда пара найдена
    public void SetMatched()
    {
        isMatched = true;
        //button.interactable = false;
        _iconImage.color = Color.gray;
        transform.Find("PushpinImage").gameObject.SetActive(true);
        // Можно изменить цвет, чтобы показать, что пара найдена
        GetComponent<Image>().color = Color.gray;
    }
}