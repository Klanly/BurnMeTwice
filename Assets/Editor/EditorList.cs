using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Cinemachine;

public static class EditorList {


    private static GUIContent
        moveUpButtonContent = new GUIContent("\u25B2", "Move up"),
        moveDownButtonContent = new GUIContent("\u25BC", "Move down"),
        deleteButtonContent = new GUIContent("\u2717", "Delete");

    #region Show - Muestra una configuración personalizada de listas (solo para  mostrar eventos)
    // Este método sirve para crear una visualización personalizada de una lista. Con esto podremos
    // ocultar la propiedad Size de las listas y evitar así que se pueda modificar manualmente.
    public static void Show(SerializedObject item, string[] eventsName, string[] receiversName, string[] cameraName, GameObject cameraManager) {
        // Para el procedimiento necesitamos usar un SerializedObject ("EventTownManager") del que
        // cogeremos la propiedad que necesitemos, en este caso, la lista de events.
        SerializedProperty list = item.FindProperty("events");

        // A partir de aquí, son las funciones necesarias para que se visualice al gusto 
        if (list.arraySize > 0) {
            // Esta funcion sirve para mostrar la propiedad tal y como es, además de actualizar su resultado
            // fuera del playmode sin la necesidad de usar el "EditorUtility.SetDirty()".
            EditorGUILayout.PropertyField(list);
            // Controlamos si la lista se expande o se colapsa, Si se expande, se mostrará 
            if (list.isExpanded) {
                // Añade una pequeña sangria
                EditorGUI.indentLevel += 1;
                // Recorremos el array de events: las listas no funcionan aquí, solo se usan arrays.
                for (int i = 0; i < list.arraySize; i++) {

                    // Se recoge la variable que se va a usar
                    SerializedProperty name = item.FindProperty("events.Array.data[" + i + "].name");
                    SerializedProperty receiver = item.FindProperty("events.Array.data[" + i + "].idEventReceiver");
                    SerializedProperty conditions = item.FindProperty("events.Array.data[" + i + "].conditions");
                    SerializedProperty actions = item.FindProperty("events.Array.data[" + i + "].action");
                    SerializedProperty state = item.FindProperty("events.Array.data[" + i + "].state");

                    // Muestra cada elemento de la lista Events
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
                    EditorGUI.indentLevel += 1;
                    if (list.GetArrayElementAtIndex(i).isExpanded) {
                        // Muestra la propiedad NAME
                        EditorGUILayout.PropertyField(name);

                        // Muestra la propiedad RECEIVER
                        //System.Array.Reverse(receiversName);
                        int index = receiver.intValue;
                        if (actions.enumValueIndex != 13 && actions.enumValueIndex != 8 && actions.enumValueIndex != 7) { // GameobjectActiveInScene -- GiveClue -- DialogueActive
                            if (actions.enumValueIndex == 14 || actions.enumValueIndex == 9) { // CheckClueInTrial -- CheckClue
                                for (int a = 0; a < receiversName.Length; a++) {
                                    if (receiversName[a] == "InventoryManager") {
                                        index = a;
                                        index = EditorGUILayout.Popup("Receiver", index, receiversName, EditorStyles.popup);
                                        receiver.intValue = index;
                                        break;
                                    }
                                }
                            } else if(actions.enumValueIndex == 6) { // DialogComplete
                                for (int a = 0; a < receiversName.Length; a++) {
                                    if (receiversName[a] == "DialogueManager") {
                                        index = a;
                                        index = EditorGUILayout.Popup("Receiver", index, receiversName, EditorStyles.popup);
                                        receiver.intValue = index;
                                        break;
                                    }
                                }
                            } else if (actions.enumValueIndex == 16) { // Advance Time
                                for (int a = 0; a < receiversName.Length; a++) {
                                    if (receiversName[a] == "TimeManager") {
                                        index = a;
                                        index = EditorGUILayout.Popup("Receiver", index, receiversName, EditorStyles.popup);
                                        receiver.intValue = index;
                                        break;
                                    }
                                }
                            } else if (actions.enumValueIndex == 17) { // Camera Change
                                for (int a = 0; a < receiversName.Length; a++) {
                                    if (receiversName[a] == "CameraManager") {
                                        index = a;
                                        index = EditorGUILayout.Popup("Receiver", index, receiversName, EditorStyles.popup);
                                        receiver.intValue = index;
                                        break;
                                    }
                                }
                            } else {
                                index = EditorGUILayout.Popup("Receiver", index, receiversName, EditorStyles.popup);
                                receiver.intValue = index;
                            }
                        }

                        // Muestra la propiedad CONDITIONS
                        ShowConditions(item, i, conditions, eventsName);
                        if (GUILayout.Button("Create Condition")) {
                            // Añadir un evento vacio
                            AddEmptyCondition(item, i, conditions);
                        }

                        // Muestra la propiedad ACTIONS
                        int op = actions.enumValueIndex;
                        op = EditorGUILayout.Popup("Action", op, actions.enumNames, EditorStyles.popup);
                        ShowActionVariables(op, item, i, cameraName, cameraManager);
                        actions.enumValueIndex = op;

                        // Muestra la propiedad STATE
                        EditorGUILayout.PropertyField(state);

                    }
                    EditorGUI.indentLevel -= 1;
                    // Muestra los botones disponibles en el método
                    ShowButtons(list, i);
                }
            }
        }
    }
    #endregion

    #region ShowConditions - Muestra la lista de Condiciones dentro de un evento
    // Como en este caso se necesita mostrar una lista dentro de una lista, tenemos que crear una nueva visualizacion de esa lista
    // la forma más cómoda es crear una nuevo método que se encarge de la visualizacion de esta lista "inception"
    // Funciona igual que la anterior lista.
    private static void ShowConditions(SerializedObject item, int index, SerializedProperty condition, string[] eventsName) {

        if (condition.arraySize > 0) {
            EditorGUILayout.PropertyField(condition);
            EditorGUI.indentLevel += 1;
            if (condition.isExpanded) {
                for (int i = 0; i < condition.arraySize; i++) {
                    SerializedProperty nameEventCondition = item.FindProperty("events.Array.data[" + index + "].conditions.Array.data[" + i + "].nameEventCondition");
                    SerializedProperty indexPopupCondition = item.FindProperty("events.Array.data[" + index + "].conditions.Array.data[" + i + "].indexPopupCondition");
                    SerializedProperty or_And = item.FindProperty("events.Array.data[" + index + "].conditions.Array.data[" + i + "].or_And");
                    SerializedProperty stateToStart = item.FindProperty("events.Array.data[" + index + "].conditions.Array.data[" + i + "].state");

                    EditorGUILayout.PropertyField(condition.GetArrayElementAtIndex(i));
                    if (condition.GetArrayElementAtIndex(i).isExpanded) {
                        // Muestra si es una condicion OR o AND
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Filter", GUILayout.Width(130));
                        string filter;
                        if (or_And.boolValue) {
                            filter = "= OR";
                        } else {
                            filter = "= AND";
                        }
                        bool fl = EditorGUILayout.ToggleLeft(filter, or_And.boolValue);
                        or_And.boolValue = fl;
                        EditorGUILayout.EndHorizontal();

                        // Muestra la propiedad EVENTO
                        int indexCondition = indexPopupCondition.intValue;
                        indexCondition = EditorGUILayout.Popup("Event", indexCondition, eventsName, EditorStyles.popup);
                        nameEventCondition.stringValue = eventsName[indexCondition];
                        indexPopupCondition.intValue = indexCondition;

                        // Muestra la propiedad ESTADO
                        EditorGUILayout.PropertyField(stateToStart);

                        // Muestra el boton para borrar esa condicion
                        ShowButtons(condition, i);
                    }
                }
            }
            EditorGUI.indentLevel -= 1;
        }

    }
    #endregion

    #region AddEmptyCondition - Crea una nueva Condición
    // Crea una nueva condición con los valores por defecto.
    private static void AddEmptyCondition(SerializedObject item, int index, SerializedProperty condition) {
        condition.arraySize++;
        SerializedProperty nameCondition = item.FindProperty("events.Array.data[" + index + "].conditions.Array.data[" + (condition.arraySize - 1) + "].nameCondition");
        nameCondition.stringValue = "Condition " + condition.arraySize;
    }
    #endregion

    #region ShowActionVariables - Muestra las variables necesarias según acción
    // Selecciona las variables que necesita la acción seleccionada
    private static void ShowActionVariables(int op, SerializedObject item, int index, string[] cameraName, GameObject cameraManager) {

        SerializedProperty timeToWait = item.FindProperty("events.Array.data[" + index + "].timeToWait");
        SerializedProperty rotationToward = item.FindProperty("events.Array.data[" + index + "].rotationToward");
        SerializedProperty rotationGlobal = item.FindProperty("events.Array.data[" + index + "].rotationGlobal");
        SerializedProperty velocityMove = item.FindProperty("events.Array.data[" + index + "].velocityMove");
        SerializedProperty wayPoint = item.FindProperty("events.Array.data[" + index + "].wayPoint");
        SerializedProperty dialogueNombenclature = item.FindProperty("events.Array.data[" + index + "].dialogueNomenclature");
        SerializedProperty seeCharactersOnScreen = item.FindProperty("events.Array.data[" + index + "].seeCharactersOnScreen");
        SerializedProperty clueToGive = item.FindProperty("events.Array.data[" + index + "].clueToGive");
        SerializedProperty clueToCheck = item.FindProperty("events.Array.data[" + index + "].clueToCheck");
        SerializedProperty animationTrigger = item.FindProperty("events.Array.data[" + index + "].animationTrigger");
        SerializedProperty activeGameObject = item.FindProperty("events.Array.data[" + index + "].activeGameObject");
        SerializedProperty clueToCheckInTrial = item.FindProperty("events.Array.data[" + index + "].clueToCheckInTrial");
        SerializedProperty cameraEnable = item.FindProperty("events.Array.data[" + index + "].cameraEnable");
        SerializedProperty cameraDisable = item.FindProperty("events.Array.data[" + index + "].cameraDisable");
        SerializedProperty cameraBlend = item.FindProperty("events.Array.data[" + index + "].camBlend");
        SerializedProperty cameraTimeBlend = item.FindProperty("events.Array.data[" + index + "].camTimeBlend");
        SerializedProperty cameraBlendIndex = item.FindProperty("events.Array.data[" + index + "].camBlendIndex");
        SerializedProperty dialoguesLoop = item.FindProperty("events.Array.data[" + index + "].dialoguesLoop");
        
        EditorGUI.indentLevel += 1;
        switch (op) {
            case 0: // Wait
                EditorGUILayout.PropertyField(timeToWait, new GUIContent("Time"));
                break;
            case 1: // WaitLookingObject
                EditorGUILayout.PropertyField(timeToWait, new GUIContent("Time"));
                EditorGUILayout.PropertyField(rotationToward, new GUIContent("Object"));
                break;
            case 2: // WaitWithRotation
                EditorGUILayout.PropertyField(timeToWait, new GUIContent("Time"));
                EditorGUILayout.PropertyField(rotationGlobal, new GUIContent("Rotation"));
                break;
            case 3: // Move
                EditorGUILayout.PropertyField(velocityMove, new GUIContent("Velocity"));
                EditorGUILayout.PropertyField(wayPoint, new GUIContent("Point"));
                break;
            case 4: // Patrol
                EditorGUILayout.PropertyField(velocityMove, new GUIContent("Velocity"));
                ShowWayPoints(item, index);
                break;
            case 5: // DialogueChange
                EditorGUILayout.PropertyField(dialogueNombenclature, new GUIContent("Nomenclature"));
                break;
            case 6: // DialogueComplete
                EditorGUILayout.PropertyField(dialogueNombenclature, new GUIContent("Nomenclature"));
                break;
            case 7: // DialogueActive
                EditorGUILayout.PropertyField(dialogueNombenclature, new GUIContent("Nomenclature"));
                EditorGUILayout.PropertyField(seeCharactersOnScreen, new GUIContent("See Characters"));
                break;
            case 8: // GiveClue
                EditorGUILayout.PropertyField(clueToGive, new GUIContent("Clue"));
                break;
            case 9: // CheckClue
                EditorGUILayout.PropertyField(clueToCheck, new GUIContent("Clue"));
                break;
            case 10: // Animation
                EditorGUILayout.PropertyField(animationTrigger, new GUIContent("Animation"));
                break;
            case 11: // Player Enter In Trigger
                break;
            case 12: // Gameobject Active
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("State Object", GUILayout.Width(130));
                string state;
                if (activeGameObject.boolValue) {
                    state = "Is Active";
                } else {
                    state = "Not Active";
                }
                bool st = EditorGUILayout.ToggleLeft(state, activeGameObject.boolValue);
                activeGameObject.boolValue = st;
                EditorGUILayout.EndHorizontal();
                break;
            case 13: // Gameobject Active In Scene
                ShowActivateObjectsInScene(item, index);
                break;
            case 14: // Check Clue In Trial
                EditorGUILayout.PropertyField(clueToCheckInTrial, new GUIContent("Clue"));
                break;
            case 15: // Stop Patrol
                break;
            case 16: // Advance Time
                break;
            case 17: // Camera Change
                int j = cameraDisable.intValue;
                j = EditorGUILayout.Popup("Disable", j, cameraName, EditorStyles.popup);
                cameraDisable.intValue = j;
                int i = cameraEnable.intValue;
                i = EditorGUILayout.Popup("Enable", i, cameraName, EditorStyles.popup);
                cameraEnable.intValue = i;

                int x = cameraBlend.enumValueIndex;
                x = EditorGUILayout.Popup("Blend", x, cameraBlend.enumNames, EditorStyles.popup);
                cameraBlend.enumValueIndex = x;

                if (x != 0) {
                    EditorGUILayout.PropertyField(cameraTimeBlend, new GUIContent("Seconds"));
                }
                break;
            case 18: // Dialogue Loop
                ShowDialoguesInLoop(item, index);
                break;
        }
        EditorGUI.indentLevel -= 1;
    }
    #endregion

    #region ShowWayPoints() - Muestra los puntos disponibles para la accioón de patrulla
    // Muestra en una lista personalizada los puntos disponibles para crear un moviemiento en patrulla. Se puede añadir y eliminar con botones.
    private static void ShowWayPoints(SerializedObject item, int index) {
        SerializedProperty wayPoints = item.FindProperty("events.Array.data[" + index + "].wayPoints");

        if (wayPoints.arraySize > 0) {
            EditorGUILayout.PropertyField(wayPoints);
            EditorGUI.indentLevel += 1;
            if (wayPoints.isExpanded) {
                for (int i = 0; i < wayPoints.arraySize; i++) {
                    SerializedProperty wayPoint = item.FindProperty("events.Array.data[" + index + "].wayPoints.Array.data[" + i + "]");
                    EditorGUILayout.PropertyField(wayPoint, new GUIContent("Point " + i));
                }
            }
            EditorGUI.indentLevel -= 1;
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Point")) {
            wayPoints.arraySize++;
        }
        if (GUILayout.Button("Remove Point")) {
            wayPoints.arraySize--;
        }
        EditorGUILayout.EndHorizontal();
    }
    #endregion

    #region ShowActivateObjectsInScene() - Active/Desactiva los objectos de la escena
    // Muestra en una lista personalizada los puntos disponibles para crear un moviemiento en patrulla. Se puede añadir y eliminar con botones.
    private static void ShowActivateObjectsInScene(SerializedObject item, int index) {
        SerializedProperty activeObjectsInScenes = item.FindProperty("events.Array.data[" + index + "].activeObjectsInScenes");

        if (activeObjectsInScenes.arraySize > 0) {
            EditorGUILayout.PropertyField(activeObjectsInScenes);
            EditorGUI.indentLevel += 1;
            if (activeObjectsInScenes.isExpanded) {
                for (int i = 0; i < activeObjectsInScenes.arraySize; i++) {
                    SerializedProperty isActive = item.FindProperty("events.Array.data[" + index + "].activeObjectsInScenes.Array.data[" + i + "].isActive");
                    SerializedProperty objectInScene = item.FindProperty("events.Array.data[" + index + "].activeObjectsInScenes.Array.data[" + i + "].objectInScene");

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("State " + i, GUILayout.Width(130));
                    string state;
                    if (isActive.boolValue) {
                        state = "Is Active";
                    } else {
                        state = "Not Active";
                    }
                    bool st = EditorGUILayout.ToggleLeft(state, isActive.boolValue);
                    isActive.boolValue = st;
                    EditorGUILayout.EndHorizontal();

                    GameObject obj = (GameObject)objectInScene.objectReferenceValue;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Object " + i, GUILayout.Width(130));
                    obj = (GameObject)EditorGUILayout.ObjectField(objectInScene.objectReferenceValue, typeof(GameObject), true);
                    if (obj != null) {
                        if (obj.GetComponent<EventReceiver>()) {
                            objectInScene.objectReferenceValue = null;
                            Debug.LogWarning("Gameobject Active In Scene: No se puede incluir un objecto con un componente EventReceiver.");
                        } else {
                            objectInScene.objectReferenceValue = obj;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUI.indentLevel -= 1;
        } else {
            activeObjectsInScenes.arraySize++;
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Object")) {
            activeObjectsInScenes.arraySize++;
        }
        if (GUILayout.Button("Remove Object")) {
            activeObjectsInScenes.arraySize--;
        }
        EditorGUILayout.EndHorizontal();
    }
    #endregion

    #region ShowDialoguesInLoop() - Muestra una lista de los diálogos que se quieres tener en bucle
    // Muestra en una lista de las nomenclaturas de los diálogos que se quiere mantener en un personaje en bucle. Se puede añadir y eliminar con botones.
    private static void ShowDialoguesInLoop(SerializedObject item, int index) {
        SerializedProperty dialogueInLoop = item.FindProperty("events.Array.data[" + index + "].dialoguesLoop");

        if (dialogueInLoop.arraySize > 0) {
            EditorGUILayout.PropertyField(dialogueInLoop);
            EditorGUI.indentLevel += 1;
            if (dialogueInLoop.isExpanded) {
                for (int i = 0; i < dialogueInLoop.arraySize; i++) {
                    SerializedProperty visualizeCharacters = item.FindProperty("events.Array.data[" + index + "].dialoguesLoop.Array.data[" + i + "].visualizeCharacters");
                    SerializedProperty dialogueNomenclatureLoop = item.FindProperty("events.Array.data[" + index + "].dialoguesLoop.Array.data[" + i + "].dialogueNomenclatureLoop");

                    bool st = EditorGUILayout.ToggleLeft("See Characters ", visualizeCharacters.boolValue);
                    visualizeCharacters.boolValue = st;

                    EditorGUILayout.PropertyField(dialogueNomenclatureLoop, new GUIContent("Nomenclature"));
                }
            }
            EditorGUI.indentLevel -= 1;
        } else {
            dialogueInLoop.arraySize++;
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Object")) {
            dialogueInLoop.arraySize++;
        }
        if (GUILayout.Button("Remove Object")) {
            dialogueInLoop.arraySize--;
        }
        EditorGUILayout.EndHorizontal();
    }
    #endregion

    #region ShowButtons - Muestra los botones de mover (arriba y abajo) y eliminar. Se puede aplicar a cualquier array.
    // Estos botones añaden funcionalidad al editor, puediendo mover hacia arriba o abajo los eventos y eliminandolos de la lista.
    private static void ShowButtons(SerializedProperty list, int index) {
        EditorGUILayout.BeginHorizontal(); // Se usa para cuando necesitas mostrar varias cosas en una misma linea.
        if (GUILayout.Button(moveUpButtonContent)) {
            if (index - 1 > -1) {
                list.MoveArrayElement(index, index - 1);
            }
        }
        if (GUILayout.Button(moveDownButtonContent)) {
            if (index + 1 < list.arraySize) {
                list.MoveArrayElement(index, index + 1);
            }
        }
        if (GUILayout.Button(deleteButtonContent)) {
            if (list.arraySize > 0) {
                list.DeleteArrayElementAtIndex(index);
            }
        }
        EditorGUILayout.EndHorizontal();
    }
    #endregion
}
