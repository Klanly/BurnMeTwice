using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

public class Inventory : MonoBehaviour
{
    public GameObject inventoryContainer;
    public GameObject choicesContainer;
    PersistentData persistentData;
   // List<ScriptableObjectClueClass> cluesInInventoryManager = new List<ScriptableObjectClueClass>();
    public List<GameObject> buttonsInInventory = new List<GameObject>();
    GameObject currentSelected;
    public Image evidenceImage;
    public TextMeshProUGUI evidenceDescription;
    public TextMeshProUGUI evidenceTitle;
    EventSystem eventSystem;
    PlayerMovement playerMovement;
    public GameObject dialogueContainer;
    ClueDataExcel clueDataExcel;
    [HideInInspector]
    public ScriptableObjectClueClass selectedClue;
    [HideInInspector]
    public bool blockInventory=false;
    public GameObject clueObtainedMessage;
    

    public List<ScriptableObjectClueClass> pruebaClues = new List<ScriptableObjectClueClass>();
    bool oneTime=false;
    public bool onTrial=false;
    public bool wasChoicesActive = false;
    // Start is called before the first frame update
    void Start()
    {
       
               
        //cluesInInventoryManager = persistentData.cluesInInventory;
        eventSystem = FindObjectOfType<EventSystem>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        clueDataExcel = GetComponent<ClueDataExcel>();
        persistentData = FindObjectOfType<PersistentData>();
        persistentData.inventory = this;
        RefreshInventory();

    }

    // Update is called once per frame
    void Update()
    {
        currentSelected = eventSystem.currentSelectedGameObject;
       
            //Prueba para añadir pistas a la lista de PersistentData
            if (Input.GetKeyDown(KeyCode.F))
            {
                persistentData.SaveClue(pruebaClues[0]);
                persistentData.SaveClue(pruebaClues[1]);
                persistentData.SaveClue(pruebaClues[2]);
                persistentData.SaveClue(pruebaClues[3]);
                persistentData.SaveClue(pruebaClues[4]);
                persistentData.SaveClue(pruebaClues[5]);
                persistentData.SaveClue(pruebaClues[6]);
                persistentData.SaveClue(pruebaClues[7]);
                persistentData.SaveClue(pruebaClues[8]);
                persistentData.SaveClue(pruebaClues[9]);
            }

        InputChecker();
        if (!onTrial)
        {
            OpenInventory();
            CloseInventory();
        }

       
        if (onTrial)
        {
            if (Input.GetButtonDown("Select")&& !inventoryContainer.activeInHierarchy)
            {
                
                OpenInventoryTrial();
                Input.ResetInputAxes();
            }

            if (Input.GetButtonDown("Select") || Input.GetButtonDown("B") && inventoryContainer.activeInHierarchy)
            {

                CloseInventoryTrial();
                Input.ResetInputAxes();
            }
        }

    }

    //Comprueba si se está moviendo el joystick para ejecutar ButtonFuncionality y así evitar que se esté ejecutando todo el rato en el Update
    #region Input Checker
    public void InputChecker()
    {
        if (inventoryContainer.activeInHierarchy)
        {
            if (eventSystem.currentSelectedGameObject != null)
            {

                if (!oneTime)
                {
                    ButtonFunctionality();

                    oneTime = true;
                }
                if (Input.GetAxisRaw("Horizontal") != 0)
                {
                    ButtonFunctionality();
                    print("Estoy en Update");
                }
            }

        }
    }
    #endregion

    #region Refresh Inventory
    public void RefreshInventory()
    {
       
            int currentIndex = 0;
            foreach (ScriptableObjectClueClass s in persistentData.cluesInInventory)
            {
                          
                buttonsInInventory[currentIndex].GetComponent<Image>().sprite = persistentData.cluesInInventory[currentIndex].clueImage;
                buttonsInInventory[currentIndex].GetComponent<Button>().interactable = true;
                currentIndex++;
            }
        ButtonFunctionality();
    }

    #endregion

    #region Open Inventory
    public void OpenInventory()
    {
        
        if (Input.GetButtonDown("Select")&& !inventoryContainer.activeInHierarchy && !blockInventory &&!dialogueContainer.activeInHierarchy)      //<--------------------BORRAR LA ULTIMA CONDICION PARA DESPUES DEL FP
        {
           
            
                playerMovement.PlayerCantMove();
                inventoryContainer.SetActive(true);
                oneTime = false;
               
            
            Input.ResetInputAxes();

            RefreshInventory();
        }
        ButtonFunctionality();

       
    }

    public void OpenInventoryTrial() {

        if ( !inventoryContainer.activeInHierarchy && !blockInventory)      //<--------------------BORRAR LA ULTIMA CONDICION PARA DESPUES DEL FP
        {

            if (choicesContainer.activeInHierarchy)
            {

                choicesContainer.SetActive(false);
                wasChoicesActive = true;
            }
            inventoryContainer.SetActive(true);
            oneTime = false;


            Input.ResetInputAxes();


        }
        ButtonFunctionality();


    }
    #endregion

    #region CloseInventory
    public void CloseInventory()
    {

        if (Input.GetButtonDown("Select")|| Input.GetButtonDown("B") && inventoryContainer.activeInHierarchy && !blockInventory)       //<--------------------BORRAR LA ULTIMA CONDICION PARA DESPUES DEL FP
        {
            
              if (!dialogueContainer.activeInHierarchy)
                {
                    playerMovement.PlayerCanMove();
                    
                }
                inventoryContainer.SetActive(false);

            Input.ResetInputAxes();
            ButtonFunctionality();

        }
      
    }

    public void CloseInventoryTrial() {

        if (inventoryContainer.activeInHierarchy && !blockInventory)       //<--------------------BORRAR LA ULTIMA CONDICION PARA DESPUES DEL FP
        {

            
            inventoryContainer.SetActive(false);

            Input.ResetInputAxes();
            ButtonFunctionality();
            if (wasChoicesActive)
            {
                choicesContainer.SetActive(true);
            }
        }

    }
    #endregion

    public void EnableClueObtainedMessage(ScriptableObjectClueClass clue)
    {
        
        clueObtainedMessage.GetComponentInChildren<TextMeshProUGUI>().text = clueDataExcel.LoadNameInClue(clue.clueName);
        clueObtainedMessage.transform.GetChild(0).GetComponent<Image>().sprite = clue.clueImage;
        clueObtainedMessage.SetActive(true);
        playerMovement.PlayerCantMove();
    }




    #region ButtonFunctionality
    public void ButtonFunctionality()
{
        if (inventoryContainer.activeInHierarchy && currentSelected != null)

        {
            if (persistentData.cluesInInventory.Count > 0)
            {
                if (currentSelected.name.Contains("Evidence"))
                {
                    switch (currentSelected.name)
                    {

                        case "Evidence1":
                            evidenceImage.sprite = persistentData.cluesInInventory[0].clueImage;
                            evidenceDescription.text = clueDataExcel.LoadDescriptionInClue(persistentData.cluesInInventory[0].clueDescription);
                            evidenceTitle.text = clueDataExcel.LoadNameInClue(persistentData.cluesInInventory[0].clueName);
                            selectedClue = persistentData.cluesInInventory[0];

                            foreach (GameObject g in buttonsInInventory)
                            {
                                if (g.name == "Evidence1")
                                {
                                    g.transform.GetChild(0).gameObject.SetActive(true);
                                }
                                else
                                {
                                    g.transform.GetChild(0).gameObject.SetActive(false);
                                }
                            }

                            break;

                        case "Evidence2":
                            evidenceImage.sprite = persistentData.cluesInInventory[1].clueImage;
                            evidenceDescription.text = clueDataExcel.LoadDescriptionInClue(persistentData.cluesInInventory[1].clueDescription);
                            evidenceTitle.text = clueDataExcel.LoadNameInClue(persistentData.cluesInInventory[1].clueName);
                            selectedClue = persistentData.cluesInInventory[1];

                            foreach (GameObject g in buttonsInInventory)
                            {
                                if (g.name == "Evidence2")
                                {
                                    g.transform.GetChild(0).gameObject.SetActive(true);
                                }
                                else
                                {
                                    g.transform.GetChild(0).gameObject.SetActive(false);
                                }
                            }
                            break;

                        case "Evidence3":
                            evidenceImage.sprite = persistentData.cluesInInventory[2].clueImage;
                            evidenceDescription.text = clueDataExcel.LoadDescriptionInClue(persistentData.cluesInInventory[2].clueDescription);
                            evidenceTitle.text = clueDataExcel.LoadNameInClue(persistentData.cluesInInventory[2].clueName);
                            selectedClue = persistentData.cluesInInventory[2];

                            foreach (GameObject g in buttonsInInventory)
                            {
                                if (g.name == "Evidence3")
                                {
                                    g.transform.GetChild(0).gameObject.SetActive(true);
                                }
                                else
                                {
                                    g.transform.GetChild(0).gameObject.SetActive(false);
                                }
                            }
                            break;

                        case "Evidence4":
                            evidenceImage.sprite = persistentData.cluesInInventory[3].clueImage;
                            evidenceDescription.text = clueDataExcel.LoadDescriptionInClue(persistentData.cluesInInventory[3].clueDescription);
                            evidenceTitle.text = clueDataExcel.LoadNameInClue(persistentData.cluesInInventory[3].clueName);
                            selectedClue = persistentData.cluesInInventory[3];

                            foreach (GameObject g in buttonsInInventory)
                            {
                                if (g.name == "Evidence4")
                                {
                                    g.transform.GetChild(0).gameObject.SetActive(true);
                                }
                                else
                                {
                                    g.transform.GetChild(0).gameObject.SetActive(false);
                                }
                            }
                            break;

                        case "Evidence5":
                            evidenceImage.sprite = persistentData.cluesInInventory[4].clueImage;
                            evidenceDescription.text = clueDataExcel.LoadDescriptionInClue(persistentData.cluesInInventory[4].clueDescription);
                            evidenceTitle.text = clueDataExcel.LoadNameInClue(persistentData.cluesInInventory[4].clueName);
                            selectedClue = persistentData.cluesInInventory[4];

                            foreach (GameObject g in buttonsInInventory)
                            {
                                if (g.name == "Evidence5")
                                {
                                    g.transform.GetChild(0).gameObject.SetActive(true);
                                }
                                else
                                {
                                    g.transform.GetChild(0).gameObject.SetActive(false);
                                }
                            }
                            break;

                        case "Evidence6":
                            evidenceImage.sprite = persistentData.cluesInInventory[5].clueImage;
                            evidenceDescription.text = clueDataExcel.LoadDescriptionInClue(persistentData.cluesInInventory[5].clueDescription);
                            evidenceTitle.text = clueDataExcel.LoadNameInClue(persistentData.cluesInInventory[5].clueName);
                            selectedClue = persistentData.cluesInInventory[5];

                            foreach (GameObject g in buttonsInInventory)
                            {
                                if (g.name == "Evidence6")
                                {
                                    g.transform.GetChild(0).gameObject.SetActive(true);
                                }
                                else
                                {
                                    g.transform.GetChild(0).gameObject.SetActive(false);
                                }
                            }
                            break;

                        case "Evidence7":
                            evidenceImage.sprite = persistentData.cluesInInventory[6].clueImage;
                            evidenceDescription.text = clueDataExcel.LoadDescriptionInClue(persistentData.cluesInInventory[6].clueDescription);
                            evidenceTitle.text = clueDataExcel.LoadNameInClue(persistentData.cluesInInventory[6].clueName);
                            selectedClue = persistentData.cluesInInventory[6];

                            foreach (GameObject g in buttonsInInventory)
                            {
                                if (g.name == "Evidence7")
                                {
                                    g.transform.GetChild(0).gameObject.SetActive(true);
                                }
                                else
                                {
                                    g.transform.GetChild(0).gameObject.SetActive(false);
                                }
                            }
                            break;

                        case "Evidence8":
                            evidenceImage.sprite = persistentData.cluesInInventory[7].clueImage;
                            evidenceDescription.text = clueDataExcel.LoadDescriptionInClue(persistentData.cluesInInventory[7].clueDescription);
                            evidenceTitle.text = clueDataExcel.LoadNameInClue(persistentData.cluesInInventory[7].clueName);
                            selectedClue = persistentData.cluesInInventory[7];

                            foreach (GameObject g in buttonsInInventory)
                            {
                                if (g.name == "Evidence8")
                                {
                                    g.transform.GetChild(0).gameObject.SetActive(true);
                                }
                                else
                                {
                                    g.transform.GetChild(0).gameObject.SetActive(false);
                                }
                            }
                            break;

                        case "Evidence9":
                            evidenceImage.sprite = persistentData.cluesInInventory[8].clueImage;
                            evidenceDescription.text = clueDataExcel.LoadDescriptionInClue(persistentData.cluesInInventory[8].clueDescription);
                            evidenceTitle.text = clueDataExcel.LoadNameInClue(persistentData.cluesInInventory[8].clueName);
                            selectedClue = persistentData.cluesInInventory[8];

                            foreach (GameObject g in buttonsInInventory)
                            {
                                if (g.name == "Evidence9")
                                {
                                    g.transform.GetChild(0).gameObject.SetActive(true);
                                }
                                else
                                {
                                    g.transform.GetChild(0).gameObject.SetActive(false);
                                }
                            }

                            break;

                        case "Evidence10":
                            evidenceImage.sprite = persistentData.cluesInInventory[9].clueImage;
                            evidenceDescription.text = clueDataExcel.LoadDescriptionInClue(persistentData.cluesInInventory[9].clueDescription);
                            evidenceTitle.text = clueDataExcel.LoadNameInClue(persistentData.cluesInInventory[9].clueName);
                            selectedClue = persistentData.cluesInInventory[9];

                            foreach (GameObject g in buttonsInInventory)
                            {
                                if (g.name == "Evidence10")
                                {
                                    g.transform.GetChild(0).gameObject.SetActive(true);
                                }
                                else
                                {
                                    g.transform.GetChild(0).gameObject.SetActive(false);
                                }
                            }


                            break;

                    }
                }
            }
            else
            {
                evidenceTitle.text = "";
                evidenceDescription.text = "";
                evidenceImage.sprite =null;

            }
        }
}
    #endregion
}
