// Hexbreaker - used in Combat System
// Last modified: 05/25/23 - Dan Sanchez
// Notes:

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Unit Information")]
    public string unitName;
    public bool isDefending = false;

    [Header("Unit Stats")]
    public int highDamage;
    public int midDamage; // The next Attack will deal this amount of damage. Can be updated through unit's actions.
    public int lowDamage;
    public int strengthModifier = 3;
    public int drainStrength;
    public bool isCharged = false;
    public bool isStunned = false;

    public string attackType1; // standard, mash, timed, or array
    public int keyToPress1; // Keep values between 1, 2, 3 or 4;
    public float attackTimer1;
    public string attackString1;

    public string attackType2;
    public int keyToPress2; // Keep values between 1, 2, 3 or 4;
    public float attackTimer2;
    public string attackString2;

    public float fillGauge1;
    public float fillGauge2;
    public int[] keyToPressArray; // Keep values between 1, 2, 3 or 4;

    public int defense; // A passive damage reduction. Reduces incoming damage by this value.

    public int maxHP;
    public int currentHP;

    public Animator basicAnim;
    public Animator overworldPlayer;

    public bool TakeDamage(int dmg, bool ignoreDefense) // Takes damage and returns true if unit has died
    {
        if (ignoreDefense)
        {
            currentHP -= dmg;
        }

        else
        {
            currentHP -= Mathf.Clamp(dmg - defense, 1, dmg); // Mathf.Clamp ensures damage cannot be reduced lower than 1 regardless of defense.
        }


        if (currentHP <= 0)
        {
            currentHP = 0;
            return true;
        }

        else
        {
            return false;
        }
    }

    public void heal(int amount) // Restores health
    {
        currentHP += Mathf.Clamp(amount, 1, maxHP); // Ensures amount healed does not exceed the maxHP value
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }

    }

    public void PlayHitAnim()
    {
        basicAnim.SetTrigger("Hit");
    }

    public void PlayAttackAnim()
    {
        basicAnim.SetTrigger("ATK");
    }

    public void PlayDeathAnim()
    {
        basicAnim.SetTrigger("Death");
    }

    public void PlayChargedAnim()
    {
        basicAnim.SetTrigger("Charged");
    }

    //////////////////////////////////////////////////
    // Unique player animation for Focus state
    public void PlayFocusEnterAnim() 
    {
        basicAnim.SetTrigger("FocusEnter");
    }

    public void PlayFocusAttackAnim()
    {
        basicAnim.SetTrigger("FocusATK");
    }

    public void PlayFocusHitAnim()
    {
        basicAnim.SetTrigger("FocusHit");
    }

    // secondary animation for the overworld player character to make it translate higher than what's animated
    public void PlayJumpAnim()
    {
        overworldPlayer.SetTrigger("Jump");
        basicAnim.SetTrigger("Jump");
    }
    public void PlayFallAnim()
    {
        overworldPlayer.SetTrigger("Fall");
        basicAnim.SetTrigger("Fall");
    }
}
