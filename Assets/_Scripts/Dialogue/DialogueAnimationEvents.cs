using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueAnimationEvents : MonoBehaviour
{

    NewDialogueManager dialogueManager;
    DialogueVisuals dialogueVisuals;
    // Start is called before the first frame update
    void Start()
    {
        dialogueManager = FindObjectOfType<NewDialogueManager>();
        dialogueVisuals = FindObjectOfType<DialogueVisuals>();
    }

   public void CloseCanvasEvent()
    {
        dialogueManager.CloseCanvas();
       
    }

    public void ChangeBroadcasterEvent()
    {
        dialogueManager.ChangeBroadcaster();
    }

    public void ClearCharactersEvent()
    {
        dialogueVisuals.ClearCharacters();
    }
    
}
