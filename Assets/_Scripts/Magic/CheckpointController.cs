using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CheckpointController : MonoBehaviour
{
    PersistentData persistentData;
    EventSystem eventSystem;
    GameObject currentSelected;
    public GameObject resetDayContainer;
    public List<Button> checkpointButtons = new List<Button>();
   
    

    [System.Serializable]
    public class SceneInCheckpoint
    {
        public string sceneName;
        public List<Image> clueImages = new List<Image>();
        
        public List<ScriptableObjectClueClass> cluesInCheckpoint = new List<ScriptableObjectClueClass>();
    }

    public List<SceneInCheckpoint> sceneInCheckpoint = new List<SceneInCheckpoint>();


    // Start is called before the first frame update
    void Start()
    {
        persistentData = FindObjectOfType<PersistentData>();
        eventSystem = FindObjectOfType<EventSystem>();
        RefreshCheckpointContainer();
    }

    // Update is called once per frame
    void Update()
    {
        currentSelected = eventSystem.currentSelectedGameObject;

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (resetDayContainer.activeInHierarchy)
            {
                resetDayContainer.SetActive(false);
               
                Input.ResetInputAxes();
            }
            else
            {

                resetDayContainer.SetActive(true);
                RefreshCheckpointContainer();
                Input.ResetInputAxes();
            }

        }

    }


    public void RefreshCheckpointContainer()
    {
        int currentIndexButton = 0;
       
        foreach (Button b in checkpointButtons)
        {
            if (currentIndexButton <= persistentData.currentSceneCheckpoints.Count - 1)
            {
                b.interactable = true;
            }
            currentIndexButton++;
        }
        foreach (SceneInCheckpoint s in sceneInCheckpoint)
        {

            for (int i = 0; i < s.clueImages.Count - 1; i++)
            {
                
                for (int p = 0; p <= persistentData.cluesInInventory.Count - 1; p++)
                {
                   
                    if (persistentData.cluesInInventory[p] == s.cluesInCheckpoint[i])
                    {
                        s.clueImages[i].sprite = persistentData.cluesInInventory[p].clueImage;
                       
                    }
                    else
                    {
                        //Meter aquí el código para que cambie el sprite si no se ha encontrado pista en el inventario
                    }


                }

            }
        }
    }


    public void LoadCheckpoint()
    {

        if (currentSelected.name.Contains("Checkpoint"))
        {
            switch (currentSelected.name)
            {
                 
                case "Checkpoint1":
                    SceneManager.LoadScene(persistentData.currentSceneCheckpoints[0]);
                    if (Input.GetButtonDown("A") )
                    {
                        SceneManager.LoadScene(persistentData.currentSceneCheckpoints[0]);
                    }

                    break;
                case "Checkpoint2":
                    SceneManager.LoadScene(persistentData.currentSceneCheckpoints[1]);
                    if (Input.GetButtonDown("A"))
                    {
                        SceneManager.LoadScene(persistentData.currentSceneCheckpoints[1]);
                    }

                    break;
                case "Checkpoint3":
                    SceneManager.LoadScene(persistentData.currentSceneCheckpoints[2]);
                    if (Input.GetButtonDown("A"))
                    {
                        SceneManager.LoadScene(persistentData.currentSceneCheckpoints[2]);
                    }

                    break;
                case "Checkpoint4":
                    SceneManager.LoadScene(persistentData.currentSceneCheckpoints[3]);
                    if (Input.GetButtonDown("A"))
                    {
                        SceneManager.LoadScene(persistentData.currentSceneCheckpoints[3]);
                    }

                    break;

            }

        }
        
    }
    
}
