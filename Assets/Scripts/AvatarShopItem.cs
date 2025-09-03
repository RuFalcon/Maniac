using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GamePush; // Подключаем GamePush

public class AvatarShopItem : MonoBehaviour
{
    [SerializeField] private Image avatarImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button actionButton;
    [SerializeField] private TextMeshProUGUI buttonText;

    private AvatarData _avatarData;
    private AvatarShopManager _shopManager;
    private AvatarState _currentState; // Храним текущее состояние

    public enum AvatarState { ToBuy, ToSelect, Selected }

    public void Setup(AvatarData data, AvatarShopManager manager)
    {
        _avatarData = data;
        _shopManager = manager;

        avatarImage.sprite = data.avatarSprite;
        nameText.text = data.avatarName;
        descriptionText.text = data.description;

        // Кнопка теперь будет вызывать метод прямо из этого скрипта
        actionButton.onClick.AddListener(OnButtonPressed);
    }

    public void UpdateState(AvatarState state, string price = "")
    {
        _currentState = state; // Запоминаем новое состояние
        Color parsedColor;
        switch (state)
        {
            case AvatarState.ToBuy:
                buttonText.text = price;
                actionButton.interactable = true;
                ColorUtility.TryParseHtmlString("#77D572", out parsedColor);
                actionButton.GetComponent<Image>().color = parsedColor;
                break;
            case AvatarState.ToSelect:
                buttonText.text = "ВЫБРАТЬ";
                actionButton.interactable = true;
                ColorUtility.TryParseHtmlString("#0168B7", out parsedColor);
                actionButton.GetComponent<Image>().color = parsedColor;
                break;
            case AvatarState.Selected:
                buttonText.text = "ВЫБРАН";
                actionButton.interactable = false;
                ColorUtility.TryParseHtmlString("#F8C301", out parsedColor);
                actionButton.GetComponent<Image>().color = parsedColor;
                break;
        }
    }

    // --- НОВЫЙ МЕТОД, КОТОРЫЙ ВЫЗЫВАЕТСЯ КНОПКОЙ ---
    public void OnButtonPressed()
    {
        // В зависимости от текущего состояния, выполняем разные действия
        switch (_currentState)
        {
            case AvatarState.ToBuy:
                // --- ЛОГИКА ПОКУПКИ ---
                Debug.Log("Запрос на покупку аватара: " + _avatarData.productTag);
                GP_Payments.Purchase(_avatarData.productTag,
                    productTag => {
                        // УСПЕХ: Сообщаем менеджеру, что нужно обновить весь список
                        Debug.Log("Покупка успешна: " + productTag);
                        _shopManager.OnPurchaseSuccess(productTag);
                    }
                );
                break;

            case AvatarState.ToSelect:
                // --- ЛОГИКА ВЫБОРА ---
                GameStateManager.instance.SetPlayerAvatar(_avatarData.productTag);
                Debug.Log("Выбран новый аватар: " + _avatarData.avatarName);

                // Сообщаем менеджеру, что нужно обновить весь список
                _shopManager.UpdateAllItemStates();
                break;

            case AvatarState.Selected:
                // Ничего не делаем, кнопка и так неактивна
                break;
        }
    }
}