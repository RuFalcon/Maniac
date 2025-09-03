using UnityEngine;
using GamePush;
using TMPro;

public class SecondSeasonPanel : MonoBehaviour
{
    [SerializeField] private GameObject _panelObject; // Ссылка на саму шторку
    [SerializeField] private TextMeshProUGUI _priceText; // Текст цены на кнопке

    private void OnEnable()
    {
        // Подписываемся на событие из IAPManager
        IAPManager.OnPurchasesDataUpdated += UpdateVisuals;
        UpdateVisuals(); // Обновляем визуал сразу при включении
        FetchPrice(); // Запрашиваем цену
    }

    private void OnDisable()
    {
        // Отписываемся, чтобы избежать утечек памяти
        IAPManager.OnPurchasesDataUpdated -= UpdateVisuals;
    }

    // Этот метод будет вызываться событием
    private void UpdateVisuals()
    {
        // Спрашиваем у IAPManager, куплен ли сезон.
        // Если да - прячем шторку. Если нет - оставляем видимой.
        if (IAPManager.instance != null && IAPManager.instance.secondSeasonIsBuyed)
        {
            _panelObject.SetActive(false);
        }
        else
        {
            _panelObject.SetActive(true);
        }
    }

    // Метод для получения и отображения цены
    private async void FetchPrice()
    {
        await GP_Init.Ready;
        GP_Payments.Fetch(
            products => {
                var product = products.Find(p => p.tag == "season_2");
                if (product != null)
                {
                    _priceText.text = product.price.ToString() + " " + product.currency;
                }
                else
                {
                    _priceText.text = "N/A";
                }
            }
        );
    }

    public void PurchaseSecondSeason()
    {
        IAPManager.instance.PurchaseSecondSeason();
    }
}