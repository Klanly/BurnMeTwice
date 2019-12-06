using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollow : MonoBehaviour
{

    public Transform posUI;
    public Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(posUI.position + offset);
    }


    public void UpdateTarget(GameObject posUIToTarget)
    {
        posUI = posUIToTarget.transform;
    }

}
