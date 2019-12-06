using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    Rigidbody rb;
    public bool canMove = true;
    public Animator animator;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        
        animator.SetFloat("Velocity", Mathf.Abs(rb.velocity.magnitude / speed));

        Movement();

    }
    public void Movement()
    {

        if (canMove)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            Vector3 direction = new Vector3(h, 0, v);

            if (h != 0 || v != 0)
                this.transform.rotation = Quaternion.LookRotation(direction);



            rb.velocity = Vector3.ClampMagnitude(direction, 1f) * speed;
        }
        else
            rb.velocity = Vector3.zero;

    }

    public void PlayerCantMove()
    {
        canMove = false;
    }
    public void PlayerCanMove()
    {
        canMove = true;
    }

}
