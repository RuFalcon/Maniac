using UnityEngine;
using System.Collections.Generic;
using GamePush;
using TMPro;

public class AvatarShopManager : MonoBehaviour
{
    [Header("Настройки UI")]
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject _avatarsStorePanel;

    [Header("Данные Аватаров")]
    [SerializeField] private List<AvatarData> allAvatars;

    private List<FetchProducts> _allShopProducts = new List<FetchProducts>();
    private List<string> _purchasedAvatarTags = new List<string>();
    private Dictionary<string, AvatarShopItem> _uiItems = new Dictionary<string, AvatarShopItem>();

    private const string AVATAR_PREFIX = "avatar_";

    //Подписка на события
    private void OnEnable()
    {
        GP_Payments.OnFetchProducts += OnFetchProducts;
        GP_Payments.OnFetchProductsError += OnFetchProductsError;
        GP_Payments.OnFetchPlayerPurchases += OnFetchPlayerPurchases;
    }
    //Отписка от событий
    private void OnDisable()
    {
        GP_Payments.OnFetchProducts -= OnFetchProducts;
        GP_Payments.OnFetchProductsError -= OnFetchProductsError;
        GP_Payments.OnFetchPlayerPurchases -= OnFetchPlayerPurchases;
    }

    private async void Start()
    {
        await GP_Init.Ready;
        _uiItems.Clear();
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        

        GP_Payments.Fetch();
    }

    public void FetchProducts() => GP_Payments.Fetch();
    public void FetchPlayerPurchases() => GP_Payments.Fetch();

    private async void OnFetchProducts(List<FetchProducts> products)
    {
        await GP_Init.Ready;
        _allShopProducts = products;
    }

    private async void OnFetchPlayerPurchases(List<FetchPlayerPurchases> purcahses)
    {
        await GP_Init.Ready;
        _purchasedAvatarTags.Clear();
        for (int i = 0; i < purcahses.Count; i++)
        {
            if (purcahses[i].tag.StartsWith(AVATAR_PREFIX))
            {
                _purchasedAvatarTags.Add(purcahses[i].tag);
            }
        }
        DisplayAvatars();
    }

    private void OnFetchProductsError() => Debug.Log("FETCH PRODUCTS: ERROR");

    private async void DisplayAvatars()
    {
        await GP_Init.Ready;
        string selectedTag = PlayerPrefs.GetString("SelectedAvatarTag", allAvatars.Count > 0 ? allAvatars[0].productTag : "");

        foreach (var avatarData in allAvatars)
        {
            if (string.IsNullOrEmpty(avatarData.productTag))
            {
                Debug.LogError("ОШИБКА ДАННЫХ: Аватар не содержит productTag", avatarData);
                continue;
            }

            if (_uiItems.ContainsKey(avatarData.productTag))
            {
                Debug.LogError("ОШИБКА ДАННЫХ: Дублирующийся productTag у аватара", avatarData);
                continue;
            }

            GameObject itemGO = Instantiate(itemPrefab, contentParent);
            AvatarShopItem itemUI = itemGO.GetComponent<AvatarShopItem>();
            itemUI.Setup(avatarData, this);
            _uiItems.Add(avatarData.productTag, itemUI);

            UpdateItemState(itemUI, avatarData, selectedTag);
        }
    }

    private void UpdateItemState(AvatarShopItem itemUI, AvatarData avatarData, string selectedTag)
    {
        if (selectedTag == avatarData.productTag)
        {
            itemUI.UpdateState(AvatarShopItem.AvatarState.Selected);
        }
        else if (avatarData.isFree || _purchasedAvatarTags.Contains(avatarData.productTag))
        {
            itemUI.UpdateState(AvatarShopItem.AvatarState.ToSelect);
        }
        else
        {
            var product = _allShopProducts.Find(p => p.tag == avatarData.productTag);
            if (product != null)
            {
                itemUI.UpdateState(AvatarShopItem.AvatarState.ToBuy, product.price.ToString() + " " + product.currencySymbol);
            }
            else
            {
                itemUI.UpdateState(AvatarShopItem.AvatarState.ToBuy, "N/A");
            }
        }
    }

    // Вызывается, когда аватар успешно куплен
    public void OnPurchaseSuccess(string productTag)
    {
        _purchasedAvatarTags.Add(productTag);
        GameStateManager.instance.SetPlayerAvatar(productTag);
        UpdateAllItemStates();
    }
    public void UpdateAllItemStates()
    {
        string selectedTag = PlayerPrefs.GetString("SelectedAvatarTag");
        foreach (var avatarData in allAvatars)
        {
            if (_uiItems.ContainsKey(avatarData.productTag))
            {
                UpdateItemState(_uiItems[avatarData.productTag], avatarData, selectedTag);
            }
        }
    }

    public void CloseAvatarsStorePanel()
    {
        _avatarsStorePanel.SetActive(false);
    }
}
