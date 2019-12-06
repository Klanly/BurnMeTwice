using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTownManager : MonoBehaviour {

    #region Variables
    public int numOfEventsCreate;

    public string[] receiversName;

    public List<GameObject> eventReceiverScripts = new List<GameObject>();
    public List<GameObject> eventCameras = new List<GameObject>();

    public List<EventTown> events = new List<EventTown>();
    public List<EventTown> eventsComplete = new List<EventTown>();
    public List<EventTown> eventsInProgress = new List<EventTown>();
    public List<EventTown> eventsWaiting = new List<EventTown>();
    public List<EventTown> eventsFailed = new List<EventTown>();

    [System.Serializable]
    public class EventTown {

        public string name;
        [HideInInspector] public uint idEvent; // Automatico
        public int idEventReceiver; // De la lista de eventReceiverScripts
        public List<Conditions> conditions = new List<Conditions>();

        [System.Serializable]
        public class Conditions {
            public string nameCondition;
            public string nameEventCondition;
            [HideInInspector] public int indexPopupCondition;
            public bool or_And;

            public enum StatesToStart {
                Waiting,
                InProgress,
                Failed,
                Completed
            }
            public StatesToStart state;
        }

        public enum Actions {
            Wait,
            WaitLookingObject,
            WaitWithRotation,
            Move,
            Patrol,
            DialogueChange,
            DialogueComplete,
            DialogueActive,
            GiveClue,
            CheckClue,
            Animation,
            WhenPlayerEnterInThisTrigger,
            GameobjectActive,
            GameObjectActiveInScene,
            CheckClueInTrial,
            StopPatrol,
            AdvanceTime,
            CameraChange,
            DialogueLoop
        }
        public Actions action;

        public float timeToWait;
        public GameObject rotationToward;
        public Vector3 rotationGlobal;

        public float velocityMove;
        public Vector3 wayPoint;
        public List<Vector3> wayPoints;

        public string dialogueNomenclature;
        public bool seeCharactersOnScreen;

        public ScriptableObjectClueClass clueToGive;
        public ScriptableObjectClueClass clueToCheck;
        public ScriptableObjectClueClass clueToCheckInTrial;

        public string animationTrigger;

        public bool activeGameObject;

        public List<ActiveObjectsInScene> activeObjectsInScenes = new List<ActiveObjectsInScene>();
        [System.Serializable]
        public class ActiveObjectsInScene {
            public bool isActive;
            public GameObject objectInScene;
        }

        //public bool timeToAdvance;

        public int cameraEnable;
        public int cameraDisable;
        public enum CameraBlend {
            Cut,
            EaseInOut,
            EaseIn,
            EaseOut,
            HardIn,
            HardOut,
            Linear
        }
        public CameraBlend camBlend;
        public int camBlendIndex;
        public float camTimeBlend;

        public List<DialoguesInLoop> dialoguesLoop = new List<DialoguesInLoop>();
        [System.Serializable]
        public class DialoguesInLoop {
            public bool visualizeCharacters;
            public string dialogueNomenclatureLoop;
        }

        public enum States {
            Waiting,
            InProgress,
            Failed,
            Completed
        }
        public States state;

    }

    //int conditionsOK;
    int conditionsOR, conditionsAND;
    int conditionsOR_OK, conditionsAND_OK;

    bool updateNow = true;
    NewDialogueManager dialogueManager;
    PersistentData persistentData;

    #endregion

    private void Start() {
        persistentData = FindObjectOfType<PersistentData>();
        dialogueManager = FindObjectOfType<NewDialogueManager>();
    }

    #region Update
    private void Update() {
        if (updateNow) {
            foreach (EventTown et in events) {
                if (et.state == EventTown.States.Waiting) { //  Si está en estado de "Waiting"
                    if (et.conditions.Count > 0) { // Compruebame si tiene condiciones
                        // Incializamos variables
                        conditionsAND = 0;
                        conditionsAND_OK = 0;
                        conditionsOR = 0;
                        conditionsOR_OK = 0;

                        // Filtrame cuantas condiciones AND y OR tengo
                        for (int i = 0; i < et.conditions.Count; i++) {
                            if (et.conditions[i].or_And) {
                                conditionsOR++;
                            } else {
                                conditionsAND++;
                            }
                        }

                        // Se compruebas las condiciones con filtro AND
                        if (conditionsAND > 0) {
                            for (int i = 0; i < et.conditions.Count; i++) { // Si tienes condiciones, compruebame condición a condición
                                if (!et.conditions[i].or_And) { // Compruebame todas las condiciones con filtro AND
                                    for (int j = 0; j < events.Count; j++) { // Y dime si esas condiciones estan o no complidas (TODAS)
                                        if (events[j].name == et.conditions[i].nameEventCondition) {
                                            if (et.conditions[i].state.ToString() == events[j].state.ToString()) {
                                                conditionsAND_OK++;
                                                break;
                                            } else {
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        // Se compruebas las condiciones con filtro OR
                        if (conditionsOR > 0) {
                            for (int x = 0; x < et.conditions.Count; x++) { // Si tienes condiciones, compruebame condición a condición
                                if (et.conditions[x].or_And) { // Compruebame todas las condiciones con filtro OR
                                    for (int y = 0; y < events.Count; y++) { // Y dime si esas condiciones estan o no complidas
                                        if (events[y].name == et.conditions[x].nameEventCondition) {
                                            if (et.conditions[x].state.ToString() == events[y].state.ToString()) {
                                                conditionsOR_OK++;
                                                break;
                                            } else {
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        } 

                        // Se ejecuta la resolución
                        if (conditionsAND_OK == conditionsAND) { // Si TODAS las condiciones con filtro AND están cumplidas correctamente
                            if (conditionsOR > 0) { // Se comprueba que existen condiciones con filtro OR
                                if (conditionsOR_OK > 0) { // Si se cumple almenos una condicion de OR, entra
                                    // Este evento se ejecuta
                                    ExecuteEvent(et);
                                }
                            } else {
                                // Este evento se ejecuta
                                ExecuteEvent(et);
                            }       
                        }
                        
                    } else {
                        // No tiene condiciones, por lo que son los primeros eventos que se ejecutan
                        ExecuteEvent(et);
                    }
                }
            }
            updateNow = false;
        }
    }
    #endregion

    #region ExecuteEvent() - Dirije la acción al receiver
    private void ExecuteEvent(EventTown et) {

        GameObject go = null;
        try { 
            go = eventReceiverScripts[et.idEventReceiver];
        } catch {
            Debug.LogErrorFormat("ERROR EVENTO: El evento " + et.name + " no tiene un receptor correcto.");
        }

        switch (et.action) {
            case EventTown.Actions.Wait:
                if (CheckReceiverIsCorrect(et, "null")) { 
                    go.GetComponent<EventReceiver>().LoadWait(et.idEvent, et.timeToWait);
                    UpdateEvent(et.idEvent, EventTown.States.InProgress);
                }
                break;
            case EventTown.Actions.WaitLookingObject:
                if (CheckReceiverIsCorrect(et, "null")) {
                    go.GetComponent<EventReceiver>().LoadWait(et.idEvent, et.timeToWait, et.rotationToward);
                    UpdateEvent(et.idEvent, EventTown.States.InProgress);
                }
                break;
            case EventTown.Actions.WaitWithRotation:
                if (CheckReceiverIsCorrect(et, "null")) {
                    go.GetComponent<EventReceiver>().LoadWait(et.idEvent, et.timeToWait, et.rotationGlobal);
                    UpdateEvent(et.idEvent, EventTown.States.InProgress);
                }
                break;
            case EventTown.Actions.Move:
                if (CheckReceiverIsCorrect(et, "null")) {
                    go.GetComponent<EventReceiver>().LoadMove(et.idEvent, et.velocityMove, et.wayPoint);
                    UpdateEvent(et.idEvent, EventTown.States.InProgress);
                }
                break;
            case EventTown.Actions.Patrol:
                if (CheckReceiverIsCorrect(et, "null")) {
                    go.GetComponent<EventReceiver>().LoadPatrol(et.idEvent, et.velocityMove, et.wayPoints);
                    UpdateEvent(et.idEvent, EventTown.States.InProgress);
                }
                //--------------------------------------------------------------------------> ¿LOS EVENTOS PUEDEN CADUCAR?
                break;
            case EventTown.Actions.DialogueChange:
                if (CheckReceiverIsCorrect(et, "null")) {
                    go.GetComponentInChildren<Broadcaster>().nomenclature = et.dialogueNomenclature;
                    UpdateEvent(et.idEvent, EventTown.States.Completed);
                }
                break;
            case EventTown.Actions.DialogueComplete:
                if (CheckReceiverIsCorrect(et, "DialogueManager")) {
                    UpdateEvent(et.idEvent, EventTown.States.InProgress);
                    go.GetComponent<EventReceiver>().DialogueComplete(et.idEvent, et.dialogueNomenclature);
                }
                //--------------------------------------------------------------------------> ESTÁ DEASCTIVADO A FALTA SE SUBIRLO SIN COMFLICTOS. SE NECESITA TESTEO
                break;
            case EventTown.Actions.DialogueActive:
                if (CheckReceiverIsCorrect(et, "DialogueManager")) {
                    if (et.seeCharactersOnScreen) {
                        dialogueManager.LoadDialogue(et.dialogueNomenclature, true);
                    } else {
                        dialogueManager.LoadDialogue(et.dialogueNomenclature, false);
                    }
                    UpdateEvent(et.idEvent, EventTown.States.Completed);
                }
                break;
            case EventTown.Actions.GiveClue:
                if (CheckReceiverIsCorrect(et, "null")) {
                    persistentData.SaveClue(et.clueToGive);
                //--------------------------------------------------------------------------> ¿NOTIFICACIÓN POR PANTALLA DE QUE HA RECIBIDO LA PISTA?
                    UpdateEvent(et.idEvent, EventTown.States.Completed);
                }
                break;
            case EventTown.Actions.CheckClue:
                if (CheckReceiverIsCorrect(et, "InventoryManager")) {
                    go.GetComponent<EventReceiver>().CheckClue(et.idEvent, et.clueToCheck);
                    UpdateEvent(et.idEvent, EventTown.States.InProgress);
                }
                //--------------------------------------------------------------------------> ¿LOS EVENTOS PUEDEN CADUCAR?
                break;
            case EventTown.Actions.Animation:
                if (CheckReceiverIsCorrect(et, "null")) {
                    go.GetComponent<EventReceiver>().LoadAnimation(et.idEvent, et.animationTrigger);
                    UpdateEvent(et.idEvent, EventTown.States.Completed);
                }
                break;
            case EventTown.Actions.WhenPlayerEnterInThisTrigger:
                if (CheckReceiverIsCorrect(et, "null")) {
                    go.GetComponent<EventReceiver>().LoadTrigger(et.idEvent);
                    UpdateEvent(et.idEvent, EventTown.States.InProgress);
                }
                break;
            case EventTown.Actions.GameobjectActive:
                if (CheckReceiverIsCorrect(et, "null")) {
                    go.GetComponent<Collider>().enabled = et.activeGameObject;
                    go.transform.GetChild(0).gameObject.SetActive(et.activeGameObject);
                    UpdateEvent(et.idEvent, EventTown.States.Completed);
                }
                break;
            case EventTown.Actions.GameObjectActiveInScene:
                if (CheckReceiverIsCorrect(et, "null")) {
                    for (int i = 0; i < et.activeObjectsInScenes.Count; i++) {
                    et.activeObjectsInScenes[i].objectInScene.SetActive(et.activeObjectsInScenes[i].isActive);
                    }
                    UpdateEvent(et.idEvent, EventTown.States.Completed);
                }
                break;
            case EventTown.Actions.CheckClueInTrial:
                if (CheckReceiverIsCorrect(et, "null")) {
                    go.GetComponent<EventReceiver>().DemandClue(et.idEvent, et.clueToCheckInTrial);
                    UpdateEvent(et.idEvent, EventTown.States.InProgress);
                }
                break;
            case EventTown.Actions.StopPatrol:
                if (CheckReceiverIsCorrect(et, "null")) {
                    go.GetComponent<EventReceiver>().StopPatrol();
                }
                break;
            case EventTown.Actions.AdvanceTime:
                if (CheckReceiverIsCorrect(et, "TimeManager")) {
                    go.GetComponent<EventReceiver>().LoadAdvanceTime(et.idEvent);
                }
                break;
            case EventTown.Actions.CameraChange:
                if (CheckReceiverIsCorrect(et, "CameraManager")) {
                    go.GetComponent<EventReceiver>().LoadCameraChange(et.idEvent, et.cameraEnable, et.cameraDisable, et.camBlend, et.camTimeBlend);
                }
                break;
            case EventTown.Actions.DialogueLoop:
                if (CheckReceiverIsCorrect(et, "null")) {
                    go.GetComponent<EventReceiver>().LoadDialogueLoop(et.idEvent, et.dialoguesLoop);
                }
                break;
        }
    }
    #endregion

    #region CheckReceiverIsCorrect() - Comprueba si el receiver es el correcto o si existe en la escena.
    private bool CheckReceiverIsCorrect(EventTown et, string expectedReceiver) {

        if (expectedReceiver == "InventoryManager") {
            if(eventReceiverScripts[et.idEventReceiver].gameObject.name != "InventoryManager") {
                Debug.LogErrorFormat("ERROR EVENTO: El evento " + et.name +" espera un receptor de tipo InventoryManager.");
                return false;
            }
        } else if (expectedReceiver == "DialogueManager") {
            if (eventReceiverScripts[et.idEventReceiver].gameObject.name != "DialogueManager") {
                Debug.LogErrorFormat("ERROR EVENTO: El evento " + et.name + " espera un receptor de tipo DialogueManager.");
                return false;
            }
        } else if (expectedReceiver == "TimeManager") {
            if (eventReceiverScripts[et.idEventReceiver].gameObject.name != "TimeManager") {
                Debug.LogErrorFormat("ERROR EVENTO: El evento " + et.name + " espera un receptor de tipo TimeManager.");
                return false;
            }
        } else if (expectedReceiver == "CameraManager") {
            if (eventReceiverScripts[et.idEventReceiver].gameObject.name != "CameraManager") {
                Debug.LogErrorFormat("ERROR EVENTO: El evento " + et.name + " espera un receptor de tipo CameraManager.");
                return false;
            }
        } else if (expectedReceiver == "null") { // El receiver puede ser cualquiera
            if (eventReceiverScripts[et.idEventReceiver].gameObject.name == "InventoryManager") {
                Debug.LogErrorFormat("ERROR EVENTO: El evento " + et.name + " no puede usar un receptor de tipo InventoryManager.");
                return false;
            } else if (eventReceiverScripts[et.idEventReceiver].gameObject.name == "DialogueManager") {
                Debug.LogErrorFormat("ERROR EVENTO: El evento " + et.name + " no puede usar un receptor de tipo DialogueManager.");
                return false;
            } else if (eventReceiverScripts[et.idEventReceiver].gameObject.name == "TimeManager") {
                Debug.LogErrorFormat("ERROR EVENTO: El evento " + et.name + " no puede usar un receptor de tipo TimeManager.");
                return false;
            }
        }

        return true;
    }
    #endregion

    #region UpdateEvent - Actualiza el estado de los eventos
    // Actualiza el estado del evento que lo llama
    public void UpdateEvent(uint idEvent, EventTown.States state) {
        foreach (EventTown et in events) {
            if (et.idEvent == idEvent) {
                Debug.Log("Cambia estado");
                et.state = state;
                //Debug.Log("State" + et.state);
                break;
            }
        }
        updateNow = true;
    }
    #endregion
}
