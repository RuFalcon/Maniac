using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GamePush;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] GameObject[] allButtons;

    [SerializeField] GameObject firstGrid;
    [SerializeField] GameObject secondGrid;
    [SerializeField] GameObject thirdGrid;
    [SerializeField] GameObject fourGrid;
    [SerializeField] GameObject fithGrid;
    [SerializeField] GameObject sixthGrid;
    [SerializeField] GameObject seventhGrid;

    [SerializeField] GameObject _loadingPanel;

    private int currentPart = 1;
    private int maxPart = 7;
    private int minPart = 1;

    private void OnEnable()
    {
        GP_Init.OnReady += OnPluginReady;
    }

    private void OnDisable()
    {
        GP_Init.OnReady -= OnPluginReady;
    }

    private void OnPluginReady()
    {
        Debug.Log("Plugin ready");
        GP_Game.GameReady();

        if (GP_Platform.Type() == Platform.VK)
        {
            GP_Ads.ShowSticky();
        }
        if (!IAPManager.instance.areAdsRemoved)
        {
            //GP_Ads.ShowPreloader();
        }  
        GameManager.available_level = GP_Player.GetInt("maniac_level");
        for (int i = 0; i < allButtons.Length; i++)
        {
            if (GameManager.available_level >= i)
            {
                allButtons[i].GetComponent<Button>().interactable = true;
                allButtons[i].transform.GetChild(0).GetComponent<Image>().color = Color.white;
            }
            if (GameManager.available_level > i)
            {
                allButtons[i].transform.GetChild(3).gameObject.SetActive(true);
            }
        }
    }

    async void Start()
    {
        await GP_Init.Ready;
        _loadingPanel.SetActive(false);
        Debug.Log(GameManager.available_level);
        for (int i = 0; i < allButtons.Length; i++)
        {
            if (GameManager.available_level >= i)
            {
                allButtons[i].GetComponent<Button>().interactable = true;
                allButtons[i].transform.GetChild(0).GetComponent<Image>().color = Color.white;
            }
            if (GameManager.available_level > i)
            {
                allButtons[i].transform.GetChild(3).gameObject.SetActive(true);
            }
        }

    }

    public void onClickLevel(int levelNumber)
    {
        SceneManager.LoadScene(levelNumber);
    }

    public void NextLevel()
    {
        if (currentPart >= maxPart) return;
        currentPart++;
        switch (currentPart)
        {
            case 2:
                firstGrid.SetActive(false);
                secondGrid.SetActive(true);
                break;
            case 3:
                secondGrid.SetActive(false);
                thirdGrid.SetActive(true);
                break;
            case 4:
                thirdGrid.SetActive(false);
                fourGrid.SetActive(true);
                break;
            case 5:
                fourGrid.SetActive(false);
                fithGrid.SetActive(true);
                break;
            case 6:
                fithGrid.SetActive(false);
                sixthGrid.SetActive(true);
                break;
            case 7:
                sixthGrid.SetActive(false);
                seventhGrid.SetActive(true);
                break;
        }
    }
    public async void PrevLevel()
    {
        //await GP_Init.Ready;
        if (currentPart <= minPart) return;
        currentPart--;
        switch (currentPart)
        {
            case 1:
                secondGrid.SetActive(false);
                firstGrid.SetActive(true);
                break;
            case 2:
                thirdGrid.SetActive(false);
                secondGrid.SetActive(true);
                break;
            case 3:
                fourGrid.SetActive(false);
                thirdGrid.SetActive(true);
                break;
            case 4:
                fithGrid.SetActive(false);
                fourGrid.SetActive(true);
                break;
            case 5:
                sixthGrid.SetActive(false);
                fithGrid.SetActive(true);
                break;
            case 6:
                seventhGrid.SetActive(false);
                sixthGrid.SetActive(true);
                break;
        }
    }

    public async void ShowAchiviments()
    {
        await GP_Init.Ready;
        GP_Achievements.Open();
    }
}
