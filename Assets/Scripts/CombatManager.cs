// Hexbreaker - Combat System
// Last modified: 06/25/23
// Notes:

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Specialized;

public enum BattleState { START, PLAYERTURN, QTE, ENEMYTURN, WON, LOST }
public class CombatManager : MonoBehaviour, IDataPersistence
{

    [Header("Talisman Data")]
    public List<string> activeTalismanNames; //These two lists run side-by-side, like a dictionary. But not using a dictionary
    public List<int> activeTalismanPotency; //because I hate those. And they hate me. -Dylan 8

    private int potencyToUse;

    //public bool talismanSelected; //To know when to close the talisman panel. -Dylan 8

    [SerializeField] GameObject talismanPanel;

    [Header("Curse Data")]
    private GameObject[] topCurses; //Self-explanatory. For randomization. -Dylan 2
    private GameObject[] midCurses;
    private GameObject[] botCurses;

    public static int cursesToTalismans = 0; //Amount of talismans to generate. -Dylan 3

    private bool rightRoomLocked; // Make a function that checks if there is a curse that should set this value to true at the end of the battle - Dan
    private bool leftRoomLocked;

    [Header("Scene Setup")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject[] enemyPrefabs;
    private GameObject enemyPrefab;
    private List<int> loadedEnemyID; // This number determines which of the enemyPrefabs[] prefab is used. Pass this data through the Overworld map by saving it upon selecting a node in that scene. - Dan

    [SerializeField] private TextMeshProUGUI battleText;
    [SerializeField] private GameObject helpMenu;
    [SerializeField] private GameObject endPanel;
    private GameObject winPanel;

    public Transform playerBattleStation;
    public Transform[] enemyBattleStations;

    [SerializeField] private BattleHUD playerHUD;
    [SerializeField] private BattleHUD enemyHUD;
    [SerializeField] private CanvasGroup enemyCanvas;

    [SerializeField] private Button breakButton;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button defendButton;

    private CreateTalismans talismanManager;

    private DataPersistenceManager dataManager;

    private LevelManager levelManager;

    private QTEManager eventManager;

    public GameObject breakMeterGO; //Since all of these are public, I figured I'd make mine public here too. -Dylan 1

    private BreakMeter bm; //This is to reference the script attached instead of fetching the component 20 times. -Dylan 1

    private OutlineWpopUpUI enemyShader;

    [Header("Battle Settings")]

    private int breakCharges = 0;
    [SerializeField] private float confirmTimer = 1.5f;
    private bool waitingForConfirm = false;
    public bool waitingForTalismanPick = false;
    public bool playerCancelled = false;

    public bool combatFinished = false;
    private bool playerDied = false;

    public Unit playerUnit;
    private Unit selectedEnemyUnit;
    private Unit actingEnemyUnit;
    private List<Unit> enemyUnits;

    private GameObject enemyGO;
    private GameObject enemyGO2;
    private GameObject enemyGO3;

    //private int currentEnemyTurn;

    private int defenseBoost = 0; // Used to temporarily increase player defense.
    private int turnCounter = 0; // Because some abilities will be gated by skill cooldowns, we'll use a counter. Turn counter always goes up on Player's turn. Please use to count turns on Talisman cooldowns - Dan

    public BattleState state;

    // Start is called before the first frame update
    void Start()
    {
        cursesToTalismans = 0;
        topCurses = GameObject.FindGameObjectsWithTag("CurseTop"); //For finding the curses. -Dylan 2
        midCurses = GameObject.FindGameObjectsWithTag("CurseMid");
        botCurses = GameObject.FindGameObjectsWithTag("CurseBot");

        winPanel = GameObject.Find("Win Panel");

        talismanManager = GameObject.Find("TalismanGenerator").GetComponent<CreateTalismans>();

        winPanel.SetActive(false);

        foreach (GameObject curse in topCurses) //Probably wildly inefficient, but hey. It works. -Dylan 2
        {
            curse.SetActive(false);
        }

        foreach (GameObject curse in midCurses)
        {
            curse.SetActive(false);
        }

        foreach (GameObject curse in botCurses)
        {
            curse.SetActive(false);
        }

        int amountOfCurses = Random.Range(1, 4);

        if (amountOfCurses == 1) //This is what randomizes the curses -Dylan 2
        {
            botCurses[Random.Range(0, botCurses.Length)].SetActive(true);
        }
        else if (amountOfCurses == 2)
        {
            botCurses[Random.Range(0, botCurses.Length)].SetActive(true);
            midCurses[Random.Range(0, midCurses.Length)].SetActive(true);
        }
        else if (amountOfCurses == 3)
        {
            botCurses[Random.Range(0, botCurses.Length)].SetActive(true);
            midCurses[Random.Range(0, midCurses.Length)].SetActive(true);
            topCurses[Random.Range(0, topCurses.Length)].SetActive(true);
        }

        levelManager = FindAnyObjectByType<LevelManager>();
        enemyShader = FindObjectOfType<OutlineWpopUpUI>();

        state = BattleState.START;

        bm = breakMeterGO.GetComponent<BreakMeter>(); //Fetches the script for the break meter. -Dylan 1
        eventManager = FindAnyObjectByType<QTEManager>();

        StartCoroutine(SetupBattle());
    }

    private void Update()
    {
        if (waitingForConfirm)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                waitingForConfirm = false;
            }
        }


        if (waitingForTalismanPick)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                waitingForConfirm = false;
            }
        }

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) //on hit
        {
            GameObject selection = hit.collider.gameObject;
            if (selection.CompareTag("Enemy"))
            {
                enemyCanvas.alpha = 1;
                Unit hoveringUnit = selection.GetComponent<Unit>();
                enemyHUD.SetHP(hoveringUnit);

                if (state == BattleState.PLAYERTURN)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        enemyShader.ChangeShader(selection);
                        selectedEnemyUnit = hoveringUnit;
                    }
                }

            }

            else
            {
                enemyCanvas.alpha = 0;
            }
        }

        if (state == BattleState.PLAYERTURN)
        {
            if (Input.GetKeyDown("h"))
            {
                helpMenu.SetActive(true);
            }

            if (Input.GetKeyUp("h"))
            {
                helpMenu.SetActive(false);
            }
        }
    }

    IEnumerator ConfirmTimer()
    {
        waitingForConfirm = true;
        yield return new WaitForSeconds(confirmTimer);
        waitingForConfirm = false;
    }

    // Loads data needed to set up battle properly - Dan 
    public void LoadData(GameData gameData)
    {
        playerUnit = playerPrefab.GetComponent<Unit>();
        loadedEnemyID = gameData.enemyUnitToLoadID;

        playerUnit.unitName = gameData.playerUnitName;
        playerUnit.highDamage = gameData.playerHighDamage;
        playerUnit.midDamage = gameData.playerMidDamage;
        playerUnit.lowDamage = gameData.playerLowDamage;
        playerUnit.defense = gameData.playerDefense;
        playerUnit.maxHP = gameData.playerMaxHP;
        playerUnit.currentHP = gameData.playerCurrentHP;
    }

    // Passes data to the saved data file
    public void SaveData(GameData gameData)
    {
        gameData.playerUnitName = playerUnit.unitName;
        gameData.playerHighDamage = playerUnit.highDamage;
        gameData.playerMidDamage = playerUnit.midDamage;
        gameData.playerLowDamage = playerUnit.lowDamage;
        gameData.playerDefense = playerUnit.defense;
        gameData.playerMaxHP = playerUnit.maxHP;
        gameData.playerCurrentHP = playerUnit.currentHP;

        gameData.combatFinished = combatFinished;
        gameData.playerDied = playerDied;

        gameData.rightRoomLocked = rightRoomLocked;
        gameData.leftRoomLocked = leftRoomLocked;
    }

    IEnumerator SetupBattle() // This function sets up the battle
    {

        combatFinished = false;

        endPanel.SetActive(false);
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        enemyUnits = new List<Unit> { };
        //playerUnit = playerGO.GetComponent<Unit>(); // We are now reading the unit data from GameData when we load into the scene. - Dan

        enemyPrefab = enemyPrefabs[loadedEnemyID[0]];
        enemyGO = Instantiate(enemyPrefab, enemyBattleStations[0]);
        enemyUnits.Add(enemyGO.GetComponent<Unit>());

        if (loadedEnemyID.Count > 1)
        {
            enemyPrefab = enemyPrefabs[loadedEnemyID[1]];
            enemyGO2 = Instantiate(enemyPrefab, enemyBattleStations[1]);
            enemyUnits.Add(enemyGO2.GetComponent<Unit>());

            if (loadedEnemyID.Count > 2)
            {
                enemyPrefab = enemyPrefabs[loadedEnemyID[2]];
                enemyGO3 = Instantiate(enemyPrefab, enemyBattleStations[2]);
                enemyUnits.Add(enemyGO3.GetComponent<Unit>());
            }
        }

        selectedEnemyUnit = enemyUnits[0];
        enemyShader.ChangeShader(selectedEnemyUnit.gameObject);
        actingEnemyUnit = enemyUnits[0];

        battleText.text = "The battle has begun! " + actingEnemyUnit.unitName + " wants to fight!";

        enemyHUD.gameObject.SetActive(true);
        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(selectedEnemyUnit);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        //Curse check
        PlayerTurn();
    }

    private void PlayerTurn()
    {
        RefreshPlayerStatus();

        battleText.text = "Your Turn!";
        turnCounter += 1;
    }

    IEnumerator EnemyTurn()
    {
        state = BattleState.ENEMYTURN;
        // auto selects and shades acting enemy for player clarity
        selectedEnemyUnit = actingEnemyUnit;
        enemyShader.ChangeShader(selectedEnemyUnit.gameObject);

        if (!actingEnemyUnit.isStunned)
        {
            if (!actingEnemyUnit.isCharged)
            {
                battleText.text = "The " + actingEnemyUnit.unitName + " is deciding what to do";

                StartCoroutine(ConfirmTimer());
                yield return new WaitUntil(() => waitingForConfirm == false);

                int actionTaken = Random.Range(0, 8);
                switch (actionTaken)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                        StartCoroutine(EnemyAttack());
                        break;

                    case 4:
                    case 5:
                    case 6:
                        StartCoroutine(EnemyAttack2());
                        break;

                    case 7: StartCoroutine(EnemyCharge());
                        break;

                    default:
                        Debug.Log("ERROR! actionTaken in EnemyTurn coroutine not recognized. Defaulting to enemyAttack.");
                        StartCoroutine(EnemyAttack());
                        break;
                }
            }

            else
            {
                battleText.text = "The " + actingEnemyUnit.unitName + " is about to unleash a charged attack!";

                StartCoroutine(ConfirmTimer());
                yield return new WaitUntil(() => waitingForConfirm == false);

                StartCoroutine(EnemyChargedAttack());
            }
        }

        else
        {
            battleText.text = "The " + actingEnemyUnit.unitName + " can't act because it is stunned!";
            actingEnemyUnit.isStunned = false;
            StartCoroutine(ConfirmTimer());
            yield return new WaitUntil(() => waitingForConfirm == false);

            if (enemyUnits.Count > 2 && actingEnemyUnit == enemyUnits[1])
            {

                actingEnemyUnit = enemyUnits[2];
                StartCoroutine(EnemyTurn());
            }

            else if (enemyUnits.Count > 1 && actingEnemyUnit == enemyUnits[0])
            {
                actingEnemyUnit = enemyUnits[1];
                StartCoroutine(EnemyTurn());
            }


            else
            {
                actingEnemyUnit = enemyUnits[0];
                state = BattleState.PLAYERTURN;
                PlayerTurn();
            }
        }
    }

    private void EndBattle()
    {

        if (state == BattleState.WON)
        {
            battleText.text = "";
            GameObject camera = GameObject.Find("Main Camera"); //To change the camera position to look at the talismans -Dylan 3

            enemyGO.SetActive(false);

            camera.transform.position = new Vector3(1.019f, 1.812f, 1.466f);

            camera.transform.rotation = Quaternion.Euler(8.301f, -96.467f, 2.312f);
            /// Display choice
            //endPanel.SetActive(true); Changed -Dylan 3
            combatFinished = true;
            playerDied = false;

            GameObject.Find("Curses").SetActive(false);
            GameObject.Find("PlayerHUDPanel").SetActive(false);
            GameObject.Find("EnemyHUDPanel").SetActive(false);

            //Future me needs to set the action talismans to make the TalismanManager their parent object again,
            //and then set the gameobjects inactive. -Dylan 8

            winPanel.SetActive(true);
        }
        else if (state == BattleState.LOST)
        {
            StartCoroutine(KillPlayer());
        }

        else { battleText.text = "Battle state is wrong"; }
    }

    IEnumerator KillPlayer()
    {
        battleText.text = "You Died";

        playerDied = true;
        combatFinished = false;
        endPanel.SetActive(true);
        dataManager = FindObjectOfType<DataPersistenceManager>();
        dataManager.NewGame();
        yield return new WaitForSeconds(1f);
        waitingForConfirm = true;
        yield return new WaitUntil(() => waitingForConfirm == false);

        levelManager.LoadSceneWithName("MainMenu");
    }

    public void TalismanPicked()
    {
        combatFinished = true;
        levelManager.LoadSceneWithName("Overworld");
    }

    private void RefreshPlayerStatus()
    {
        if (playerUnit.isDefending)
        {
            defenseBoost = 0;
            playerUnit.isDefending = false;
        }

        eventManager.eventResult = QTEResult.NONE;

        if (BreakMeter.charge == 100)
        {
            breakButton.interactable = true;
        }

        else
        {
            breakButton.interactable = false;
        }

        attackButton.interactable = true;
        defendButton.interactable = true;

        enemyHUD.SetHUD(selectedEnemyUnit);

    }

    #region Talisman Checks

    public void HideTalisman() 
    {
        // Vanish the talismans that appear
    }

    private int FetchPotency(int index) //Just ever-so-slightly easier to type. -Dylan 8
    {
        return activeTalismanPotency[index];
    }

    private int FetchIndexOfName(string name) //Efficiently find the index of the name. -Dylan 8
    {
        if (activeTalismanNames.Contains(name))
        {
            return activeTalismanNames.FindIndex(a => a.Contains(name));
        }
        else
        {
            return 0; //Is never used when the index doesn't exist anyway, just needed a returning code path. -Dylan 8
        }
    }

    private bool CheckIfTalismanActive(string name) //I like parameters. -Dylan 8
    {
        if (activeTalismanNames.Contains(name))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    #region Player Actions
    IEnumerator PlayerAttack() // Deals damage to enemy and then checks if it is dead
    {

        if (talismanManager.action.Contains(true))
        {
            talismanPanel.SetActive(true);

            int amountOfActionTalismans = 0;

            for (int i = 0; i < talismanManager.talismans.Count; i++)
            {
                if (talismanManager.action[i])
                {
                    talismanManager.talismans[i].SetActive(true);
                    talismanManager.talismans[i].transform.SetParent(GameObject.Find("Canvas").transform, false);

                    if (amountOfActionTalismans == 0)
                    {
                        talismanManager.talismans[i].transform.position = new Vector3(500f, 500f, 0f);
                    }
                    else if (amountOfActionTalismans == 1)
                    {
                        talismanManager.talismans[i].transform.position = new Vector3(1000f, 500f, 0f);
                    }
                    else if (amountOfActionTalismans == 2)
                    {
                        talismanManager.talismans[i].transform.position = new Vector3(1500f, 500f, 0f);
                    }

                    amountOfActionTalismans++;
                }
            }

            waitingForTalismanPick = true;
            yield return new WaitUntil(() => waitingForTalismanPick == false);

            if (playerCancelled == true) 
            {
                state = BattleState.PLAYERTURN;
                yield break;
                
            }
        }

        state = BattleState.QTE;

        battleText.text = "You attack! Press the right key!";
        string attackType = playerUnit.attackType1; // change attackType to be either Standard, Mash, Timed, Array
        float timer = playerUnit.attackTimer1; // How much time alloted in QTE
        int keyToPress = playerUnit.keyToPress1; // Change to 1,2,3 or 4

        // Check for talisman. if last talisman used isn so and so, then change attackType to be either Standard, Mash, Timed, Array

        switch (attackType)
        {
            case "Timed":
                eventManager.TriggerTimedQTE(playerUnit.attackTimer1, playerUnit.keyToPress1);
                //Change color of QTE circle
                break;

            case "Mash":
                eventManager.TriggerMashQTE(playerUnit.attackTimer1, playerUnit.keyToPress1, playerUnit.fillGauge1);
                //Change color of QTE circle
                break;

            case "Array":
                eventManager.TriggerQTEArray(playerUnit.attackTimer1, playerUnit.keyToPressArray);
                //Change color of QTE circle
                break;

            case "Standard":
                eventManager.GenerateStandardQTE(playerUnit.attackTimer1);
                //Change color of QTE circle
                break;

            default:
                Debug.Log("ERROR! attackType not recognized. Defaulting to Standard attack");
                eventManager.GenerateStandardQTE(3);
                break;
        }


        yield return new WaitUntil(() => eventManager.eventCompleted == true);
        int damageDealt = 0;


        switch (eventManager.eventResult)
        {
            case QTEResult.NONE:
                print("ERROR! EventResult should not be NONE");
                break;
            case QTEResult.LOW:
                damageDealt = playerUnit.lowDamage;
                if (CheckIfTalismanActive("Conflicting")) 
                {
                    int extraDamage = FetchPotency(FetchIndexOfName("Conflicting")); // Here you set the extra damage from the talisman
                    damageDealt = playerUnit.midDamage + extraDamage;
                }
                
                battleText.text = "A weak hit! The " + selectedEnemyUnit.unitName + " takes " + damageDealt + " damage!";

                if (CheckIfTalismanActive("Purification"))
                {
                    bm.ChangeMeterValue(FetchPotency(FetchIndexOfName("Purification")));
                }
                else if (GameObject.Find("WeakAttackCurse") != null) //Changes the break charge based on the weak curse. -Dylan 2
                {
                    bm.ChangeMeterValue(4);
                }
                else
                {
                    bm.ChangeMeterValue(6);
                }
                break;
            case QTEResult.MID:
                damageDealt = playerUnit.midDamage;

                if (CheckIfTalismanActive("Contending"))
                {
                    int extraDamage = FetchPotency(FetchIndexOfName("Contending"));
                    damageDealt = playerUnit.highDamage + extraDamage;
                }

                battleText.text = "A hit! The " + selectedEnemyUnit.unitName + " takes " + damageDealt + " damage!";

                if (CheckIfTalismanActive("Purification"))
                {
                    bm.ChangeMeterValue(FetchPotency(FetchIndexOfName("Purification")));
                }
                else
                {
                    bm.ChangeMeterValue(12);
                }
                break;
            case QTEResult.HIGH:
                damageDealt = playerUnit.highDamage;

                if (CheckIfTalismanActive("Omnipotent"))
                {
                    int extraDamage = FetchPotency(FetchIndexOfName("Omnipotent"));
                    damageDealt = playerUnit.highDamage + extraDamage;
                }

                battleText.text = "A strong hit! The " + selectedEnemyUnit.unitName + " takes " + damageDealt + " damage!";

                if (CheckIfTalismanActive("Purification"))
                {
                    bm.ChangeMeterValue(FetchPotency(FetchIndexOfName("Purification")));
                }
                else if (GameObject.Find("DefendingCurse") != null) //Changes break charge based on the strong attack curse. -Dylan 2
                {
                    bm.ChangeMeterValue(50);
                }
                else if (GameObject.Find("PerfectAttackCurse") != null)
                {
                    bm.ChangeMeterValue(8);
                }
                else
                {
                    bm.ChangeMeterValue(18);
                }
                break;
            default:
                print("ERROR! EventResult is not recognized!");
                break;
        }

        bool isDead = false;

        if (CheckIfTalismanActive("Multistrike"))
        {
            foreach (Unit enemy in enemyUnits)
            {
                enemy.TakeDamage(damageDealt + FetchPotency(FetchIndexOfName("Multistrike")), false);  
            }

            if (selectedEnemyUnit.currentHP <= 0) 
            {
                isDead = true;
            }
        }

        else 
        {
            isDead = selectedEnemyUnit.TakeDamage(damageDealt, false);
        }

        if (CheckIfTalismanActive("Vampiric")) 
        {
            int amountToHeal = FetchPotency(FetchIndexOfName("Vampiric")); // Set this to the # the player should heal from the talisman
            playerUnit.heal(amountToHeal);
            playerHUD.SetHP(playerUnit);
        }
        
        enemyHUD.SetHP(selectedEnemyUnit);

        StartCoroutine(ConfirmTimer());
        yield return new WaitUntil(() => waitingForConfirm == false);

        activeTalismanNames.Clear();
        //Clears the two lists after they are used, as a sort of reset. -Dylan 8
        activeTalismanPotency.Clear();

        // Here begins endturn functionality
        if (isDead)
        {
            if (enemyUnits.Count > 1)
            {
                enemyUnits.Remove(selectedEnemyUnit);
                selectedEnemyUnit.gameObject.SetActive(false);
                selectedEnemyUnit = enemyUnits[0];
                actingEnemyUnit = enemyUnits[0];
                StartCoroutine(EnemyTurn());
            }

            else 
            {
                selectedEnemyUnit.gameObject.SetActive(false);
                state = BattleState.WON;
                EndBattle();
            }            
            // End the battle
        }

        else 
        {
            // Proceed to enemy's turn.
            actingEnemyUnit = enemyUnits[0];
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
        // Change state of battle based on result

    }

    IEnumerator PlayerDefend() 
    {
        if (talismanManager.action.Contains(true))
        {
            talismanPanel.SetActive(true);

            int amountOfActionTalismans = 0;

            for (int i = 0; i < talismanManager.talismans.Count; i++)
            {
                if (talismanManager.action[i])
                {
                    talismanManager.talismans[i].SetActive(true);
                    talismanManager.talismans[i].transform.SetParent(GameObject.Find("Canvas").transform, false);

                    if (amountOfActionTalismans == 0)
                    {
                        talismanManager.talismans[i].transform.position = new Vector3(500f, 500f, 0f);
                    }
                    else if (amountOfActionTalismans == 1)
                    {
                        talismanManager.talismans[i].transform.position = new Vector3(1000f, 500f, 0f);
                    }
                    else if (amountOfActionTalismans == 2)
                    {
                        talismanManager.talismans[i].transform.position = new Vector3(1500f, 500f, 0f);
                    }

                    amountOfActionTalismans++;
                }
            }

            waitingForTalismanPick = true;
            yield return new WaitUntil(() => waitingForTalismanPick == false);

            if (playerCancelled == true)
            {
                state = BattleState.PLAYERTURN;
                yield break;

            }           
        }

        playerUnit.isDefending = true;
        battleText.text = playerUnit.unitName + " takes a defensive stance!";

        StartCoroutine(ConfirmTimer());
        yield return new WaitUntil(() => waitingForConfirm == false);

        // If we create any effects that would damage the enemy upon defending, we can copy the PlayerAttack endturn functionality

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());

    }

    IEnumerator PlayerBreak()
    {
        // Break animation plays. It is an auto success!

        breakButton.interactable = false;
        BreakMeter.charge = 0;
        //Play the animation for the break button

        state = BattleState.QTE;

        battleText.text = "You attempt to break the enemy's curse! Press the right key!";
        string attackType = playerUnit.attackType2;

        switch (attackType)
        {
            case "Timed":
                eventManager.TriggerTimedQTE(playerUnit.attackTimer2, playerUnit.keyToPress2);
                //Change color of QTE circle
                break;

            case "Mash":
                eventManager.TriggerMashQTE(playerUnit.attackTimer2, playerUnit.keyToPress2, playerUnit.fillGauge2);
                //Change color of QTE circle
                break;

            case "Array":
                eventManager.TriggerQTEArray(playerUnit.attackTimer2, playerUnit.keyToPressArray);
                //Change color of QTE circle
                break;

            case "Standard":
                eventManager.GenerateStandardQTE(playerUnit.attackTimer2);
                //Change color of QTE circle
                break;

            default:
                Debug.Log("ERROR! attackType not recognized. Defaulting to Standard attack");
                eventManager.GenerateStandardQTE(3);
                break;
        }


        yield return new WaitUntil(() => eventManager.eventCompleted == true);
        int damageDealt = 0;

        if (eventManager.eventResult == QTEResult.HIGH)
        {
            damageDealt = playerUnit.highDamage;
        }

        else 
        {
            damageDealt = playerUnit.midDamage;
        }

        //

        battleText.text = "You break the curse imposed by " + selectedEnemyUnit.unitName + " and deal " + damageDealt + " damage!";

        cursesToTalismans += 1;      

        // Damage the enemy selected. Choose damage based on eventState;
        bool isDead = selectedEnemyUnit.TakeDamage(damageDealt, false);
        enemyHUD.SetHP(selectedEnemyUnit);

        StartCoroutine(ConfirmTimer());
        yield return new WaitUntil(() => waitingForConfirm == false);

        //bm.BreakCurse(GameObject.Find("ExampleCurse")); //Just placeholder, no coroutine needed yet since there's no curses. -Dylan 1

        bool bCurseActive = false; //Finds out what level of curse is active. This is to make sure the curses are broken in order. -Dylan 2
        bool mCurseActive = false;
        bool tCurseActive = false;

        foreach (GameObject curse in botCurses)
        {
            if (curse.activeSelf)
            {
                bCurseActive = true;
            }
        }

        foreach (GameObject curse in midCurses)
        {
            if (curse.activeSelf)
            {
                mCurseActive = true;
            }
        }

        foreach (GameObject curse in topCurses)
        {
            if (curse.activeSelf)
            {
                tCurseActive = true;
            }
        }

        if (bCurseActive)
        {
            foreach (GameObject curse in botCurses)
            {
                curse.SetActive(false);
                bm.BreakCurse();
            }
        }
        else if (mCurseActive)
        {
            foreach (GameObject curse in midCurses)
            {
                curse.SetActive(false);
                bm.BreakCurse();
            }
        }
        else if (tCurseActive)
        {
            foreach (GameObject curse in topCurses)
            {
                curse.SetActive(false);

                bm.BreakCurse();
            }
        }

        if (selectedEnemyUnit.isCharged)
        {
            selectedEnemyUnit.isCharged = false;
            selectedEnemyUnit.isStunned = true;
            battleText.text = "You interrupt the " + selectedEnemyUnit.unitName + " from charging its attack! " + selectedEnemyUnit.unitName + " is now stunned!";
        }

        else 
        {
            if (eventManager.eventResult != QTEResult.HIGH)            
            {
                PowerUpEnemy(selectedEnemyUnit);
                print("Break event result was not high, enemy powered up");
            }           
        }

        StartCoroutine(ConfirmTimer());
        yield return new WaitUntil(() => waitingForConfirm == false);


        // Here begins endturn functionality
        if (isDead)
        {
            if (enemyUnits.Count > 1)
            {
                enemyUnits.Remove(selectedEnemyUnit);
                selectedEnemyUnit.gameObject.SetActive(false);
                selectedEnemyUnit = enemyUnits[0];
                enemyShader.ChangeShader(selectedEnemyUnit.gameObject);
                actingEnemyUnit = enemyUnits[0];
                state = BattleState.ENEMYTURN;
                StartCoroutine(EnemyTurn());
            }

            else
            {
                selectedEnemyUnit.gameObject.SetActive(false);
                state = BattleState.WON;
                EndBattle();
            }
            // End the battle
        }

        else
        {
            // Proceed to enemy's turn.
            actingEnemyUnit = enemyUnits[0];
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
        // Change state of battle based on result
    }

    #endregion

    #region Enemy Actions
    IEnumerator EnemyAttack()
    {
        int damageDealt = Random.Range(1, 5); // Determines how effective the enemy's attack is

        switch (damageDealt)
        {
            case 1:
                damageDealt = actingEnemyUnit.lowDamage;
                break;

            case 2:
            case 3:
                damageDealt = actingEnemyUnit.midDamage;
                break;

            case 4:
                damageDealt = actingEnemyUnit.highDamage;
                break;

            default:
                print("ERROR! random.range outside possible bounds.");
                break;
        }

        battleText.text = "The " + actingEnemyUnit.unitName + " attacks! " + playerUnit.unitName + " takes " + damageDealt + " damage!";

        if (playerUnit.isDefending)
        {
            defenseBoost = 0; // Reset any previous defense gains in case player is attacked more than once on same turn.
            state = BattleState.QTE;
            battleText.text = "The " + actingEnemyUnit.unitName + " attacks! Block by pressing the right key!";

            string attackType = actingEnemyUnit.attackType1;

            switch (attackType) 
            {
                case "Timed":
                    eventManager.TriggerTimedQTE(actingEnemyUnit.attackTimer1, actingEnemyUnit.keyToPress1);
                    break;

                case "Mash":
                    eventManager.TriggerMashQTE(actingEnemyUnit.attackTimer1, actingEnemyUnit.keyToPress1, actingEnemyUnit.fillGauge1);
                    break;

                case "Array":
                    eventManager.TriggerQTEArray(actingEnemyUnit.attackTimer1, actingEnemyUnit.keyToPressArray);
                    break;

                case "Standard":
                    eventManager.GenerateStandardQTE(actingEnemyUnit.attackTimer1);
                    break;

                default:
                    Debug.Log("ERROR! attackType not recognized. Defaulting to Standard attack");
                    eventManager.GenerateStandardQTE(actingEnemyUnit.attackTimer1);
                    break;
            }


            // Start QTE. Reduce incoming damage based on degree of success by increasing player defense. Align QTE with enemy's attack animation.
            yield return new WaitUntil(() => eventManager.eventCompleted == true);

            switch (eventManager.eventResult)
            {
                case QTEResult.NONE:
                    print("ERROR! EventResult should not be NONE");
                    break;

                case QTEResult.LOW:
                    defenseBoost = 1;
                    battleText.text = "A weak block..." + playerUnit.unitName + " takes " + (Mathf.Clamp(damageDealt - defenseBoost, 1, damageDealt)) + " damage!";

                    if (GameObject.Find("WeakBlockCurse") != null) //Changes meter charge based on curse. -Dylan 2
                    {
                        bm.ChangeMeterValue(13);
                    }
                    else
                    {
                        bm.ChangeMeterValue(15);
                    }
                    break;

                case QTEResult.MID:
                    defenseBoost = 2;
                    battleText.text = "You block! " + playerUnit.unitName + " takes " + (Mathf.Clamp(damageDealt - defenseBoost, 1, damageDealt)) + " damage!";
                    break;

                case QTEResult.HIGH:
                    defenseBoost = 3;
                    battleText.text = "A perfect block! " + playerUnit.unitName + " takes " + (Mathf.Clamp(damageDealt - defenseBoost, 1, damageDealt)) + " damage!";

                    if (GameObject.Find("AttackingCurse") != null) //Changes meter charge based on curse. If the player can only defend, that should be the only curse to apply in a perfect QTE. -Dylan 2
                    {
                        bm.ChangeMeterValue(50);
                    }
                    else if (GameObject.Find("PerfectBlockCurse") != null)
                    {
                        bm.ChangeMeterValue(25);
                    }
                    else
                    {
                        bm.ChangeMeterValue(35);
                    }
                    break;

                default:
                    print("ERROR! EventResult is not recognized!");
                    break;
            }
            // Depending on degree of success, temporarily increase defense.
        }

        bool isDead = playerUnit.TakeDamage(Mathf.Clamp(damageDealt - defenseBoost, 1, damageDealt), false);
        playerHUD.SetHP(playerUnit);

        StartCoroutine(ConfirmTimer());
        yield return new WaitUntil(() => waitingForConfirm == false);

        if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }

        else
        {
            if (enemyUnits.Count > 2 && actingEnemyUnit == enemyUnits[1])
            {

                actingEnemyUnit = enemyUnits[2];
                StartCoroutine(EnemyTurn());
            }

            else if (enemyUnits.Count > 1 && actingEnemyUnit == enemyUnits[0])
            {
                actingEnemyUnit = enemyUnits[1];
                StartCoroutine(EnemyTurn());
            }


            else
            {
                actingEnemyUnit = enemyUnits[0];
                state = BattleState.PLAYERTURN;
                PlayerTurn();
            }
        }
    }

    IEnumerator EnemyAttack2() 
    {
        int damageDealt = Random.Range(1, 5); // Determines how effective the enemy's attack is

        switch (damageDealt)
        {
            case 1:
                damageDealt = actingEnemyUnit.lowDamage;
                break;

            case 2:
                damageDealt = actingEnemyUnit.midDamage;
                break;

            case 3:
            case 4:
                damageDealt = actingEnemyUnit.highDamage;
                break;

            default:
                print("ERROR! random.range outside possible bounds.");
                break;
        }

        battleText.text = "The " + actingEnemyUnit.unitName + " attacks! " + playerUnit.unitName + " takes " + damageDealt + " damage!";

        if (playerUnit.isDefending)
        {
            defenseBoost = 0; // Reset any previous defense gains in case player is attacked more than once on same turn.
            state = BattleState.QTE;
            battleText.text = "The " + actingEnemyUnit.unitName + " attacks! Block by pressing the right key!";

            string attackType = actingEnemyUnit.attackType2;

            switch (attackType)
            {
                case "Timed":
                    eventManager.TriggerTimedQTE(actingEnemyUnit.attackTimer2, actingEnemyUnit.keyToPress2);
                    break;

                case "Mash":
                    eventManager.TriggerMashQTE(actingEnemyUnit.attackTimer2, actingEnemyUnit.keyToPress2, actingEnemyUnit.fillGauge2);
                    break;

                case "Array":
                    eventManager.TriggerQTEArray(actingEnemyUnit.attackTimer2, actingEnemyUnit.keyToPressArray);
                    break;

                case "Standard":
                    eventManager.GenerateStandardQTE(actingEnemyUnit.attackTimer2);
                    break;

                default:
                    Debug.Log("ERROR! attackType not recognized. Defaulting to Standard attack");
                    eventManager.GenerateStandardQTE(actingEnemyUnit.attackTimer2);
                    break;
            }


            // Start QTE. Reduce incoming damage based on degree of success by increasing player defense. Align QTE with enemy's attack animation.
            yield return new WaitUntil(() => eventManager.eventCompleted == true);

            switch (eventManager.eventResult)
            {
                case QTEResult.NONE:
                    print("ERROR! EventResult should not be NONE");
                    break;

                case QTEResult.LOW:
                    defenseBoost = 1;
                    battleText.text = "A weak block..." + playerUnit.unitName + " takes " + (Mathf.Clamp(damageDealt - defenseBoost, 1, damageDealt)) + " damage!";

                    if (GameObject.Find("WeakBlockCurse") != null) //Changes meter charge based on curse. -Dylan 2
                    {
                        bm.ChangeMeterValue(13);
                    }
                    else
                    {
                        bm.ChangeMeterValue(15);
                    }
                    break;

                case QTEResult.MID:
                    defenseBoost = 2;
                    battleText.text = "You block! " + playerUnit.unitName + " takes " + (Mathf.Clamp(damageDealt - defenseBoost, 1, damageDealt)) + " damage!";
                    break;

                case QTEResult.HIGH:
                    defenseBoost = 3;
                    battleText.text = "A perfect block! " + playerUnit.unitName + " takes " + (Mathf.Clamp(damageDealt - defenseBoost, 1, damageDealt)) + " damage!";

                    if (GameObject.Find("AttackingCurse") != null) //Changes meter charge based on curse. If the player can only defend, that should be the only curse to apply in a perfect QTE. -Dylan 2
                    {
                        bm.ChangeMeterValue(50);
                    }
                    else if (GameObject.Find("PerfectBlockCurse") != null)
                    {
                        bm.ChangeMeterValue(25);
                    }
                    else
                    {
                        bm.ChangeMeterValue(35);
                    }
                    break;

                default:
                    print("ERROR! EventResult is not recognized!");
                    break;
            }
            // Depending on degree of success, temporarily increase defense.
        }

        bool isDead = playerUnit.TakeDamage(Mathf.Clamp(damageDealt - defenseBoost, 1, damageDealt), false);
        playerHUD.SetHP(playerUnit);

        StartCoroutine(ConfirmTimer());
        yield return new WaitUntil(() => waitingForConfirm == false);

        if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }

        else
        {
            if (enemyUnits.Count > 2 && actingEnemyUnit == enemyUnits[1])
            {

                actingEnemyUnit = enemyUnits[2];
                StartCoroutine(EnemyTurn());
            }

            else if (enemyUnits.Count > 1 && actingEnemyUnit == enemyUnits[0])
            {
                actingEnemyUnit = enemyUnits[1];
                StartCoroutine(EnemyTurn());
            }


            else
            {
                actingEnemyUnit = enemyUnits[0];
                state = BattleState.PLAYERTURN;
                PlayerTurn();
            }
        }
    }

    IEnumerator EnemyChargedAttack() 
    {
        int damageDealt;

        if (!playerUnit.isDefending)
        {
            damageDealt = actingEnemyUnit.highDamage + actingEnemyUnit.midDamage;
        }

        else 
        {
            damageDealt = actingEnemyUnit.highDamage + actingEnemyUnit.lowDamage;
        }

        battleText.text = "The " + actingEnemyUnit.unitName + " unleashes a charged attack! " + playerUnit.unitName + " takes " + damageDealt + " damage!";

        if (playerUnit.isDefending)
        {
            defenseBoost = 0; // Reset any previous defense gains in case player is attacked more than once on same turn.
            state = BattleState.QTE;
            battleText.text = "The " + actingEnemyUnit.unitName + " unleashes a charged attack! Block by pressing the right key!";

            string attackType = actingEnemyUnit.attackType2;

            switch (attackType)
            {
                case "Timed":
                    eventManager.TriggerTimedQTE(actingEnemyUnit.attackTimer2, actingEnemyUnit.keyToPress2);
                    break;

                case "Mash":
                    eventManager.TriggerMashQTE(actingEnemyUnit.attackTimer2, actingEnemyUnit.keyToPress2, actingEnemyUnit.fillGauge2);
                    break;

                case "Array":
                    eventManager.TriggerQTEArray(actingEnemyUnit.attackTimer2, actingEnemyUnit.keyToPressArray);
                    break;

                case "Standard":
                    eventManager.GenerateStandardQTE(actingEnemyUnit.attackTimer2);
                    break;

                default:
                    Debug.Log("ERROR! attackType not recognized. Defaulting to Standard attack with 3 sec timer");
                    eventManager.GenerateStandardQTE(3);
                    break;
            }


            // Start QTE. Reduce incoming damage based on degree of success by increasing player defense. Align QTE with enemy's attack animation.
            yield return new WaitUntil(() => eventManager.eventCompleted == true);

            switch (eventManager.eventResult)
            {
                case QTEResult.NONE:
                    print("ERROR! EventResult should not be NONE");
                    break;

                case QTEResult.LOW:
                    defenseBoost = 1;
                    battleText.text = "A weak block..." + playerUnit.unitName + " takes " + (Mathf.Clamp(damageDealt - defenseBoost, 1, damageDealt)) + " damage!";

                    if (GameObject.Find("WeakBlockCurse") != null) //Changes meter charge based on curse. -Dylan 2
                    {
                        bm.ChangeMeterValue(13);
                    }
                    else
                    {
                        bm.ChangeMeterValue(15);
                    }
                    break;

                case QTEResult.MID:
                    defenseBoost = 2;
                    battleText.text = "You block! " + playerUnit.unitName + " takes " + (Mathf.Clamp(damageDealt - defenseBoost, 1, damageDealt)) + " damage!";
                    break;

                case QTEResult.HIGH:
                    defenseBoost = 3;
                    battleText.text = "A perfect block! " + playerUnit.unitName + " takes " + (Mathf.Clamp(damageDealt - defenseBoost, 1, damageDealt)) + " damage!";

                    if (GameObject.Find("AttackingCurse") != null) //Changes meter charge based on curse. If the player can only defend, that should be the only curse to apply in a perfect QTE. -Dylan 2
                    {
                        bm.ChangeMeterValue(50);
                    }
                    else if (GameObject.Find("PerfectBlockCurse") != null)
                    {
                        bm.ChangeMeterValue(25);
                    }
                    else
                    {
                        bm.ChangeMeterValue(35);
                    }
                    break;

                default:
                    print("ERROR! EventResult is not recognized!");
                    break;
            }
            // Depending on degree of success, temporarily increase defense.
        }

        bool isDead = playerUnit.TakeDamage(Mathf.Clamp(damageDealt - defenseBoost, 1, damageDealt), false);
        playerHUD.SetHP(playerUnit);

        StartCoroutine(ConfirmTimer());
        yield return new WaitUntil(() => waitingForConfirm == false);

        if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }

        else
        {
            if (enemyUnits.Count > 2 && actingEnemyUnit == enemyUnits[1])
            {

                actingEnemyUnit = enemyUnits[2];
                StartCoroutine(EnemyTurn());
            }

            else if (enemyUnits.Count > 1 && actingEnemyUnit == enemyUnits[0])
            {
                actingEnemyUnit = enemyUnits[1];
                StartCoroutine(EnemyTurn());
            }


            else
            {
                actingEnemyUnit = enemyUnits[0];
                state = BattleState.PLAYERTURN;
                PlayerTurn();
            }
        }
    }

    IEnumerator SummonEnemy() 
    {
        StartCoroutine(ConfirmTimer());
        yield return new WaitUntil(() => waitingForConfirm == false);
        // TO DO
    }

    private void PowerUpEnemy(Unit enemy) 
    {
        
        enemy.lowDamage = enemy.lowDamage + enemy.strengthModifier;
        enemy.midDamage = enemy.midDamage + enemy.strengthModifier;
        enemy.highDamage = enemy.highDamage + enemy.strengthModifier;

        battleText.text = "The " + enemy.unitName + "'s rage has caused it to grow stronger!";
    }

    IEnumerator EnemyCharge() 
    {
        battleText.text = "The " + actingEnemyUnit.unitName + " is preparing to unleash its power!";
        actingEnemyUnit.isCharged = true;
        //Play animation or sound effects or whatever

        StartCoroutine(ConfirmTimer());
        yield return new WaitUntil(() => waitingForConfirm == false);

        if (enemyUnits.Count > 2 && actingEnemyUnit == enemyUnits[1])
        {

            actingEnemyUnit = enemyUnits[2];
            StartCoroutine(EnemyTurn());
        }

        else if (enemyUnits.Count > 1 && actingEnemyUnit == enemyUnits[0])
        {
            actingEnemyUnit = enemyUnits[1];
            StartCoroutine(EnemyTurn());
        }

        else
        {
            actingEnemyUnit = enemyUnits[0];
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

    #endregion

    #region Buttons
    public void OnAttackButton() 
    {
        if (state != BattleState.PLAYERTURN)
            return;

        playerCancelled = false;

        if (GameObject.Find("AttackingCurse") != null) //To prevent attacking. -Dylan 2
        {
            battleText.text = "The enemy's curse is preventing " + playerUnit.unitName + " from attacking!";
            attackButton.interactable = false;
            return;
        }

        StartCoroutine(PlayerAttack());
        state = BattleState.START;
    }

    public void OnCancelButton() 
    {
        playerCancelled = true;
        talismanPanel.SetActive(false);
        HideTalisman();
    }

    public void OnSkipButton() 
    {
        talismanPanel.SetActive(false);
        HideTalisman();
    }

    public void OnDefendButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        playerCancelled = false;

        if (GameObject.Find("DefendingCurse") != null) //To prevent defending. -Dylan 2
        {
            battleText.text = "The enemy's curse is preventing " + playerUnit.unitName + " from defending!";
            defendButton.interactable = false;
            return;
        }



        StartCoroutine(PlayerDefend());
        state = BattleState.START;
    }

    public void OnBreakButton()
    {

        if (state != BattleState.PLAYERTURN)
            return;

        // If player has action talismans, display talismans and allow player to choose before continuing

        StartCoroutine(PlayerBreak());
        state = BattleState.START;
    }

    #endregion
}
