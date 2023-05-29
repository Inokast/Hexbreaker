// Hexbreaker - Combat System
// Last modified: 05/28/23 - Dan Sanchez
// Notes: Should be working as expected;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum BattleState { START, PLAYERTURN, QTE, ENEMYTURN, WON, LOST }
public class CombatManager : MonoBehaviour
{
    [Header("Scene Setup")] 

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    [SerializeField] private TextMeshProUGUI battleText;
    [SerializeField] private GameObject endPanel;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    [SerializeField] private BattleHUD playerHUD;
    [SerializeField] private BattleHUD enemyHUD;

    [SerializeField] private Button breakButton;

    private QTEManager eventManager;

    public GameObject breakMeterGO; //Since all of these are public, I figured I'd make mine public here too. -Dylan

    private BreakMeter bm; //This is to reference the script attached instead of fetching the component 20 times. -Dylan

    [Header("Battle Settings")]

    private Unit playerUnit;
    private Unit enemyUnit;

    private int defenseBoost = 0; // Used to temporarily increase player defense.
    private int turnCounter = 0; // Because some abilities will be gated by skill cooldowns, we'll use a counter. Turn counter always goes up on Player's turn.

    public BattleState state;

    // Start is called before the first frame update
    void Start()
    {       
        state = BattleState.START;

        bm = breakMeterGO.GetComponent<BreakMeter>(); //Fetches the script for the break meter. -Dylan
        eventManager = FindAnyObjectByType<QTEManager>();

        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle() // This function sets up the battle
    {
        endPanel.SetActive(false);
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGO.GetComponent<Unit>();

        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<Unit>();

        battleText.text = "The battle has begun! " + enemyUnit.unitName + " wants to fight!";

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
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
            battleText.text = "You won the battle! Choose a talisman to purify (Placeholder)";
            /// Display choice
            endPanel.SetActive(true);
        }
        else if (state == BattleState.LOST)
        {
            battleText.text = "You were defeated. Better luck next time";
            endPanel.SetActive(true);
        }

        else { battleText.text = "Battle state is wrong"; }
    }

    private void RefreshPlayerStatus()
    {
        if (playerUnit.isDefending)
        {
            playerUnit.defense -= defenseBoost;
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
        
    }
    private void PlayerTurn()
    {
        RefreshPlayerStatus();

        battleText.text = "Your Turn!";
        turnCounter += 1;
    }

    IEnumerator PlayerBreak() 
    {
        // Break animation plays. It is an auto success!

        breakButton.interactable = false;
        //Play the animation for the break button deactivating

        battleText.text = "You break the curse imposed by " + enemyUnit.unitName + " and deal " + playerUnit.highDamage + " damage!";

        yield return new WaitForSeconds(2f);

        // Damage the enemy selected. Choose damage based on eventState;
        bool isDead = enemyUnit.TakeDamage(playerUnit.highDamage, false);
        enemyHUD.SetHP(enemyUnit);

        bm.BreakCurse(GameObject.Find("ExampleCurse")); //Just placeholder, no coroutine needed yet since there's no curses. -Dylan

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

    #region Player Actions
    IEnumerator PlayerAttack() // Deals damage to enemy and then checks if it is dead
    {
        state = BattleState.QTE;
        // Trigger QTE after a brief delay
        battleText.text = "You attack! Press the right key";
        eventManager.GenerateQTE(3f); // Generates Quick time event;

        yield return new WaitForSeconds(3f);
        int damageDealt = 0;
        switch (eventManager.eventResult)
        {
            case QTEResult.NONE:
                print("ERROR! EventResult should not be NONE");
                break;
            case QTEResult.LOW:
                damageDealt = playerUnit.lowDamage;
                battleText.text = "A weak hit! The " + enemyUnit.unitName + " takes " + damageDealt + " damage!";
                break;
            case QTEResult.MID:
                damageDealt = playerUnit.midDamage;
                battleText.text = "A hit! The " + enemyUnit.unitName + " takes " + damageDealt + " damage!";
                break;
            case QTEResult.HIGH:
                damageDealt = playerUnit.highDamage;
                battleText.text = "A strong hit! The " + enemyUnit.unitName + " takes " + damageDealt + " damage!";
                break;
            default:
                print("ERROR! EventResult is not recognized!");
                break;
        }

        bool isDead = enemyUnit.TakeDamage(damageDealt, false);
        enemyHUD.SetHP(enemyUnit);

        //Break Meter charge is inserted here. -Dylan
        bm.ChangeMeterValue(18); //Placeholder value. Need to see how things play out. -Dylan
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

        bm.ChangeMeterValue(35); //Again, placeholder value. But I do think defend should give more than attack. Otherwise "attack OP pls nerf." -Dylan

        yield return new WaitForSeconds(2f);

        // If we create any effects that would damage the enemy upon defending, we can copy the PlayerAttack endturn functionality

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());

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

        battleText.text = "The " + enemyUnit.unitName + " attacks! " + playerUnit.unitName + " takes " + damageDealt + " damage!";

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
                    battleText.text = "A weak block..." + playerUnit.unitName + " takes " + (Mathf.Clamp(damageDealt - (playerUnit.defense += defenseBoost) , 1, damageDealt)) + " damage!";
                    break;

                case QTEResult.MID:
                    defenseBoost = 2;
                    battleText.text = "You block! " + playerUnit.unitName + " takes " + (Mathf.Clamp(damageDealt - (playerUnit.defense += defenseBoost), 1, damageDealt)) + " damage!";
                    break;

                case QTEResult.HIGH:
                    defenseBoost = 3;
                    battleText.text = "A perfect block! " + playerUnit.unitName + " takes " + (Mathf.Clamp(damageDealt - (playerUnit.defense += defenseBoost), 1, damageDealt)) + " damage!";
                    break;

                default:
                    print("ERROR! EventResult is not recognized!");
                    break;
            }
            // Depending on degree of success, temporarily increase defense.
            playerUnit.defense += defenseBoost;      
            
        }

        bool isDead = playerUnit.TakeDamage(damageDealt, false);
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

        StartCoroutine(PlayerAttack());
        state = BattleState.START;
    }

    public void OnDefendButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

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
