using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneTrigger : MonoBehaviour {

    public string zoneName;
    private bool enter;

    private void Update() {
        if (enter) {
            Debug.Log("State: " + gameObject.transform.parent.GetComponent<ZoneChange>().messageBox.GetCurrentAnimatorStateInfo(0).IsName("ShowMessageLocation"));
            if (gameObject.transform.parent.GetComponent<ZoneChange>().messageBox.GetCurrentAnimatorStateInfo(0).IsName("ShowMessageLocation")) {
                gameObject.transform.parent.GetComponent<ZoneChange>().messageBox.SetBool("isActive", false);
                if (!gameObject.transform.parent.GetComponent<ZoneChange>().messageBox.GetCurrentAnimatorStateInfo(0).IsName("ShowMessageLocation")) {
                    ShowLocation();
                }
            } else {
                ShowLocation();
            }
        }
    }

    private void OnTriggerEnter(Collider col) {
        if (col.GetComponent<PlayerMovement>()) {
            enter = true;
        }
    }

    private void ShowLocation() {
        gameObject.transform.parent.GetComponent<ZoneChange>().currentZone = zoneName;
        gameObject.transform.parent.GetComponent<ZoneChange>().textBox.text = zoneName;
        gameObject.transform.parent.GetComponent<ZoneChange>().messageBox.SetBool("isActive", true);
        gameObject.transform.parent.GetComponent<ZoneChange>().DisableZoneMessage();
        enter = false;
    }

}
