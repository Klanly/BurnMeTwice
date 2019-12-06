using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.PostProcessing;

public class Afterlife : MonoBehaviour
{
    InteractTrigger interactTrigger;
    bool onAfterlife = false;
    GameObject[] afterLifeGameObjects;
    GameObject[] lifeGameObjects;

    public PostProcessVolume postProcess;
    ColorGrading colorGradingLayer = null;
    ChromaticAberration chromaticAberration = null;
    Inventory inventory;
    NewDialogueManager dialogueManager;

    // Start is called before the first frame update
    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        dialogueManager = FindObjectOfType<NewDialogueManager>();
        postProcess.profile.TryGetSettings(out colorGradingLayer);
        postProcess.profile.TryGetSettings(out chromaticAberration);
        interactTrigger = GetComponentInChildren<InteractTrigger>();
        afterLifeGameObjects = GameObject.FindGameObjectsWithTag("AfterlifeObject");
        lifeGameObjects = GameObject.FindGameObjectsWithTag("LifeObject");

        DisableAfterlife();

       
    }

    // Update is called once per frame
    void Update()
    {
        AfterlifeTrigger();
    }

    void AfterlifeTrigger()
    {
        if (Input.GetButtonDown("RB") && !inventory.inventoryContainer.activeInHierarchy && !dialogueManager.dialogueContainer.activeInHierarchy)
        {
            if (!onAfterlife)
            {
                EnableAfterlife();
                onAfterlife = true;
                Input.ResetInputAxes();

            }
            else
            {
                DisableAfterlife();
                onAfterlife = false;
                Input.ResetInputAxes();
            }

        }

    }

    void EnableAfterlife()
    {
        
        foreach (GameObject g in afterLifeGameObjects)
        {
            if (g.GetComponent<NavMeshAgent>() != null)
            {
               g.GetComponent<NavMeshAgent>().enabled = true;
               g.GetComponent<Rigidbody>().isKinematic = false;
               g.GetComponent<Collider>().enabled = true;
               g.transform.GetChild(0).gameObject.SetActive(true);
               interactTrigger.ExitVillager();
            }

            if(g.GetComponent<PickUp>()!=null)
            {
                if (!g.GetComponent<PickUp>().carrying)
                {
                    g.GetComponent<MeshRenderer>().enabled = true;
                    g.GetComponent<Rigidbody>().isKinematic = false;
                    g.GetComponent<Collider>().enabled = true;
                }

            }
           
        }



        foreach (GameObject g in lifeGameObjects)
        {
            if (g.GetComponent<NavMeshAgent>() != null)
            {  
                g.GetComponent<Collider>().enabled = false;
                g.transform.GetChild(0).gameObject.SetActive(false);
            }

        }

        colorGradingLayer.hueShift.value = -101;
        chromaticAberration.intensity.value = 1;

    }
    void DisableAfterlife()
    {
        foreach (GameObject g in afterLifeGameObjects)
        {
            if (g.GetComponent<NavMeshAgent>() != null)
            {
                g.GetComponent<NavMeshAgent>().enabled = false;
                g.GetComponent<Rigidbody>().isKinematic = true;
                g.GetComponent<Collider>().enabled = false;
                g.transform.GetChild(0).gameObject.SetActive(false);
                interactTrigger.ExitVillager();

            }
            if (g.GetComponent<PickUp>() != null)
            {
                if (!g.GetComponent<PickUp>().draggedToRealWorld)
                {
                    if (!g.GetComponent<PickUp>().carrying)
                    {
                        g.GetComponent<MeshRenderer>().enabled = false;
                        g.GetComponent<Rigidbody>().isKinematic = true;
                        g.GetComponent<Collider>().enabled = false;
                    }
                    else
                    {
                        g.GetComponent<PickUp>().draggedToRealWorld = true;
                    }
                }

            }

        }

        foreach (GameObject g in lifeGameObjects)
        {
            if (g.GetComponent<NavMeshAgent>() != null)
            {
                g.GetComponent<Collider>().enabled = true;
                g.transform.GetChild(0).gameObject.SetActive(true);
            }

        }
        colorGradingLayer.hueShift.value = 0;
        chromaticAberration.intensity.value = 0.098f;
    }

}
