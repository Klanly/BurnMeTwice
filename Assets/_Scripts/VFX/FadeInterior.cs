using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInterior : MonoBehaviour
{
    public PlayerMovement player;
    public bool allowTp=false;

    Animator anim;
    
    // Start is called before the first frame update
    void Start()
    {
      anim =  GetComponent<Animator>();
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerCantMove()
    {
        player.canMove = false;
    }
    public void PlayerCanMove()
    {
        player.canMove = true;
    }
    public void AllowTP()
    {
        print("Si fade");
        allowTp = true;
    }

    public void FinishFade()
    {
        PlayerCanMove();
        allowTp = false;
        this.gameObject.SetActive(false);
    }
}
