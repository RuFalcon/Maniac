using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Store : MonoBehaviour
{
    [SerializeField] private GameObject _storePanel;
    [SerializeField] private GameObject _avatarStorePanel;
    //[SerializeField] private GameObject _backgroundStorePanel;

    [SerializeField] private GameObject _adsBlock;
    [SerializeField] private TextMeshProUGUI _adsBlockText;
    [SerializeField] private GameObject _secondSeasonBlock;
    [SerializeField] private TextMeshProUGUI _secondSeasonBlockText;

    private void Update()
    {
        if (IAPManager.instance.areAdsRemoved)
        {
            _adsBlock.GetComponent<Button>().interactable = false;
            Color parsedColor;
            ColorUtility.TryParseHtmlString("F8C301", out parsedColor);
            _adsBlock.GetComponent<Image>().color = parsedColor;
            _adsBlockText.text = "куплен";
        }
        if (IAPManager.instance.secondSeasonIsBuyed)
        {
            _secondSeasonBlock.GetComponent<Button>().interactable = false;
            Color parsedColor;
            ColorUtility.TryParseHtmlString("F8C301", out parsedColor);
            _secondSeasonBlock.GetComponent<Image>().color = parsedColor;
            _secondSeasonBlockText.text = "куплен";
        }
    }

    public void OpenAvatarStorePanel()
    {
        _avatarStorePanel.SetActive(true);
    }

    //public void OpenBackgroundStorePanel()
    //{
        //_backgroundStorePanel.SetActive(true);
    //}

    public void CloseStorePanel()
    {
        _storePanel.SetActive(false);
    }
}
