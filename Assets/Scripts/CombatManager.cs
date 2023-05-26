// Hexbreaker - Combat System
// Last modified: 05/25/23 - Dan Sanchez
// Notes: Everything is public temporarily. Will change to private as needed. Also, we set BattleState to START after player has chosen their action to prevent them from triggering their action twice.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum BattleState { START, PLAYERTURN, QTE, ENEMYTURN, WON, LOST }
public enum QTEResult {NONE, LOW, MID, HIGH }
public class CombatManager : MonoBehaviour
{
    [Header("Scene Setup")] 

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public TextMeshProUGUI battleText;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    [Header("Battle Settings")]

    private Unit playerUnit;
    private Unit enemyUnit;

    private int defenseBoost = 0; // Used to temporarily increase player defense.
    private int turnCounter = 0; // Because some abilities will be gate by skill cooldowns, we'll use a counter. Turn counter always goes up on Player's turn.

    public BattleState state;
    public QTEResult eventResult;

    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle() // This function sets up the battle
    {
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
            battleText.text = "You won the battle! Choose a talisman to purify";
            /// Display choice
        }
        else if (state == BattleState.LOST)
        {
            battleText.text = "You were defeated. Good luck next time";
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
        eventResult = QTEResult.NONE;
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
        // Trigger QTE after a brief delay
        battleText.text = "QTE goes here. Player is Attacking";       

        yield return new WaitForSeconds(2f);

        //set QTE result
        state = BattleState.START;

        // Damage the enemy selected. Choose damage based on eventState;
        bool isDead = enemyUnit.TakeDamage(playerUnit.damage, false);
        enemyHUD.SetHP(enemyUnit);

        battleText.text = "The " + enemyUnit.unitName + " takes " + playerUnit.damage + " damage!";
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
    #endregion

    #region Enemy Actions
    IEnumerator EnemyAttack() 
    {

        if (playerUnit.isDefending) 
        {
            playerUnit.defense -= defenseBoost; // Reset any previous defense gains in case player is attacked more than once on same turn.
            state = BattleState.QTE;
            battleText.text = "QTE goes here";
            // Start QTE. Reduce incoming damage based on degree of success by increasing player defense. Align QTE with enemy's attack animation.
            yield return new WaitForSeconds(2f);

            

            eventResult = QTEResult.MID; // For now we assume it is always mid

            switch (eventResult)
            {
                case QTEResult.NONE:
                    print("ERROR! EventResult should not be NONE");
                    break;
                case QTEResult.LOW:
                    defenseBoost = 1;
                    break;
                case QTEResult.MID:
                    defenseBoost = 2;
                    break;
                case QTEResult.HIGH:
                    defenseBoost = 3;
                    break;
                default:
                    print("ERROR! EventResult is not recognized!");
                    break;
            }

            // Depending on degree of success, temporarily increase defense.
            playerUnit.defense += defenseBoost;           
        }

        bool isDead = playerUnit.TakeDamage(enemyUnit.damage, false);

        battleText.text = playerUnit.unitName + " takes " + (Mathf.Clamp(enemyUnit.damage - playerUnit.defense, 1, enemyUnit.damage)) + " damage!";

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

        //StartCoroutine(PlayerBreak());
        state = BattleState.START;
    }

    #endregion
}
