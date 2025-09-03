using UnityEngine;

[CreateAssetMenu(fileName = "New Avatar", menuName = "Shop/Avatar Data")]
public class AvatarData : ScriptableObject
{
    public string avatarName;
    [TextArea] public string description;
    public Sprite avatarSprite;

    [Header("Shop Settings")]
    public bool isFree = false; // Бесплатный ли этот аватар?
    public string productTag; // Тег товара в GamePush (например, "avatar_noir_detective")
}