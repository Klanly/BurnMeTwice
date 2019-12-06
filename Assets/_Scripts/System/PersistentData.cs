using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentData : MonoBehaviour
{
    public List<ScriptableObjectClueClass> cluesInInventory = new List<ScriptableObjectClueClass>();
    [HideInInspector]
    public Inventory inventory;
    public List<string> currentSceneCheckpoints = new List<string>();
    //--------------------------------------------------------------------IMPORTANTE - Resetear variable estática al reiniciar el juego y destruir el PersistentData
    public static bool created;
   
    bool currentPersistentData=false;

    public enum Langague
    {
        Spanish,
        English
    }
    public Langague language;



    private void Awake()
    {               
        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;
            currentPersistentData = true;
        }
        if(!currentPersistentData)
        {
            Destroy(this.gameObject);
        }
    }
   
    public void SaveNarrativeEventCompleted(string scene)
    {
        currentSceneCheckpoints.Add(scene);
    }
    
    public void SaveClue(ScriptableObjectClueClass clue)
    {
        if (cluesInInventory.Count > 0)
        {
            bool clueFound=false;
            foreach (ScriptableObjectClueClass c in cluesInInventory)
            {
                if (clue == c)
                {
                    clueFound = true;
                }
            }

            if (!clueFound)
            {
                print("Hola");
                cluesInInventory.Add(clue);
                inventory.EnableClueObtainedMessage(clue);
                inventory.RefreshInventory();
            }

        }
        else
        {
            inventory.EnableClueObtainedMessage(clue);
            cluesInInventory.Add(clue);
            inventory.RefreshInventory();
        }

    }

    public void ClearDataAndLoadScene(string scene)
    {
        currentSceneCheckpoints.Clear();
        cluesInInventory.Clear();
        currentSceneCheckpoints.Add(scene);
    }

   

}
