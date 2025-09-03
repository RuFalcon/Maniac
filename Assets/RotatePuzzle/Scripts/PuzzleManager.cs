using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GamePush;

public class PuzzleManager : MonoBehaviour
{
    private System.Random _random = new System.Random();
    [SerializeField] GameObject[] _puzzles;
    public bool _isCorrect;
    private PicturesSO _currentPuzzle;
    private int _index = 0;

    [SerializeField] PicturesSO[] _puzzleContainers;
    [SerializeField] TextMeshProUGUI _titleText;
    [SerializeField] GameObject _buttonNextLevel;

    private void Start()
    {
        Shuffle(_puzzleContainers);

        _currentPuzzle = _puzzleContainers[_index];
        for (int i = 0; i < _puzzles.Length; i++)
        {
            _puzzles[i].GetComponent<Image>().sprite = _currentPuzzle.GetImages()[i];
        }
    }

    private void Update()
    {
        if (!_isCorrect)
        {
            _currentPuzzle._puzzleComlete = true;
            foreach (GameObject item in _puzzles)
            {
                if (item.transform.rotation.z > 0.01 || item.transform.rotation.z < -0.01)
                {
                    _currentPuzzle._puzzleComlete = false;
                    break;
                }
            }

            if (_currentPuzzle._puzzleComlete == true)
            {
                SolvePuzzle();
            }
        }
    }

    private void SolvePuzzle()
    {
        SoundManager.instance.PlayChimeSound();
        LevelIncrease();
        _isCorrect = true;
        _buttonNextLevel.SetActive(true);
        _titleText.text = "Победа!";

    }

    public void LevelIncrease()
    {
        if (GP_Player.GetInt("maniac_level") <= SceneManager.GetActiveScene().buildIndex)
        {
            GP_Player.Set("maniac_level", SceneManager.GetActiveScene().buildIndex);
            GP_Player.Sync();
            GameManager.available_level = GP_Player.GetInt("maniac_level");
        }
    }

    public void GoHome()
    {
        if (!IAPManager.instance.areAdsRemoved)
        {
            GP_Ads.ShowFullscreen();
        } 
        SceneManager.LoadScene(0);
    }

    void Shuffle(PicturesSO[] array)
    {
        int p = array.Length;
        for (int n = p - 1; n > 0; n--)
        {
            int r = _random.Next(0, n);
            PicturesSO t = array[r];
            array[r] = array[n];
            array[n] = t;
        }
    }
}
