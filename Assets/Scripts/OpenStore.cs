using UnityEngine;

public class OpenStore : MonoBehaviour
{
    [SerializeField] private GameObject _storePanel;
   
    public void OpenStorePanel()
    {
        _storePanel.SetActive(true);
    }
}
