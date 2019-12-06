using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour {

    // Esta clase controla la UI del tiempo
    uint currentEventsTimeComplete;
    public uint maxEventsTime;
    public float velocityUI;

    // Crear un update para la funcionalidad del movimiento de la interfaz cuando se complete un evento de avanzar el tiempo.

    public void Advance() {
        currentEventsTimeComplete++;
    }


}
