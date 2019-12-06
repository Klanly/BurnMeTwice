using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ZoneChange : MonoBehaviour {

    public Animator messageBox;
    public TextMeshProUGUI textBox;

    public string currentZone;

    private void Start() {
        if (currentZone != "") {
            DisableZoneMessage();
        } else {
            Debug.LogErrorFormat("Error de Zona: ZonaChange.currentZone no tienen definido un valor incial");
        }
        
    }

    public void DisableZoneMessage() {
        for (int i = 0; i < gameObject.transform.childCount; i++) {
            if (transform.GetChild(i).GetComponent<ZoneTrigger>().zoneName == currentZone) {
                transform.GetChild(i).gameObject.SetActive(false);
            } else {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
}
