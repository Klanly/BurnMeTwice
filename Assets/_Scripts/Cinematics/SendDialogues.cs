using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendDialogues : MonoBehaviour
{
    NewDialogueManager dm;
    // Start is called before the first frame update
    void Start()
    {
        dm = FindObjectOfType<NewDialogueManager>();
    }

    public void SendDialogue(string nomenclature)
    {
        dm.LoadDialogue(nomenclature, false);
    }

    public void NextDialogue()
    {
        dm._cinNextRow = true;
    }
}
