// Hexbreaker - used in Combat System
// Last modified: 05/25/23 - Dan Sanchez
// Notes:

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI healthText;
    public Slider hpSlider;

    public void SetHUD(Unit unit) 
    {
        if (nameText != null) 
        {
            nameText.text = unit.unitName;
        }
        
        healthText.text = unit.currentHP + " / " + unit.maxHP + " HP";
        hpSlider.maxValue = unit.maxHP;
        hpSlider.value = unit.currentHP;
    }

    public void SetHP(Unit unit) 
    {
        hpSlider.value = unit.currentHP;
        healthText.text = unit.currentHP + " / " + unit.maxHP + " HP";
    }
}
