using UnityEngine;
using GamePush;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement; // Подключаем GamePush

public class IAPManager : MonoBehaviour
{
    public static IAPManager instance;

    public static event Action OnPurchasesDataUpdated;

    // Наш главный флаг. Он будет 'true', если реклама отключена.
    public bool areAdsRemoved { get; private set; } = false; // По умолчанию реклама включена
    public bool secondSeasonIsBuyed { get; private set; } = false;

 

    private bool isFetchingPurchases = false;

    private void OnEnable()
    {
        GP_Payments.OnFetchProductsError += OnFetchProductsError;
        GP_Payments.OnFetchPlayerPurchases += OnFetchPlayerPurchases;
    }
    //Отписка от событий
    private void OnDisable()
    {
        GP_Payments.OnFetchProductsError -= OnFetchProductsError;
        GP_Payments.OnFetchPlayerPurchases -= OnFetchPlayerPurchases;
    }

    private async void Start()
    {
        await GP_Init.Ready;
 
    }

    void Awake()
    {
        // Настраиваем синглтон
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //Можно получить список покупок игрока через метод 
    public void FetchPlayerPurchases() => GP_Payments.Fetch();

    // Успешно получен
    private async void OnFetchPlayerPurchases(List<FetchPlayerPurchases> purchases)
    {
        await GP_Init.Ready;
        // Если уже идет запрос, не запускаем новый
        if (isFetchingPurchases) return;

        isFetchingPurchases = true;
        Debug.Log("Запрос данных о покупках игрока...");

        areAdsRemoved = false;
        secondSeasonIsBuyed = false;

        foreach (var purchase in purchases)
        {
            if (purchase.tag == "remove_ads") areAdsRemoved = true;
            if (purchase.tag == "season_2") secondSeasonIsBuyed = true;
        }
        Debug.Log("Данные о покупках обновлены.");
        OnPurchasesDataUpdated?.Invoke();
        isFetchingPurchases = false;
    }

    private void OnFetchProductsError() 
    {
        Debug.Log("FETCH PRODUCTS: ERROR");
        isFetchingPurchases = false;
    } 



    public void PurchaseRemoveAds()
    {
        if (areAdsRemoved) return;
        if (!GP_Payments.IsPaymentsAvailable()) return;

        GP_Payments.Purchase("remove_ads",
            purchase => {
                // УСПЕХ: После покупки просто перезапрашиваем все данные
                Debug.Log("Покупка 'remove_ads' успешна! Обновляем данные...");
                areAdsRemoved = true;
            }
        );
    }

    public void PurchaseSecondSeason()
    {
        if (secondSeasonIsBuyed) return;
        if (!GP_Payments.IsPaymentsAvailable()) return;

        GP_Payments.Purchase("season_2",
            purchase => {
                // УСПЕХ: После покупки просто перезапрашиваем все данные
                Debug.Log("Покупка 'season_2' успешна! Обновляем данные...");
                secondSeasonIsBuyed = true;
                string currentSceneName = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(currentSceneName);
            }
        );
    }

}