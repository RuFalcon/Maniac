using GamePush;
using UnityEngine;

public class StoreButtonClick : MonoBehaviour
{
    public void PurchaseRemoveAds()
    {
        IAPManager.instance.PurchaseRemoveAds();
    }

    public void PurchaseSecondSeason()
    {
        IAPManager.instance.PurchaseSecondSeason();
    }
}
