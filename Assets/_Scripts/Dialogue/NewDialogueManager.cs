using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Null Reference Studios - All Rights Reserved


/*FLOW OF THE DIALOGUE MANAGER:
 * 
 * 1) We input a dialogue calling the "LoadDialogue" void, with the nomenclature of the desired dialogue.
 * 2) This void searchs that dialogue in the "dialogueList" list and loads the rows that this class contains.
 * 3) Then we check if the rows are part of a normal text display or a choice text dialogue.
 * 4) If it is a normal text display, we call the "TextRowQueue" coroutine, with the list of rows as an input.
 * 5) This coroutine waits por the player input, sending this rows one at a time as the player triggers the mentioned input.
 * 6) This rows are send to the "DisplayRow" coroutine, displaying the text into the canvas adding characters over time. We can control when to finish the display or let it be displayed step by step.
 * 7) When the text is done, we tell the "TextRowQueue" coroutine that it can send the next row of the dialogue.
 * 8) When the "TextRowQueue" coroutine is out of rows, the dialogue is interpretated as finished and the canvas deactivates, so does the coroutines.
 */

public class NewDialogueManager : MonoBehaviour
{
    #region VARIABLES
    //Variable para evitar que el jugador se mueva en los diálogos
    PlayerMovement playerMovement;
    //TextAsset of the dialogues
    [SerializeField]
    TextAsset dialogos;
    //List that saves all the rows of the text asset
    List<Row> rowList = new List<Row>();
    //List that saves all the dialogues of the text asset
    public List<Dialogue> dialogueList = new List<Dialogue>();
    //List of solved thoughts
    public List<string> solvedThoughtsList = new List<string>();
    //List of completed dialogues
    [SerializeField] List<string> completeDialogues = new List<string>();
    //Camera shake
    public CameraShake cameraShake;
    //Flash
    [SerializeField]
    GameObject flash;
    //Language of the dialogues
    DialogueVisuals dialogueVisuals;
    [SerializeField]
    Animator digVisAnim;
    PersistentData persistentData;
    EventTownManager eventManager;

    //Text GO that displays the text into the canvas
    [SerializeField]
    TextMeshProUGUI canvasText;
    //Broadcaster text
    [SerializeField]
    TextMeshProUGUI broadcasterText;
    //Canvas of the dialogue manager
  
    public GameObject dialogueContainer;
    [SerializeField]
    TextMeshProUGUI textThought;
    //List of text choices
    [SerializeField]
    List<Button> textChoices = new List<Button>();
    //Variable that tells if a row is being displayed at the moment
    bool displayingRow;
    //Variable that tells the "DisplayRow" coroutine to force display the entire text at that frame
    bool forceDisplay;
    //Variable that returns if there is a dialogue at the moment
    public bool dialogueInProgress;
    //Saves the nomenclature of the next dialogue
    public string nextDialogue;
    //Current branch
    string currentBranch;
    //Index of selected dialogue in a dialogue choice
    int dialogueIndex;
    //Value of the current blur of the thought
    public int blur;
    //current thought
    public string currentThought;
    //Already in dialogue
    bool dialogueVisActive = false;
    bool needVisuals;
    //Variable que estaba en la corrutina de las elecciones. Se ha sacado para poder usarla desde los métodos de los botones.
    bool loopChoices;
    //Lista que se usa para saber que branch coger desde el método de los botones
    public List<Row> currentBranchRowList = new List<Row>();
    public GameObject choicesContainer;
    //BLur GO
    [SerializeField]
    GameObject blurPost;

    //Audio
    AudioSource audioSource;

    //Interfaz
    public bool canAnimateInput = true;
    [SerializeField]
    Animator dialogueAnimator;
    [SerializeField]
    GameObject gizmoNextDialogue;
    public bool alreadyInDialogue;
    int broadcasterSide;
    public enum Side
    {
        left,
        right
    }
    public Side side;
    public bool changeBrodSides=true;
    public string refLeftSide;
    public string refRightSide;
    public string previousBrod;
    string currentBroadcaster;

    //Inventario
    public GameObject inventoryContainer;


    //Misc
    private TMP_Text m_TextComponent;
    private bool hasTextChanged;
    public bool onTrial=false;
    public bool onCinematic;

    //Cinematics
    public bool _cinNextRow = false;
    #endregion

    #region LoadDialoguesFromExcel
    // Start is called before the first frame update
    void Start()
    {
        //eventManager = FindObjectOfType<EventTownManager>();                   <------------------------------------------ACTIVAR
        persistentData = FindObjectOfType<PersistentData>();
        dialogueVisuals = GetComponent<DialogueVisuals>();
        //TMP
        m_TextComponent = canvasText.GetComponent<TMP_Text>();

        //Audio
        audioSource = GetComponent<AudioSource>();
        //Camera shake
        if (cameraShake == null)
        cameraShake = FindObjectOfType<CameraShake>();

        //Encuentra al jugador
        playerMovement = FindObjectOfType<PlayerMovement>();
        
        //We make sure that the canvas is deactivated
        dialogueContainer.SetActive(false);
        textChoices[0].gameObject.SetActive(false);
        textChoices[1].gameObject.SetActive(false);
        textChoices[2].gameObject.SetActive(false);
        textChoices[3].gameObject.SetActive(false);

        //This region reads the dialogue excel and saves it as dialogues, using the Row and Dialogue classes
        

        string[] fila = dialogos.text.Split(new char[] { '\n' });

        //Save the rows
        for (int i = 1; i < fila.Length - 1; i++)
        {
            string[] columna = fila[i].Split(new char[] { '@' });
            if (columna[0] != "")
            {
                Row row = new Row();
                row.nomenclature = columna[0];
                row.broadcaster = columna[1];
                row.dialogueChoice = columna[2];
                row.spanish = columna[3];
                row.english = columna[4];
                row.speed = columna[6];
                row.shake = columna[7];
                row.flash = columna[8];
                row.animation = columna[9];
                row.sound = columna[10];
                row.music = columna[11];

                string[] thoughts = columna[5].Split(new char[] { '/' });
                //set the blur value
                if (thoughts[0] != "")
                {
                    row.blur = int.Parse(thoughts[0]);
                }
                else
                {
                    row.blur = -1;
                }
                //Set the thought in spanish
                try
                {
                    if (thoughts[1] != "")
                    {
                        row.thoughtSp = thoughts[1];
                    }
                }
                catch
                {
                    row.thoughtSp = "";
                }
                //set the thought in english
                try
                {
                    if (thoughts[1] != "")
                    {
                        row.thoughtEn = thoughts[2].Remove(thoughts[2].Length - 2, 1);
                    }
                }
                catch
                {
                    row.thoughtEn = "";
                }

                rowList.Add(row);
            }
        }

        string indexDialogo = "";
        Dialogue dialogue = new Dialogue();

        //Save the dialogues
        for (int j = 0; j < rowList.Count; j++)
        {
            if (rowList[j].nomenclature != indexDialogo)
            {
                dialogueList.Add(dialogue);
                indexDialogo = rowList[j].nomenclature;
                dialogue = new Dialogue();
                dialogue.nomenclature = rowList[j].nomenclature;
                dialogue.rowList.Add(rowList[j]);
            }
            else
            {
                indexDialogo = rowList[j].nomenclature;
                dialogue.rowList.Add(rowList[j]);
            }
        }
        dialogueList.Add(dialogue);
        dialogueList.RemoveAt(0);

        //Save the broadcasters
        foreach (Dialogue d in dialogueList)
        {
            string brod1 = "";
            string brod2 = "";

            foreach (Row r in d.rowList)
            {
                if (r.broadcaster == "Vivianne")
                {
                    d.broadcaster_1 = "Vivianne";
                    foreach (Row s in d.rowList)
                    {
                        if (s.broadcaster != "Vivianne")
                        {
                            d.broadcaster_2 = s.broadcaster;
                            break;
                        }
                    }
                }
            }

            if (d.broadcaster_1 != "Vivianne")
            {
                foreach (Row r in d.rowList)
                {
                    if (r.broadcaster != "" && brod2 == "")
                    {
                        brod2 = r.broadcaster;
                    }
                    else if (r.broadcaster != "" && brod1 == "" && r.broadcaster != brod2)
                    {
                        brod1 = r.broadcaster;
                    }
                }
                d.broadcaster_1 = brod1;
                d.broadcaster_2 = brod2;
            }


        }
        
    }
    #endregion

    #region Check Dialogue Complete
    //public bool CheckDialogueComplete(string dialogueNomenclature)
    //{
    //    foreach (string s in completeDialogues)
    //    {
    //        if (s == dialogueNomenclature)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}


    public int CheckDialogueComplete(List<EventReceiver.CheckDialogue> dialogues)
    {

        for (int i = 0; i < completeDialogues.Count; i++)
        {
            for (int j = 0; j < dialogues.Count; j++)
            {
                if (completeDialogues[i] == dialogues[j].nomenclature)
                {
                    return (int)dialogues[j].idEventDialogue;
                }
            }
        }

        return -1;
    }
    #endregion

    #region Input dialogue
    //We call this void to input a dialogue into the manager
    public void LoadDialogue(string dialogueNomenclature, bool visuals)
    {
        
        //gizmo interfaz
        gizmoNextDialogue.SetActive(false);
        //Paramos el tiempo
        if (!onTrial && !onCinematic)
        {
            Time.timeScale = 0;
        }
        //Checkeamos si hay que activar los visual o no
        needVisuals = visuals;
        //Hace que el jugador no pueda moverse
        if (playerMovement != null)
        {
            playerMovement.PlayerCantMove();
        }
        //Reset canvas
        //canvasText.gameObject.SetActive(false);
        textChoices[0].gameObject.SetActive(false);
        textChoices[1].gameObject.SetActive(false);
        textChoices[2].gameObject.SetActive(false);
        textChoices[3].gameObject.SetActive(false);
        StopAllCoroutines();
        //We search a dialogue in our list that matches the nomenclature input
        Dialogue currentDialogue = null;
        foreach (Dialogue d in dialogueList)
        {
            if (d.nomenclature == dialogueNomenclature)
            {
                currentDialogue = d;
            }
        }


        //If the current dialogue is null we log an error, meaning the nomenclature it's incorrect.

        //Activate the canvas
        dialogueContainer.SetActive(true);

       

        if (!alreadyInDialogue)
            {
               
                refLeftSide = currentDialogue.broadcaster_1;
                refRightSide = currentDialogue.broadcaster_2;
                alreadyInDialogue = true;
            }

        //Reset the dialogue link nomenclature
        nextDialogue = "";

            List<Row> rowList = new List<Row>();

            //We save the Row info into a list
            foreach (Row r in currentDialogue.rowList)
            {
                rowList.Add(r);
            }

            //If the row list isn't empty we call the coroutine that manages the row input into the text displayer
            if (rowList.Count > 0)
            {
                if (rowList[0].nomenclature[0].Equals('E'))
                {
                    StartCoroutine("DialogueChoice", rowList);
                    currentBranch = rowList[0].nomenclature;
                }
                else
                {
                if (changeBrodSides && !onTrial)
                {
                    if (currentDialogue.rowList[0].broadcaster.Equals(currentDialogue.broadcaster_1))
                    {
                        dialogueAnimator.SetTrigger("InLeft");
                        side = Side.left;
                        
                    }

                    if (currentDialogue.rowList[0].broadcaster.Equals(currentDialogue.broadcaster_2))
                    {
                        dialogueAnimator.SetTrigger("InRight");
                        side = Side.right;
                       
                    }

                    currentBroadcaster = currentDialogue.rowList[0].broadcaster;
                    ChangeBroadcaster();
                    previousBrod = rowList[0].broadcaster;
                    changeBrodSides = false;
                }

                if (changeBrodSides && onTrial)
                {
                    dialogueAnimator.SetTrigger("InLeft");
                    changeBrodSides = false;
                }

                if (!dialogueVisActive && needVisuals)
                {
                    dialogueVisuals.LoadCharacters(currentDialogue.broadcaster_1, currentDialogue.broadcaster_2);
                    digVisAnim.SetTrigger("In");
                    dialogueVisActive = true;
                    //Blur
                    blurPost.SetActive(true);
                    dialogueVisuals.LoadAnimations(rowList[0].broadcaster, "idle");

                }

                StartCoroutine(TextRowQueue(rowList, dialogueNomenclature));
                

                    
                    
                }
            }

    }
    #endregion

    #region Text Row Queue
    //This coroutine manages the rows that form the current dialogue, sending them to the text displayer as we demand
    IEnumerator TextRowQueue(List<Row> rowList, string dialogueNomenclature)
    {
        canvasText.gameObject.SetActive(true);
        //We call the DisplayRow coroutine sending one row at a time, deleting them from the local list as we do
        StartCoroutine("DisplayRow", rowList[0]);
        currentBroadcaster = rowList[0].broadcaster;
        if (onTrial)
        {
            ChangeBroadcaster();
        }
        
        if (rowList.Count == 1)
        {
            if (rowList[0].dialogueChoice != "")
            {
                nextDialogue = rowList[0].dialogueChoice;
            }
        }


        //Interfaz
        if (rowList[0].animation != "" && needVisuals)
        {
            dialogueVisuals.LoadAnimations(rowList[0].broadcaster, rowList[0].animation);

        }

        if (needVisuals && rowList[0].broadcaster != previousBrod && !onTrial)
        {
            print(previousBrod);
            if (rowList[0].broadcaster == refLeftSide)
            {
                dialogueAnimator.SetTrigger("ToLeft");
                side = Side.left;
                
            }

            if (rowList[0].broadcaster == refRightSide)
            {
                dialogueAnimator.SetTrigger("ToRight");
                side = Side.right;
              
            }
            

        }
        previousBrod = rowList[0].broadcaster;
        

       rowList.RemoveAt(0);

        //Set the dialogue variable to true
        dialogueInProgress = true;

        

        //If the local list is empty that means that the dialogue has finished, ending the coroutine
        while (dialogueInProgress)
        {
            if (Input.GetKey(KeyCode.Return) || Input.GetButtonDown("A") && !inventoryContainer.activeInHierarchy || _cinNextRow)
            {
                _cinNextRow = false;
                Input.ResetInputAxes();
                //DisplayingRow is a variable that tells if a text is being displayed at the moment, so we can chech if we can send another row to the coroutine
                if (!displayingRow)
                {
                   
                    //If one row is left, we check if there is another dialogue after, if not the dialogue ends
                    if (rowList.Count == 1)
                    {
                        if (rowList[0].dialogueChoice != "")
                        {
                            nextDialogue = rowList[0].dialogueChoice;
                            print(rowList[0].dialogueChoice);
                        }
                    }

                    //If there's not any row left, we check if there is another in queue or we have to end the dialogue
                    try
                    {
                        StartCoroutine(DisplayRow(rowList[0]));
                        currentBroadcaster = rowList[0].broadcaster;

                       
                    }
                    catch
                    {
                        dialogueInProgress = false;

                        if (nextDialogue != "")
                        {
                            //animator
                            canAnimateInput = false;

                            LoadDialogue(nextDialogue, needVisuals);
                            completeDialogues.Add(dialogueNomenclature);

                        }
                        else
                        {
                            if (!onTrial || onCinematic)
                            { 
                                dialogueAnimator.SetTrigger("End");
                                if (needVisuals)
                                {
                                    digVisAnim.SetTrigger("Out");
                                }
                            }
                            completeDialogues.Add(dialogueNomenclature);
                        }
                        break;
                    }

                    //Dialogue visuals animations
                    if (rowList[0].animation != "" && needVisuals)
                    {
                        dialogueVisuals.LoadAnimations(rowList[0].broadcaster, rowList[0].animation);
                       
                    }

                    if (needVisuals && rowList[0].broadcaster != previousBrod && !onTrial)
                    {
                        if (rowList[0].broadcaster == refLeftSide)
                        {
                            dialogueAnimator.SetTrigger("ToLeft");
                            side = Side.left;
                           
                        }

                        if (rowList[0].broadcaster == refRightSide)
                        {
                            dialogueAnimator.SetTrigger("ToRight");
                            side = Side.right;
                          
                        }
                       

                    }
                    previousBrod = rowList[0].broadcaster;


                    rowList.RemoveAt(0);

                    //Visual Dialogue
                }
            }
            yield return null;
        }
    }
    #endregion

    #region Display Row
    //This coroutine takes a row as an input, displaying it at the canvas character to character, stopping when the sentence is done 
    IEnumerator DisplayRow(Row row)
    {
        if (onTrial)
        {
            ChangeBroadcaster();
        }
        gizmoNextDialogue.SetActive(false);
        bool loop = true;
        StopCoroutine("AnimateVertexColors");
        displayingRow = true;
        float speedDisplay=0.01f;
        blur = 100;

        #region Speed
        if (row.speed != "")
        {
            switch (row.speed[0])
            {
                case 's':
                    speedDisplay = 0.1f;
                    break;
                case 'f':
                    speedDisplay = 0.01f;
                    break;
            }
        }
        #endregion

        #region Camera Shake
        
        switch(row.shake)
        {
            case "*":
                cameraShake.LoadCameraShake(0.5f, 0.14f, 2);
                break;
            case "**":
                cameraShake.LoadCameraShake(1f, 0.14f, 5);
                break;
            case "***":
                cameraShake.LoadCameraShake(1.5f, 0.14f, 15);
                break;
        }

        #endregion

        #region Flash

        switch(row.flash)
        {
            case "*":
                flash.GetComponent<Animator>().SetTrigger("SoftFlash");
                break;
            case "**":
                flash.GetComponent<Animator>().SetTrigger("MediumFlash");
                break;
            case "***":
                flash.GetComponent<Animator>().SetTrigger("HardFlash");
                break;
        }

        #endregion

        #region Thought
        //We reference the language variable to load the desired language string of the row class
        switch (persistentData.language)
        {
            //Spanish
            case PersistentData.Langague.Spanish:
                canvasText.text = row.spanish;
                if (row.thoughtSp != "")
                { 
                    textThought.text = row.thoughtSp;
                    currentThought = row.thoughtSp;
                }
                break;

            //English
            case PersistentData.Langague.English:
                canvasText.text = row.english;
                if (row.thoughtEn != "")
                {
                    textThought.text = row.thoughtEn;
                    currentThought = row.thoughtSp;
                }
                break;
        }

        //Set the blur value
        if (row.blur >= 0)
        {
            blur = row.blur;
            textThought.color = new Color(1, 1, 1, (100 - blur) / 100f);
            if (blur == 0)
            {
                bool found = false;
                foreach (string s in solvedThoughtsList)
                {
                    if (s == currentThought)
                    {
                        found = true;
                    }
                }
                if (!found)
                {
                    solvedThoughtsList.Add(currentThought);
                }
            }
        }

        if (currentThought != "Escondido árbol muerto")
        {
            textThought.color = new Color(1, 1, 1, 0);
        }
        //foreach (string s in solvedThoughtsList)
        //{
        //    if (s == currentThought && s != "")
        //    {
        //        blur = 0;
        //        textThought.color = new Color(1, 1, 1, (100 - blur) / 100f);
        //        break;
        //    }
        //}
        #endregion

        #region Display Row
        //Mostrar caracteres
        m_TextComponent.ForceMeshUpdate();

        TMP_TextInfo textInfo = m_TextComponent.textInfo;

        int totalVisibleCharacters = textInfo.characterCount; // Get # of Visible Character in text object
        int visibleCount = 0;
        m_TextComponent.ForceMeshUpdate();
    
        while (loop)
        {
            float dotWait = 0;

            try
            {
                switch (m_TextComponent.textInfo.characterInfo[visibleCount-1].character)
                {
                    case '.':
                        dotWait = 0.2f;
                        break;
                    case ',':
                        dotWait = 0.2f;
                        break;
                    case ';':
                        dotWait = 0.2f;
                        break;
                    case '?':
                        dotWait = 0.2f;
                        break;
                    case '!':
                        dotWait = 0.2f;
                        break;
                    default:
                        break;
                }
            }
            catch
            {

            }

            if (hasTextChanged)
            {
                totalVisibleCharacters = textInfo.characterCount; // Update visible character count.
                hasTextChanged = false;
            }

            m_TextComponent.maxVisibleCharacters = visibleCount; // How many characters should TextMeshPro display?
            audioSource.Play();

            visibleCount += 1;

            if (m_TextComponent.maxVisibleCharacters == m_TextComponent.textInfo.characterCount)
            {
                loop = false;
                gizmoNextDialogue.SetActive(true);
            }

            if (forceDisplay && !onTrial)
            {
                m_TextComponent.maxVisibleCharacters = m_TextComponent.textInfo.characterCount;
                loop = false;
                gizmoNextDialogue.SetActive(true);
                speedDisplay = 0;
                dotWait = 0;
            }

            

            yield return new WaitForSecondsRealtime(speedDisplay + dotWait);
        }
        //As the sentence is done, we reset the displayingRow and forceDisplay variables

        displayingRow = false;
        forceDisplay = false;
        #endregion
    }
    #endregion

    #region Dialogue links
    IEnumerator DialogueChoice(List<Row> rowList)
    {
        //Reset the variables
        dialogueIndex = 0;
        //Añade la lista de la corrutina a currentBranchRowList para que esté disponible para los métodos de los botones
        currentBranchRowList = rowList;
        //Set the broadcaster as null
        //broadcasterText.text = "";

        //activates the necesary text GO for the dialogue links
        for (int i = 0; i < rowList.Count; i++)
        {
            textChoices[i].gameObject.SetActive(true);
            switch (persistentData.language)
            {
                case PersistentData.Langague.Spanish:
                    textChoices[i].GetComponentInChildren<TextMeshProUGUI>().text = rowList[i].spanish;
                    
                    break;
                case PersistentData.Langague.English:
                    textChoices[i].GetComponentInChildren<TextMeshProUGUI>().text = rowList[i].english;
                    break;
            }

        }

        
        //We hold the coroutine here until the player selects a dialogue link
        choicesContainer.SetActive(true);

        yield return null;

        
        
    }
    #endregion

    #region Choices
    public void Choice1()
    {
        completeDialogues.Add(currentBranch);
        LoadDialogue(currentBranchRowList[0].dialogueChoice, needVisuals);
        choicesContainer.SetActive(false);
        loopChoices = false;
    }
    public void Choice2()
    {
        completeDialogues.Add(currentBranch);
        LoadDialogue(currentBranchRowList[1].dialogueChoice, needVisuals);
        choicesContainer.SetActive(false);
        loopChoices = false;
    }
    public void Choice3()
    {
        completeDialogues.Add(currentBranch);
        LoadDialogue(currentBranchRowList[2].dialogueChoice, needVisuals);
        choicesContainer.SetActive(false);
        loopChoices = false;
    }
    public void Choice4()
    {
        completeDialogues.Add(currentBranch);
        LoadDialogue(currentBranchRowList[3].dialogueChoice, needVisuals);
        choicesContainer.SetActive(false);
        loopChoices = false;
    }

    #endregion

    #region Update
    private void Update()
    {
        ForceDisplay();

        if (Input.GetKeyDown(KeyCode.J))
        {
            CinNextRow();
        }
    }
    #endregion

    #region Force display
    //This void checks if a text is being displayed at the moment. If so, we traduce the "next row input" to force to finish the current row
    void ForceDisplay()
    {
        if (Input.GetKey(KeyCode.Return) || Input.GetButtonDown("A") && displayingRow && !inventoryContainer.activeInHierarchy)
        {
          
            forceDisplay = true;
        }
    }
    #endregion

    #region CloseDialogue
    public void CloseCanvas()
    {
        if (!onTrial)
        {
            dialogueContainer.SetActive(false);
        }
      
        //Devuelve el control al jugador
        if (playerMovement != null)
        {
            playerMovement.PlayerCanMove();
        }
        dialogueVisActive = false;

        Time.timeScale = 1;
        if (blurPost != null)
        {
            blurPost.SetActive(false);
        }
        if (onTrial)
        {
            canAnimateInput = false;
        }
        else
        {
            canAnimateInput = true;
        }
        changeBrodSides = true;
        alreadyInDialogue = false;
        currentThought = "";
    }
    #endregion

    #region Change broadcaster
    public void ChangeBroadcaster()
    {
        broadcasterText.text = currentBroadcaster;
    }
    #endregion

    public void CinNextRow()
    {
        _cinNextRow = true;
    }
}
