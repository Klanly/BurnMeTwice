using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public bool carrying;
    Vector3 initialPosition;
    Quaternion initialRotation;
    Rigidbody rb;

    public bool onInteractTrigger=false;
    [HideInInspector]
    public Transform grabPosition;
    [HideInInspector]
    public Transform dropPosition;
    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public GameObject interactTrigger;
    public float _waitForGrabAgain=0.4f;
    bool cooldown=false;
    [HideInInspector]
    public Outline outline;
    public bool draggedToRealWorld=false;
    

    void Start()
    {
        carrying = false;
        initialPosition = this.transform.position;
        initialRotation = this.transform.rotation;
        rb = GetComponent<Rigidbody>();
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    private void Update()
    {
        GrabObject();
        DropObject();
    }
    
    public void ResetObjectPosition()
    {
        rb.velocity = Vector3.zero; 
        transform.position = initialPosition;
        transform.rotation = initialRotation;
    }

    public void GrabObject()
    {
        if (Input.GetButtonDown("A"))
        {
            if (!carrying && onInteractTrigger&&!cooldown)
            {
                Input.ResetInputAxes();
                carrying = true;
                onInteractTrigger = false;
                transform.position = grabPosition.position;
                transform.rotation = player.transform.rotation;
                transform.SetParent(player.transform);
                interactTrigger.GetComponent<InteractTrigger>().interactiveFeedbackSprite.SetActive(false);
                interactTrigger.SetActive(false);
                rb.isKinematic = true;
            }
        }
    }
    public void DropObject()
    {
        if (Input.GetButtonDown("A"))
        {
            if (carrying)
            {
                Input.ResetInputAxes();
                this.transform.position = dropPosition.position;
                cooldown = true;
                onInteractTrigger = true;
                transform.SetParent(null);
                rb.isKinematic = false;
                interactTrigger.SetActive(true);
                interactTrigger.GetComponent<InteractTrigger>().interactiveFeedbackSprite.SetActive(true);
                StartCoroutine(WaitForGrabAgain());
                carrying = false;
                interactTrigger.GetComponent<InteractTrigger>();
            }
        }

    }

    public IEnumerator WaitForGrabAgain()
    {
        yield return new WaitForSeconds(_waitForGrabAgain);
        cooldown = false;
    }

}
