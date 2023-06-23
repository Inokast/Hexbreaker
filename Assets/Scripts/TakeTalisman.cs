using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class TakeTalisman : MonoBehaviour
{
    [SerializeField] GameObject nextLevelButton;

    private OverworldManager om;

    private CombatManager cm;

    private Unit playerUnit;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Overworld")
        {
            om = GameObject.Find("OverworldManager").GetComponent<OverworldManager>();
        }
        else
        {
            cm = GameObject.Find("CombatManager").GetComponent<CombatManager>();
        }

        playerUnit = GameObject.Find("Player").GetComponent<Unit>();
    }

    public void GetTalisman()
    {
        string tag = gameObject.tag;

        TMP_Text[] texts = gameObject.GetComponentsInChildren<TMP_Text>();

        string name = texts[1].text;

        string description = texts[2].text;

        if (name == "Restorative Talisman")
        {
            string extractedNumber = "";

            for (int i = 0; i < description.Length; i++)
            {
                if (Char.IsDigit(description[i]))
                {
                    extractedNumber += description[i];
                }
            }

            int amount = int.Parse(extractedNumber);

            if (SceneManager.GetActiveScene().name == "Overworld")
            {
                om.playerUnit.currentHP += amount;

                if (om.playerUnit.currentHP > om.playerUnit.maxHP)
                {
                    om.playerUnit.currentHP = om.playerUnit.maxHP;
                }
            }
            else
            {
                cm.playerUnit.currentHP += amount;

                if (cm.playerUnit.currentHP > cm.playerUnit.maxHP)
                {
                    cm.playerUnit.currentHP = cm.playerUnit.maxHP;
                }
            }

            playerUnit.talismans.Add(this.transform.parent.gameObject);

            playerUnit.action.Add(false);
        }
        else if (name == "Surgical Talisman")
        {
            string extractedNumber = "";

            for (int i = 0; i < description.Length; i++)
            {
                if (Char.IsDigit(description[i]))
                {
                    extractedNumber += description[i];
                }
            }

            int amount = int.Parse(extractedNumber);

            int firstHalf = 0;

            int secondHalf = 0;

            if (amount > 100)
            {
                firstHalf = Convert.ToInt32(amount.ToString().Substring(0, 2));

                secondHalf = Convert.ToInt32(amount.ToString().Substring(2));
            }
            else if (amount <= 100)
            {
                firstHalf = Convert.ToInt32(amount.ToString().Substring(0, 1));

                secondHalf = Convert.ToInt32(amount.ToString().Substring(1));
            }

            if (SceneManager.GetActiveScene().name == "Overworld")
            { 
                om.playerUnit.maxHP -= secondHalf;

                om.playerUnit.currentHP += firstHalf;

                if (om.playerUnit.currentHP > om.playerUnit.maxHP)
                {
                    om.playerUnit.currentHP = om.playerUnit.maxHP;
                }
            }
            else
            {
                cm.playerUnit.maxHP -= secondHalf;

                cm.playerUnit.currentHP += firstHalf;

                if (cm.playerUnit.currentHP > cm.playerUnit.maxHP)
                {
                    cm.playerUnit.currentHP = cm.playerUnit.maxHP;
                }
            }

            playerUnit.talismans.Add(this.transform.parent.gameObject);

            playerUnit.action.Add(false);
        }
        else if (name == "Expansive Talisman")
        {
            string extractedNumber = "";

            for (int i = 0; i < description.Length; i++)
            {
                if (Char.IsDigit(description[i]))
                {
                    extractedNumber += description[i];
                }
            }

            int amount = int.Parse(extractedNumber);
            
            if (SceneManager.GetActiveScene().name == "Overworld")
            {
                om.playerUnit.maxHP += amount;
            }
            else
            {
                cm.playerUnit.maxHP += amount;
            }

            playerUnit.talismans.Add(this.transform.parent.gameObject);

            playerUnit.action.Add(false);
        }
        else if (name == "Training Talisman" || name == "Powerful Talisman" || name == "Perfect Talisman")
        {
            string extractedNumber = "";

            for (int i = 0; i < description.Length; i++)
            {
                if (Char.IsDigit(description[i]))
                {
                    extractedNumber += description[i];
                }
            }

            int amount = int.Parse(extractedNumber);

            if (SceneManager.GetActiveScene().name == "Overworld" && name == "Training Talisman")
            {
                om.playerUnit.lowDamage += amount;
            }
            else if (name == "Training Talisman")
            {
                cm.playerUnit.lowDamage += amount;
            }
            else if (SceneManager.GetActiveScene().name == "Overworld" && name == "Powerful Talisman")
            {
                om.playerUnit.midDamage += amount;
            }
            else if (name == "Powerful Talisman")
            {
                cm.playerUnit.midDamage += amount;
            }
            else if (SceneManager.GetActiveScene().name == "Overworld" && name == "Perfect Talisman")
            {
                om.playerUnit.highDamage += amount;
            }
            else if (name == "Perfect Talisman")
            {
                cm.playerUnit.highDamage += amount;
            }

            playerUnit.talismans.Add(this.transform.parent.gameObject);

            playerUnit.action.Add(false);
        }
        else if (name == "Impenetrable Talisman")
        {
            string extractedNumber = "";

            for (int i = 0; i < description.Length; i++)
            {
                if (Char.IsDigit(description[i]))
                {
                    extractedNumber += description[i];
                }
            }

            int amount = int.Parse(extractedNumber);

            if (SceneManager.GetActiveScene().name == "Overworld")
            {
                om.playerUnit.defense += amount;
            }
            else
            {
                cm.playerUnit.defense += amount;
            }

            playerUnit.talismans.Add(this.transform.parent.gameObject);

            playerUnit.action.Add(false);
        }

        GameObject[] buttons = GameObject.FindGameObjectsWithTag("TalismanButton");

        if (SceneManager.GetActiveScene().name != "Overworld")
        {
            GameObject instantiatedButton = Instantiate(nextLevelButton, new Vector3(968f, 100f, 0f), Quaternion.identity, GameObject.Find("Canvas").transform);
        }

        foreach (GameObject button in buttons)
        {
            button.SetActive(false);
        }
    }
}
