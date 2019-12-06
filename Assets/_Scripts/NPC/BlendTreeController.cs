using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BlendTreeController : MonoBehaviour
{
    Animator anim;
    NavMeshAgent nma;

    private void Start()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();
        nma = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        try
        {
            anim.SetFloat("Speed", nma.velocity.magnitude);
        }
        catch
        {
            anim.SetFloat("Speed", 0);
        }
    }
}
