using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractTrigger : MonoBehaviour
{
    public Transform grabPos;
    public Transform dropPos;
    public GameObject player;
    public GameObject currentPickUp;
    GameObject currentVillager;
    NewDialogueManager dialogueManager;
    public GameObject interactiveFeedbackSprite;
    public bool onVillager;
    
   
    // Start is called before the first frame update
    void Start()
    {
        dialogueManager = FindObjectOfType<NewDialogueManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("A") && onVillager && !dialogueManager.dialogueContainer.activeInHierarchy)
        {
            dialogueManager.LoadDialogue(currentVillager.GetComponent<Broadcaster>().nomenclature, currentVillager.GetComponent<Broadcaster>().visuals);
            

        }

        

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 9 && currentPickUp==null)
        {
            currentPickUp = other.gameObject;
            other.gameObject.GetComponent<PickUp>().onInteractTrigger = true;
            other.gameObject.GetComponent<PickUp>().grabPosition = grabPos;
            other.gameObject.GetComponent<PickUp>().dropPosition = dropPos;
            other.gameObject.GetComponent<PickUp>().player = player;
            other.gameObject.GetComponent<PickUp>().interactTrigger = this.gameObject;
            other.gameObject.GetComponent<PickUp>().outline.enabled=true;
            interactiveFeedbackSprite.GetComponent<UIFollow>().UpdateTarget(other.gameObject);
            interactiveFeedbackSprite.gameObject.SetActive(true);
        }

        
        if (other.gameObject.layer == 10)
        {
            onVillager = true;
            currentVillager = other.gameObject;
            interactiveFeedbackSprite.GetComponent<UIFollow>().UpdateTarget(other.gameObject);
            interactiveFeedbackSprite.gameObject.SetActive(true);

        }

        if (other.gameObject.layer == 13)
        {
            other.gameObject.GetComponent<OpenDoor>().onInteractTrigger = true;
            other.gameObject.GetComponent<OpenDoor>().player = transform.parent.gameObject;
            interactiveFeedbackSprite.GetComponent<UIFollow>().UpdateTarget(other.gameObject);
            interactiveFeedbackSprite.gameObject.SetActive(true);
        }

        if (other.gameObject.layer == 14)
        {
            other.gameObject.GetComponent<GiveClueEnviroment>().onInteractTrigger = true;
            interactiveFeedbackSprite.GetComponent<UIFollow>().UpdateTarget(other.gameObject);
            interactiveFeedbackSprite.gameObject.SetActive(true);

        }


    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            currentPickUp = null;
            other.gameObject.GetComponent<PickUp>().onInteractTrigger = false;
            other.gameObject.GetComponent<PickUp>().outline.enabled = false;
            interactiveFeedbackSprite.gameObject.SetActive(false);
        }
        if (other.gameObject.layer == 10)
        {
            ExitVillager();
        }

        if (other.gameObject.layer == 13)
        {
            other.gameObject.GetComponent<OpenDoor>().onInteractTrigger = false;
            interactiveFeedbackSprite.gameObject.SetActive(false);

        }
        if (other.gameObject.layer == 14)
        {
            other.gameObject.GetComponent<GiveClueEnviroment>().onInteractTrigger = false;
            interactiveFeedbackSprite.gameObject.SetActive(false);


        }

    }

    public void ExitVillager()
    {
        onVillager = false;
        currentVillager = null;
        interactiveFeedbackSprite.gameObject.SetActive(false);

    }

}
