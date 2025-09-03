using UnityEngine;

[CreateAssetMenu(fileName = "New Maniac Profile", menuName = "Dialogue/Maniac Profile")]
public class ManiacProfile : ScriptableObject
{
    public string maniacName;
    public Sprite maniacAvatar;
    public string achievementTag;
}