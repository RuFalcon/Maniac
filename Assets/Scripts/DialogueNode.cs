using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Dialogue Node", menuName = "Dialogue/Dialogue Node")]
public class DialogueNode : ScriptableObject
{
    public ManiacProfile maniacProfile;
    // Сообщения, которые отправляет маньяк (может быть несколько подряд)
    [TextArea]
    public List<string> maniacMessages;

    // Варианты ответов, которые предлагаются игроку
    public List<DialogueChoice> playerChoices;

    [TextArea]
    public string victoryMessage;
}
