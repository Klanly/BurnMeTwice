using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cinemachine;

public class EventReceiver : MonoBehaviour {

    #region Variables
    //Link to EventTownManager
    EventTownManager eventTownManager;

    //Dialogue Manager
    NewDialogueManager dialogueManager;

    //Current event
    uint currentIdEvent;

    //Move action
    Vector3 currentPointToMove;

    //Wait action
    float timer;
    float timerControl;
    GameObject currentRotationTowards;
    Vector3 currentRotation;

    //Patrol action
    List<Vector3> currentWaypoints;
    int currentWaypoint;
    bool keepPatrol;

    //DemandClue action
    ScriptableObjectClueClass currentClue;
    Inventory inventory;
    GameObject dialogueCanvas;
    bool tryAgain = false;

    // CheckClue action
    List<CheckClues> listClues = new List<CheckClues>();
    public class CheckClues {
        public uint idEvetnClue;
        public ScriptableObjectClueClass clue;
    }

    //DialogueComplete
    List<CheckDialogue> listDialogues = new List<CheckDialogue>();
    public class CheckDialogue {
        public uint idEventDialogue;
        public string nomenclature;
    }
    public string currentNomenclature; //ELIMINAR SI FUNCIONA EL CÓDIGO NUEVO

    //Advance Time
    TimeManager timeManager;

    //Camera Change
    int camEnable, camDisable;
    EventTownManager.EventTown.CameraBlend camBlend;
    float camSec;

    //Components
    NavMeshAgent _nma;
    Collider _col;

    PersistentData persistentData;

    enum Action {
        _null,
        Move,
        Wait,
        Patrol,
        DemandClue,
        CheckClue,
        DialogueComplete,
        AdvanceTime,
        CameraChange
    }
    Action action;
    #endregion

    #region LoadWait(GameObject) - Un NPC espera en una posición mirando hacia un objecto
    //Para que un NPC espere en una posición, mirando hacia un objeto
    public void LoadWait(uint idEvent, float time, GameObject rotationTowards) {
        //Set the id event
        currentIdEvent = idEvent;
        currentRotationTowards = rotationTowards;
        //Set the timer
        timer = time;
        timerControl = 0.5f;
        if (time <= 0)
            Debug.LogError("LoadWait cannot take a negative time value as an argument");
        //Activate the update
        action = Action.Wait;
    }
    #endregion

    #region LoadWait(Vector3) - Un NPC espera en una posición mirando hacia una dirección
    //Para que un NPC espere en una posición, mirando hacia una direccion global
    public void LoadWait(uint idEvent, float time, Vector3 rotation) {
        //Set the id event
        currentIdEvent = idEvent;
        currentRotation = rotation;
        //Set the timer
        timer = time;
        timerControl = 0.5f;
        if (time <= 0)
            Debug.LogError("LoadWait cannot take a negative time value as an argument");
        //Activate the update
        action = Action.Wait;
    }
    #endregion

    #region LoadWait() - Un NPC esperará un tiempo en su posición
    //Para que un NPC espere en una posición
    public void LoadWait(uint idEvent, float time) {
        //Set the id event
        currentIdEvent = idEvent;
        //Set the timer
        timer = time;
        timerControl = timer * 2;
        if (time <= 0)
            Debug.LogError("LoadWait cannot take a negative time value as an argument");
        //Activate the update
        action = Action.Wait;
    }
    #endregion

    #region LoadMove() - Para mover un NPC desde los puntos A-B
    //Para mover a un NPC a una posición mediante navmesh
    public void LoadMove(uint idEvent, float speed, Vector3 destination) {
        //Set the id event
        currentIdEvent = idEvent;
        currentPointToMove = destination;
        //Set the navmesh agent speed
        try {
            GetComponent<NavMeshAgent>().speed = speed;
        } catch {
            Debug.LogError("The LoadMove game object hasn't have a NavMesh agent component");
        }
        //Set the destination
        try {
            GetComponent<NavMeshAgent>().SetDestination(destination);
        } catch {
            Debug.LogError("The LoadMove game object hasn't have a NavMesh agent component");
        }
        //Activate the update
        action = Action.Move;
        print("MoveReceiver" + gameObject.name);
    }
    #endregion

    #region LoadPatrol() - Para definir un bucle de movimiento en un NPC - SIN TERMINAR
    //Para definir la patrulla de un NPC
    public void LoadPatrol(uint idEvent, float speed, List<Vector3> wayPoints) {
        //Set the id event
        currentIdEvent = idEvent;
        //Set the navmesh agent speed
        try {
            GetComponent<NavMeshAgent>().speed = speed;
        } catch {
            Debug.LogError("The LoadMove game object hasn't have a NavMesh agent component");
        }
        //Reset the waypoint id
        currentWaypoint = 0;
        //Set th first destination
        _nma.SetDestination(wayPoints[currentWaypoint]);
        //Set the waypoints list
        currentWaypoints = wayPoints;
        //Set the bool
        keepPatrol = true;
        //Set the action enum
        action = Action.Patrol;
    }
    #endregion

    #region StopPatrol() - Para cortar un Patrol en progreso
    public void StopPatrol() {
        keepPatrol = false;
    }
    #endregion

    #region LoadAnimation() - Para cambiar una animación del un personaje
    //Para triggerear una animación
    public void LoadAnimation(uint idEvent, string animationTrigger) {
        //Set the id event
        currentIdEvent = idEvent;
        //Set the animation
        try {
            GetComponent<Animator>().SetTrigger(animationTrigger);
        } catch {
            Debug.LogError("The game object hasn't have an animator component, or the input trigger is incorrect");
        }
    }
    #endregion

    #region LoadTrigger() - Activa el trigger seleccionado
    //Para comprobar si un trigger ha sido accionado
    public void LoadTrigger(uint idEvent) {
        //Set the id event
        currentIdEvent = idEvent;
        //Enable the collider
        try {
            GetComponent<Collider>().enabled = true;
        } catch {
            Debug.LogError("The LoadTrigger game object hasn't have a collider component");
        }
    }
    #endregion

    #region DemandClue() - Para abrir el inventario en los Juicios
    // Este evento solo se usa en los juicios
    public void DemandClue(uint idEvent, ScriptableObjectClueClass clue) {
        //Set the id event
        currentIdEvent = idEvent;
        //Set the current clue
        currentClue = clue;
        //Open the inventory
        inventory.OpenInventoryTrial();
        //Block the inventory
        inventory.blockInventory = true;
        //Send the action to the update
        action = Action.DemandClue;
    }
    #endregion

    #region DialogueComplete() - Para comprobar que un dialogo se ha completado en pantalla
    public void DialogueComplete(uint idEvent, string nomenclature) {
        //Set the id event
        currentIdEvent = idEvent;
        currentNomenclature = nomenclature;

        CheckDialogue cd = new CheckDialogue {
            idEventDialogue = idEvent,
            nomenclature = nomenclature
        };

        listDialogues.Add(cd);
        action = Action.DialogueComplete;
    }
    #endregion

    #region CheckClue() - Se comprueba que el jugador tiene una pista en su inventario
    // Se usa durante el nivel jugable
    public void CheckClue(uint idEvent, ScriptableObjectClueClass newClue) {
        //Set the id event
        //currentIdEvent = idEvent;
        //Set the current clue
        //currentClue = clue;

        //-----------------------------------------------------------------------------------> NUEVO CÓDIGO PARA COMPROBAR MÁS DE UNA PISTA
        //CheckClues cc = new CheckClues {
        //    idEvetnClue = idEvent,
        //    clue = newClue
        //};
        //
        //listClues.Add(cc);

        //Send the action to the update
        action = Action.CheckClue;
    }
    #endregion

    #region LoadAdvanceTime() - Hace avanzar el tiempo de juego
    public void LoadAdvanceTime(uint idEvent) {
        currentIdEvent = idEvent;
        action = Action.AdvanceTime;
    }
    #endregion

    #region LoadCameraChange() - Cambia las camaras durante el juicio con la transición deseada
    public void LoadCameraChange(uint idEvent, int cameraEnable, int cameraDisable, EventTownManager.EventTown.CameraBlend cameraBlend, float seconds) {
        currentIdEvent = idEvent;
        camEnable = cameraEnable;
        camDisable = cameraDisable;
        camBlend = cameraBlend;
        camSec = seconds;
        action = Action.CameraChange;
    }
    #endregion

    #region LoadDialogueLoop() - Carga varios diálogos en loop
    public void LoadDialogueLoop(uint idEvent, List<EventTownManager.EventTown.DialoguesInLoop> dialoguesLoop) {
        // QUEDA CARGAR LA LISTA EN ESTE SCRIPT Y GENERAR LA FUNCION DE LA ACCIÓN.
        // CONSULTAR EL TRELLO PARAMAS INFORMACIÓN
    }
    #endregion

    private void Awake() {
        try {
            eventTownManager = FindObjectOfType<EventTownManager>();
        } catch {
            Debug.LogError("There's not an event manager in the scene. You cannot instantiate an event receiver without an event manager");
        }

        try {
            dialogueManager = FindObjectOfType<NewDialogueManager>();
        } catch {
            Debug.LogError("There's not an dialogue manager in the scene.");
        }

        try {
            persistentData = FindObjectOfType<PersistentData>();
        } catch {
            Debug.LogError("No hay persistent data en la escena.");
        }

        if (GetComponent<NavMeshAgent>() != null) {
            _nma = GetComponent<NavMeshAgent>();
        }

        if (GetComponent<Collider>() != null) {
            _col = GetComponent<Collider>();
        }

        if (GetComponent<Inventory>() != null) {
            inventory = GetComponent<Inventory>();
            if (inventory != null)
                dialogueCanvas = inventory.dialogueContainer;
        }

        if (GetComponent<TimeManager>() != null) {
            timeManager = GetComponent<TimeManager>();
        }

    }

    private void Update() {
        switch (action) {
            case Action.Move:
                #region Move
                if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(currentPointToMove.x, 0, currentPointToMove.z)) < 0.3f) {
                    eventTownManager.UpdateEvent(currentIdEvent, EventTownManager.EventTown.States.Completed);
                    action = Action._null;
                }
                #endregion
                break;
            case Action.Wait:
                #region Wait
                timerControl -= Time.deltaTime;
                if (timerControl <= 0) {
                    if (currentRotationTowards != null) {
                        try {
                            transform.LookAt(currentRotationTowards.transform);
                            transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
                        } catch {
                            Debug.LogError("The rotationTowards argument isn't valid");
                        }
                        timerControl = timer + 2;
                        currentRotationTowards = null;
                    } else {
                        try {
                            transform.LookAt(currentRotation);
                            transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
                        } catch {
                            Debug.LogError("The rotation argument isn't valid");
                        }
                        timerControl = timer + 2;
                    }

                }

                timer -= Time.deltaTime;
                if (timer <= 0) {
                    eventTownManager.UpdateEvent(currentIdEvent, EventTownManager.EventTown.States.Completed);
                    action = Action._null;
                }
                #endregion
                break;
            case Action.Patrol:
                #region Patrol
                if (keepPatrol) {
                    print(currentWaypoint);
                    print(currentWaypoints[currentWaypoint]);
                    if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(currentWaypoints[currentWaypoint].x, 0, currentWaypoints[currentWaypoint].z)) < 0.1f) {
                        currentWaypoint++;
                        if (currentWaypoint > currentWaypoints.Count - 1) {
                            currentWaypoint = 0;
                        }

                        _nma.SetDestination(currentWaypoints[currentWaypoint]);
                    }
                } else {
                    eventTownManager.UpdateEvent(currentIdEvent, EventTownManager.EventTown.States.Completed);
                    action = Action._null;
                }
                // ----------------------------------------------------------------------------> ¿LA ACTUALIZACIÓN DEL ESTADO DEL EVENTO?
                #endregion
                break;
            case Action.DemandClue:
                #region Demand Clue
                if (Input.GetButtonDown("A")) {
                    if (inventory.selectedClue == currentClue) {
                        eventTownManager.UpdateEvent(currentIdEvent, EventTownManager.EventTown.States.Completed);
                        inventory.blockInventory = false;
                        inventory.CloseInventoryTrial();
                        action = Action._null;
                    } else {
                        inventory.blockInventory = false;
                        inventory.CloseInventoryTrial();
                        inventory.blockInventory = true;
                        dialogueManager.LoadDialogue("D1_Ev1", false);
                        tryAgain = true;
                    }
                }

                if (tryAgain && !dialogueCanvas.activeInHierarchy) {
                    tryAgain = false;
                    inventory.blockInventory = false;
                    inventory.OpenInventoryTrial();
                    inventory.blockInventory = true;
                }
#endregion
                break;
            case Action.CheckClue:
                #region Check Clue
                //-----------------------------------------------------------------------------------> NUEVO CÓDIGO PARA COMPROBAR MÁS DE UNA PISTA
                if (listClues.Count > 0) {
                    for (int i = 0; i < persistentData.cluesInInventory.Count; i++) {
                        for (int j = 0; j < listClues.Count; j++) {
                            if (persistentData.cluesInInventory[i] == listClues[j].clue) {
                                eventTownManager.UpdateEvent(listClues[j].idEvetnClue, EventTownManager.EventTown.States.Completed);
                                listClues.Remove(listClues[j]);
                            }
                        }
                    }
                } else {
                    action = Action._null;
                }

                //-----------------------------------------------------------------------------------> ANTIGUO CÓDIGO
                //for (int i = 0; i < persistentData.cluesInInventory.Count; i++) {
                //    if (persistentData.cluesInInventory[i] == currentClue) {
                //        eventTownManager.UpdateEvent(currentIdEvent, EventTownManager.EventTown.States.Completed);
                //        action = Action._null;
                //    }
                //}
#endregion
                break;
            case Action.DialogueComplete:
                #region Dialogue Complete
                //-------------------------------------------------------------------------------------> PARA COMPROBAR SI FUNCIONA
                print("listDialogues.Count: " + listDialogues.Count);
                if (listDialogues.Count > 0) {
                    print("Entra 1");
                    int dial = dialogueManager.CheckDialogueComplete(listDialogues);
                    if (dial > -1) {
                        print("Entra 2");
                        for (int i = 0; i < listDialogues.Count; i++) {
                            if (listDialogues[i].idEventDialogue == dial) {
                                listDialogues.Remove(listDialogues[i]);
                                eventTownManager.UpdateEvent((uint)dial, EventTownManager.EventTown.States.Completed);
                            }
                        }
                    }
                } else {
                    action = Action._null;
                }

                //----------------------------------------------------------------------------------------> LO ANTIGUO
                //if (dialogueManager.CheckDialogueComplete(listDialogues)) {
                //    eventTownManager.UpdateEvent(currentIdEvent, EventTownManager.EventTown.States.Completed);
                //    action = Action._null;
                //}

                //-----------------------------------------------------------------------------------------> LO QUE VA EN NEWDIALOGUEMANAGER
                //public void int CheckDialogueComplete(List<EventReceiver.CheckDialogue> dialogues)
                //{

                //    for (int i = 0; i < completeDialogues.Count; i++)
                //    {
                //        for (int j = 0; j < dialogues.Count; j++)
                //        {
                //            if (completeDialogues[i] == dialogues[j].nomenclature)
                //            {
                //                return (int)dialogues[j].idEventDialogue;
                //            }
                //        }
                //    }

                //    return -1;
                //}
#endregion
                break;
            case Action.AdvanceTime:
                timeManager.Advance();
                eventTownManager.UpdateEvent(currentIdEvent, EventTownManager.EventTown.States.Completed);
                break;
            case Action.CameraChange:
                #region CameraChange
                switch (camBlend) {
                    case EventTownManager.EventTown.CameraBlend.Cut:
                        gameObject.transform.GetChild(0).GetComponent<CinemachineBrain>().m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
                        break;
                    case EventTownManager.EventTown.CameraBlend.EaseInOut:
                        gameObject.transform.GetChild(0).GetComponent<CinemachineBrain>().m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
                        gameObject.transform.GetChild(0).GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = camSec;
                        break;
                    case EventTownManager.EventTown.CameraBlend.EaseIn:
                        gameObject.transform.GetChild(0).GetComponent<CinemachineBrain>().m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseIn;
                        gameObject.transform.GetChild(0).GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = camSec;
                        break;
                    case EventTownManager.EventTown.CameraBlend.EaseOut:
                        gameObject.transform.GetChild(0).GetComponent<CinemachineBrain>().m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseOut;
                        gameObject.transform.GetChild(0).GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = camSec;
                        break;
                    case EventTownManager.EventTown.CameraBlend.HardIn:
                        gameObject.transform.GetChild(0).GetComponent<CinemachineBrain>().m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.HardIn;
                        gameObject.transform.GetChild(0).GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = camSec;
                        break;
                    case EventTownManager.EventTown.CameraBlend.HardOut:
                        gameObject.transform.GetChild(0).GetComponent<CinemachineBrain>().m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.HardOut;
                        gameObject.transform.GetChild(0).GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = camSec;
                        break;
                    case EventTownManager.EventTown.CameraBlend.Linear:
                        gameObject.transform.GetChild(0).GetComponent<CinemachineBrain>().m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Linear;
                        gameObject.transform.GetChild(0).GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = camSec;
                        break;
                }
                gameObject.transform.GetChild(1).GetChild(camDisable).gameObject.SetActive(false);
                gameObject.transform.GetChild(1).GetChild(camEnable).gameObject.SetActive(true);
                eventTownManager.UpdateEvent(currentIdEvent, EventTownManager.EventTown.States.Completed);
                #endregion
                break;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<PlayerMovement>() != null) {
            eventTownManager.UpdateEvent(currentIdEvent, EventTownManager.EventTown.States.Completed);
        }
    }
}
