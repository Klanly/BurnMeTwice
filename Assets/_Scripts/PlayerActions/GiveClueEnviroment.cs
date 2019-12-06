using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GiveClueEnviroment : MonoBehaviour
{

    public bool clueGiven=false;
    public ScriptableObjectClueClass clueToGive;
    public PersistentData persistentData;
    public bool onInteractTrigger;
  

    private void Start()
    {
        persistentData = FindObjectOfType<PersistentData>();    
    }

    public void Update()
    {

        if (Input.GetButtonDown("A"))
        {

            if (onInteractTrigger && !clueGiven)
            {
                persistentData.SaveClue(clueToGive);
              
                clueGiven = true;

            }

        }

    }


    
}
