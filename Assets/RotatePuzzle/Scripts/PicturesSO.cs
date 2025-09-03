using UnityEngine;

[CreateAssetMenu()]
public class PicturesSO : ScriptableObject
{
    [SerializeField] Sprite[] _images;
    [SerializeField] public bool _puzzleComlete = false;

    public Sprite[] GetImages()
    {
        return _images;
    }

    public bool GetPuzzleComplete()
    {
        return _puzzleComlete;
    }
}
