using GamePush;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NoAdsPrice : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _noAdsText;
    [SerializeField] private TextMeshProUGUI _secondSeasonText;
    private List<FetchProducts> _allShopProducts = new List<FetchProducts>();

    //Подписка на события
    private void OnEnable()
    {
        GP_Payments.OnFetchProducts += OnFetchProducts;
        GP_Payments.OnFetchProductsError += OnFetchProductsError;
    }
    //Отписка от событий
    private void OnDisable()
    {
        GP_Payments.OnFetchProducts -= OnFetchProducts;
        GP_Payments.OnFetchProductsError -= OnFetchProductsError;
    }

    private async void Start()
    {
        await GP_Init.Ready;
        GP_Payments.Fetch();
    }

    //Можно получить список товаров через метод 
    public void FetchProducts() => GP_Payments.Fetch();

    // Успешно получен
    private async void OnFetchProducts(List<FetchProducts> products)
    {
        await GP_Init.Ready;
        for (int i = 0; i < products.Count; i++)
        {
            if (products[i].tag == "remove_ads")
            {
                _noAdsText.text = products[i].price.ToString() + " " + products[i].currencySymbol;
            }
            if (products[i].tag == "season_2")
            {
                _secondSeasonText.text = products[i].price.ToString() + " " + products[i].currencySymbol;
            }
        }
            
    }
    // Ошибки при получении
    private void OnFetchProductsError() => Debug.Log("FETCH PRODUCTS: ERROR");
}
