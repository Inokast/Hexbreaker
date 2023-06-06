// Hexbreaker - Combat System
// Last modified: 06/04/23 - Dan Sanchez
// Notes:

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum BattleState { START, PLAYERTURN, QTE, ENEMYTURN, WON, LOST }
public class CombatManager : MonoBehaviour, IDataPersistence
{

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
    private int loadedEnemyID = 0; // This number determines which of the enemyPrefabs[] prefab is used. Pass this data through the Overworld map by saving it upon selecting a node in that scene. - Dan

    [SerializeField] private TextMeshProUGUI battleText;
    [SerializeField] private GameObject endPanel;
    private GameObject winPanel;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    [SerializeField] private BattleHUD playerHUD;
    [SerializeField] private BattleHUD enemyHUD;

    [SerializeField] private Button breakButton;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button defendButton;

    private DataPersistenceManager dataManager;

    private LevelManager levelManager;

    private QTEManager eventManager;

    public GameObject breakMeterGO; //Since all of these are public, I figured I'd make mine public here too. -Dylan 1

    private BreakMeter bm; //This is to reference the script attached instead of fetching the component 20 times. -Dylan 1

    [Header("Battle Settings")]

    private bool combatFinished = false;
    private bool playerDied = false;

    public Unit playerUnit;
    private Unit enemyUnit;

    private GameObject enemyGO;

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

        state = BattleState.START;

        bm = breakMeterGO.GetComponent<BreakMeter>(); //Fetches the script for the break meter. -Dylan 1
        eventManager = FindAnyObjectByType<QTEManager>();

        StartCoroutine(SetupBattle());
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
        //playerUnit = playerGO.GetComponent<Unit>(); // We are now reading the unit data from GameData when we load into the scene. - Dan

        enemyPrefab = enemyPrefabs[loadedEnemyID];

        enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<Unit>();

        battleText.text = "The battle has begun! " + enemyUnit.unitName + " wants to fight!";

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        //Curse check
        PlayerTurn();
    }

    IEnumerator EnemyTurn()
    {
        //Check what actions the enemy can use. Here we check enemy behavior.

        battleText.text = "The " + enemyUnit.unitName + " is deciding what to do";

        yield return new WaitForSeconds(2f);

        StartCoroutine(EnemyAttack()); // For now we will assume the enemy will always attack

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
        yield return new WaitForSeconds(2);

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
        
    }
    private void PlayerTurn()
    {
        RefreshPlayerStatus();

        battleText.text = "Your Turn!";
        turnCounter += 1;
    }

    

    #region Player Actions
    IEnumerator PlayerAttack() // Deals damage to enemy and then checks if it is dead
    {
        state = BattleState.QTE;

        battleText.text = "You attack! Press the right key";
        eventManager.GenerateQTE(3f); // Generates Quick time event

        yield return new WaitForSeconds(3.1f);
        int damageDealt = 0;
        switch (eventManager.eventResult)
        {
            case QTEResult.NONE:
                print("ERROR! EventResult should not be NONE");
                break;
            case QTEResult.LOW:
                damageDealt = playerUnit.lowDamage;
                battleText.text = "A weak hit! The " + enemyUnit.unitName + " takes " + damageDealt + " damage!";

                if (GameObject.Find("WeakAttackCurse") != null) //Changes the break charge based on the weak curse. -Dylan 2
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
                battleText.text = "A hit! The " + enemyUnit.unitName + " takes " + damageDealt + " damage!";
                bm.ChangeMeterValue(12);
                break;
            case QTEResult.HIGH:
                damageDealt = playerUnit.highDamage;
                battleText.text = "A strong hit! The " + enemyUnit.unitName + " takes " + damageDealt + " damage!";

                if (GameObject.Find("DefendingCurse") != null) //Changes break charge based on the strong attack curse. -Dylan 2
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

        bool isDead = enemyUnit.TakeDamage(damageDealt, false);
        enemyHUD.SetHP(enemyUnit);

        yield return new WaitForSeconds(2f);

        // Here begins endturn functionality
        if (isDead)
        {
            // (If more than 1 enemy) Check if all enemies are dead
            state = BattleState.WON;
            EndBattle();
            // End the battle
        }

        else 
        {
            // Proceed to enemy's turn.
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
        // Change state of battle based on result

    }

    IEnumerator PlayerDefend() 
    {
        playerUnit.isDefending = true;

        battleText.text = playerUnit.unitName + " takes a defensive stance!";

        yield return new WaitForSeconds(2f);

        // If we create any effects that would damage the enemy upon defending, we can copy the PlayerAttack endturn functionality

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());

    }

    IEnumerator PlayerBreak()
    {
        // Break animation plays. It is an auto success!

        breakButton.interactable = false;
        //Play the animation for the break button deactivating

        battleText.text = "You break the curse imposed by " + enemyUnit.unitName + " and deal " + playerUnit.highDamage + " damage!";

        cursesToTalismans += 1;

        yield return new WaitForSeconds(2f);

        // Damage the enemy selected. Choose damage based on eventState;
        bool isDead = enemyUnit.TakeDamage(playerUnit.highDamage, false);
        enemyHUD.SetHP(enemyUnit);

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

        // Here begins endturn functionality
        if (isDead)
        {
            // (If more than 1 enemy) Check if all enemies are dead
            state = BattleState.WON;
            EndBattle();
            // End the battle
        }

        else
        {
            // Proceed to enemy's turn.
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
                damageDealt = enemyUnit.lowDamage;
                break;

            case 2:
            case 3:
                damageDealt = enemyUnit.midDamage;
                break;

            case 4:
                damageDealt = enemyUnit.highDamage;
                break;

            default:
                print("ERROR! random.range outside possible bounds.");
                break;
        }
        print("Damage dealt before defense" + damageDealt);
        battleText.text = "The " + enemyUnit.unitName + " attacks! " + playerUnit.unitName + " takes " + damageDealt + " damage!";
        int playerDef = playerUnit.defense;
        if (playerUnit.isDefending) 
        {
            playerUnit.defense -= defenseBoost; // Reset any previous defense gains in case player is attacked more than once on same turn.
            state = BattleState.QTE;          
            battleText.text = "The " + enemyUnit.unitName + " attacks! Block by pressing the right key!";
            eventManager.GenerateQTE(3f);
            // Start QTE. Reduce incoming damage based on degree of success by increasing player defense. Align QTE with enemy's attack animation.
            yield return new WaitForSeconds(3f);
            
            switch (eventManager.eventResult)
            {
                case QTEResult.NONE:
                    print("ERROR! EventResult should not be NONE");
                    break;

                case QTEResult.LOW:
                    defenseBoost = 1;
                    battleText.text = "A weak block..." + playerUnit.unitName + " takes " + (Mathf.Clamp(damageDealt - defenseBoost , 1, damageDealt)) + " damage!";

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

        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }

        else
        {
            // (Check if there are more enemies)
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

        if (GameObject.Find("AttackingCurse") != null) //To prevent attacking. -Dylan 2
        {
            battleText.text = "The enemy's curse is preventing " + playerUnit.unitName + " from attacking!";
            attackButton.interactable = false;
            return;
        }

        StartCoroutine(PlayerAttack());
        state = BattleState.START;
    }

    public void OnDefendButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

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

        StartCoroutine(PlayerBreak());
        state = BattleState.START;
    }

    #endregion
}
