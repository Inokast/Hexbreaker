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
        nameText.text = unit.unitName;
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
