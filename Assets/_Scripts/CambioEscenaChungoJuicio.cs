using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioEscenaChungoJuicio : MonoBehaviour
{
    public string escenaNombre;
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene(escenaNombre);
    }

}
