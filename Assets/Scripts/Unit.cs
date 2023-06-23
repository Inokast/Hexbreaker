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

    public int defense; // A passive damage reduction. Reduces incoming damage by this value.

    public int maxHP;
    public int currentHP;

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

    }
}
