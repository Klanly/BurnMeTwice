using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenDoor : MonoBehaviour
{
    public GameObject posTeleport;
    public GameObject player;
    public bool onInteractTrigger = false;
    public FadeInterior fadeInterior;
    public GameObject fadeGameObject;
    bool canTeleport=false;
    bool oneTime = false;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (!oneTime)
        {
            fadeGameObject.SetActive(false);
            oneTime = true;
        }
        if (Input.GetButton("A") && onInteractTrigger)
        {

            fadeGameObject.SetActive(true);
           // fadeInterior.GetComponent<Animator>().enabled = true;
           // player.transform.position = posTeleport.transform.position;
           
        }

        if (onInteractTrigger)
        {
            if (fadeGameObject != null)
            {
                if (fadeInterior.allowTp)
                {
                    print("no tp");
                    player.transform.position = posTeleport.transform.position;
                }
            }
        }
    }
}
