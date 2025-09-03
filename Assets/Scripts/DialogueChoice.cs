using UnityEngine;

[System.Serializable] 
public class DialogueChoice
{
    [TextArea]
    public string choiceText; 
    public DialogueNode nextNode; 
    public bool isCorrectChoice = true; 

    // Эти поля используются, ТОЛЬКО если isCorrectChoice = false
    [TextArea]
    public string maniacResponseOnFail; // Саркастический ответ маньяка
    [TextArea]
    public string failureOutcomeText;  // Текст о том, что случилось с игроком
}
