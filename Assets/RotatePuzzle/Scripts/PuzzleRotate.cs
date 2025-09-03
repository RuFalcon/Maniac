using UnityEngine;
using UnityEngine.EventSystems; 

// Добавляем интерфейс IPointerClickHandler
public class PuzzleRotate : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private PuzzleManager _puzzleManager;


    private void Awake()
    {
        int[] rotate = { 0, 90, 180, 270 };
        transform.Rotate(0f, 0f, rotate[Random.Range(0, rotate.Length)]);
    }

    // Вместо OnMouseDown() мы используем OnPointerClick()
    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.instance.PlayPuzzleClickSound();

        if (!_puzzleManager._isCorrect)
        {
            transform.Rotate(0f, 0f, 90f);
        }
    }

}