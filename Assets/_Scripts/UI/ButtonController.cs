
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour
{
    GameObject currentSelected;
    public NewDialogueManager dialogueManager;
    public EventSystem eventSystem;
    public GameObject inventoryContainer;
    public GameObject resetDayContainer;
    public GameObject firstSelectedChoices;
    public GameObject firstSelectedInventory;
    public GameObject firstSelectedResetDay;

    bool choicesOneTime = false;
    bool inventoryOneTime = false;
    bool resetDayOneTime = false;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        SeleccionadorBotonesChoices();
        SeleccionadorBotonesInventory();
        SeleccionadorBotonesResetDay();

     
    }

    #region SeleccionadorBotonesChoices
    public void SeleccionadorBotonesChoices()
    {
        if (dialogueManager.dialogueContainer.activeInHierarchy && !inventoryContainer.activeInHierarchy && !resetDayContainer.activeInHierarchy)
        {

           

            if (dialogueManager.choicesContainer.activeInHierarchy && !choicesOneTime)
            {
                StartCoroutine(PrimeroSeleccionadoChoicesCorrutina());
                choicesOneTime = true;

            }
            
            VolverControlMandoDespuesDeClick();
        }
        if (!dialogueManager.choicesContainer.activeInHierarchy)
        {
            choicesOneTime = false;
        }
    }
    #endregion
    
    #region SeleccionadorBotonesResetDay
    public void SeleccionadorBotonesResetDay()
    {
        if (!dialogueManager.dialogueContainer.activeInHierarchy && !inventoryContainer.activeInHierarchy)
        {



            if (resetDayContainer.activeInHierarchy && !resetDayOneTime)
            {
                StartCoroutine(PrimeroSeleccionadoResetDayCorrutina());
                resetDayOneTime = true;

            }

            VolverControlMandoDespuesDeClick();
        }
        if (!resetDayContainer.activeInHierarchy)
        {
            resetDayOneTime = false;
        }
    }
    #endregion
  
    #region SeleccionadorBotonesInventory
    public void SeleccionadorBotonesInventory()
    {
        if (!dialogueManager.onTrial)
        {
            if (!dialogueManager.dialogueContainer.activeInHierarchy && inventoryContainer.activeInHierarchy && !resetDayContainer.activeInHierarchy)
            {



                if (!dialogueManager.choicesContainer.activeInHierarchy && inventoryContainer.activeInHierarchy && !inventoryOneTime)
                {
                    print("Estoy");
                    StartCoroutine(PrimeroSeleccionadoInventoryCorrutina());
                    inventoryOneTime = true;

                }

                VolverControlMandoDespuesDeClick();
            }
            if (!inventoryContainer.activeInHierarchy)
            {
                inventoryOneTime = false;
            }
        }
        else
        {
            if (inventoryContainer.activeInHierarchy && !resetDayContainer.activeInHierarchy)
            {



                if (!dialogueManager.choicesContainer.activeInHierarchy && inventoryContainer.activeInHierarchy && !inventoryOneTime)
                {
                    print("Estoy");
                    StartCoroutine(PrimeroSeleccionadoInventoryCorrutina());
                    inventoryOneTime = true;

                }

                VolverControlMandoDespuesDeClick();
            }
            if (!inventoryContainer.activeInHierarchy)
            {
                inventoryOneTime = false;
            }
        }
    }
    #endregion

    #region VolverControlMandoDespuesDeClick
    public void VolverControlMandoDespuesDeClick()
    {
        if (eventSystem.currentSelectedGameObject != null)
        {

            currentSelected = eventSystem.currentSelectedGameObject;
        }
        else
        {
            eventSystem.SetSelectedGameObject(currentSelected);

        }
    }
    #endregion

    #region PrimeroSeleccionadoChoicesCorrutina
    public IEnumerator PrimeroSeleccionadoChoicesCorrutina()
    {
        eventSystem.SetSelectedGameObject(null);
        yield return null;
        eventSystem.SetSelectedGameObject(firstSelectedChoices);

    }
    #endregion

    #region PrimeroSeleccionadoInventoryCorrutina
    public IEnumerator PrimeroSeleccionadoInventoryCorrutina()
    {
        eventSystem.SetSelectedGameObject(null);
        yield return null;
        eventSystem.SetSelectedGameObject(firstSelectedInventory);

    }
    #endregion

    #region PrimeroSeleccionadoInventoryCorrutina
    public IEnumerator PrimeroSeleccionadoResetDayCorrutina()
    {
        eventSystem.SetSelectedGameObject(null);
        yield return null;
        eventSystem.SetSelectedGameObject(firstSelectedResetDay);

    }
    #endregion
}
