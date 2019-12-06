using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EventTownManager))]
public class Editor_EventTownManager : Editor {

    private string[] eventsName;
    private List<GameObject> receiversObject = new List<GameObject>();
    private List<GameObject> cameraObject = new List<GameObject>();
    private string[] cameraName;
    public GameObject receiverManager;
    public GameObject inventoryManager;
    public GameObject dialogueManager;
    public GameObject timeManager;
    public GameObject cameraManager;

    public override void OnInspectorGUI() {

        // Hace referencia a las variables del script que editamos.
        EventTownManager eventTownManager = (EventTownManager)target;

        // Convierte la lista de eventos en un array de nombres
        eventsName = new string[eventTownManager.events.Count];
        for (int i = 0; i < eventTownManager.events.Count; i++) {
            eventsName[i] = eventTownManager.events[i].name;
        }

        //receiverManager = null;
        //cameraManager = null;

        //receiverManager = (GameObject)EditorGUILayout.ObjectField("Receiver Manager: ", receiverManager, typeof(GameObject), true);
        //cameraManager = (GameObject)EditorGUILayout.ObjectField("Camera Manager: ", cameraManager, typeof(GameObject), true);

        // Convierte la lista de objetos receiver y pickup en una lista de nombres
        FillReceiversList(eventTownManager);
        FillCameraList(eventTownManager);

        serializedObject.Update();
        SerializedObject obj = this.serializedObject;

        SerializedProperty receiversGameObject = obj.FindProperty("eventReceiverScripts");
        EditorGUILayout.PropertyField(receiversGameObject, true);
        SerializedProperty camerasGameObject = obj.FindProperty("eventCameras");
        EditorGUILayout.PropertyField(camerasGameObject, true);

        EditorList.Show(obj, eventsName, eventTownManager.receiversName, cameraName, cameraManager);
        serializedObject.ApplyModifiedProperties();

        // Se crea un botón con su funcionalidad 
        if (GUILayout.Button("Create Event")) {
            // Añadir un evento vacio
            AddEmptyEvent(eventTownManager);
        }
        

    }

    #region AddEmptyEvent - Añade un nuevo evento a la lista
    // Añade un nuevo evento con los valores por defecto a la lista "Events"
    void AddEmptyEvent(EventTownManager eventTownManager) {

        EventTownManager.EventTown et = new EventTownManager.EventTown();


        // Controla que siempre que haya un elemento en la lista se incremente el nombre del evento
        // para no tener dos iguales. Si se borran todos los eventos de lista comenzará de nuevo.
        if (eventTownManager.events.Count > 0) {
            eventTownManager.numOfEventsCreate++;
        }
        if (eventTownManager.events.Count == 0) {
            eventTownManager.numOfEventsCreate = 0;
        }


        // Inicializa las variables del nuevo evento.
        et.idEvent = (uint)eventTownManager.numOfEventsCreate;
        et.name = "Event " + et.idEvent;
        et.idEventReceiver = 0;

        // Añade el evento creado a la lista de eventos.
        eventTownManager.events.Add(et);
    }
    #endregion

    #region FillReceiversList - Rellena la lista de "EventReceiverScripts"
    // Añade a la lista "eventReceiverScripts" de "EventTownManager" todos los objetos que tengan un "EventReceiver"
    // También los visualiza por pantalla en el editor de los eventos como un popup de strings.
    // ------------------------------------------------------------------------------------------> PARA QUE FUNCIONE, EL SCRIPT EVENTRECEIVER TIENE QUE ESTAR EN UN HIJO.
    void FillReceiversList(EventTownManager eventTownManager) {
        bool add = true;

        receiverManager = GameObject.FindGameObjectWithTag("ReceiverManager");
        List<GameObject> receivers = new List<GameObject>();

        inventoryManager = GameObject.FindGameObjectWithTag("InventoryManager");
        if (inventoryManager) {
            receivers.Insert(receivers.Count, inventoryManager);
        }

        dialogueManager = GameObject.FindGameObjectWithTag("DialogueManager");
        if (dialogueManager) {
            receivers.Insert(receivers.Count, dialogueManager);
        }

        timeManager = GameObject.FindGameObjectWithTag("TimeManager");
        if (timeManager) {
            receivers.Insert(receivers.Count, timeManager);
        }

        cameraManager = GameObject.FindGameObjectWithTag("CameraManager");
        if (cameraManager) {
            receivers.Insert(receivers.Count, cameraManager);
        }

        for (int i = 0; i < receiverManager.transform.childCount; i++) {
            receivers.Insert(receivers.Count, receiverManager.transform.GetChild(i).gameObject);
        }

        // Se añaden a la lista receiverObject y se comprueba que si hay un nuevo objeto, no esté repetido
        // para no variar el orden cada vez que se ejecuta (por si se mueven los gameobject en la escena)
        for (int i = 0; i < receivers.Count; i++) {
            if (receiversObject != null) {
                for (int j = 0; j < receiversObject.Count; j++) {
                    if (receivers[i].gameObject == receiversObject[j].gameObject) {
                        add = false;
                        break;
                    } else {
                        add = true;
                    }
                }
            }
            if (add) {
                receiversObject.Insert(receiversObject.Count, receivers[i].gameObject);
                eventTownManager.eventReceiverScripts.Insert(eventTownManager.eventReceiverScripts.Count, receivers[i].gameObject);
            }
        }


        // Reinicia todas las variables. Si "go" está a 0 es que no hay objectos en la escena de tipo "EventReceiver"
        if (receivers.Count == 0) {
            receiversObject.Clear();
            eventTownManager.eventReceiverScripts.Clear();
        }
        
        // Eliminar de la lista "receiversObject" y "eventTownManager.eventReceiverScripts" todos aquellos elementos que ya no esten
        for (int i = 0; i < receiversObject.Count; i++) {
            if (receiversObject[i] == null) {
                receiversObject.RemoveAt(i);
                i--;
            }
        }
        
        // Se resetea la lista eventReceiverScripts
        eventTownManager.eventReceiverScripts.Clear();
        for (int i = 0; i < receiversObject.Count; i++) {
            eventTownManager.eventReceiverScripts.Add(receiversObject[i]);
        }
        
        // Rellena la lista que se mostrará en el enum
        eventTownManager.receiversName = new string[receiversObject.Count];
        if (receiversObject.Count != 0) {
            for (int i = 0; i < receiversObject.Count; i++) {
                eventTownManager.receiversName[i] = receiversObject[i].name;
            }
        }
    }
    #endregion

    #region FillCameraList - Rellena una lista con las cámaras del escenario
    private void FillCameraList(EventTownManager eventTownManager) {
        bool add = true;

        if (cameraManager != null) {
            GameObject[] cam = new GameObject[cameraManager.transform.GetChild(1).childCount];
            for (int i = 0; i < cameraManager.transform.GetChild(1).childCount; i++) {
                cam[i] = cameraManager.transform.GetChild(1).GetChild(i).gameObject;
            }

            // Se añaden a la lista cameraObject y se comprueba que si hay un nuevo objeto, no esté repetido
            // para no variar el orden cada vez que se ejecuta (por si se mueven los gameobject en la escena)
            for (int i = 0; i < cam.Length; i++) {
                if (cameraObject != null) {
                    for (int j = 0; j < cameraObject.Count; j++) {
                        if (cameraObject[j] == cam[i].gameObject) {
                            add = false;
                            break;
                        } else {
                            add = true;
                        }
                    }
                }
                if (add) {
                    cameraObject.Insert(cameraObject.Count, cam[i].gameObject);
                    eventTownManager.eventCameras.Insert(eventTownManager.eventCameras.Count, cam[i].gameObject);
                }
            }

            if (cam.Length == 0) {
                cameraObject.Clear();
                eventTownManager.eventCameras.Clear();
            }

            for (int i = 0; i < cameraObject.Count; i++) {
                if (cameraObject[i] == null) {
                    cameraObject.RemoveAt(i);
                    i--;
                }
            }

            // Se resetea la lista eventCameras
            eventTownManager.eventCameras.Clear();
            for (int i = 0; i < cameraObject.Count; i++) {
                eventTownManager.eventCameras.Add(cameraObject[i]);
            }

            // Rellena la lista que se mostrará en el enum
            cameraName = new string[cameraObject.Count];
            if (cameraObject.Count != 0) {
                for (int i = 0; i < cameraObject.Count; i++) {
                    cameraName[i] = cameraObject[i].name;
                }
            }
        }
        
    }
    #endregion
}

