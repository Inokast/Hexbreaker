using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class TakeTalisman : MonoBehaviour
{
    [SerializeField] GameObject nextLevelButton;

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
                OverworldManager om = GameObject.Find("OverworldManager").GetComponent<OverworldManager>();
                Debug.Log("Overworld Data Loaded");
                om.playerUnit.currentHP += amount;

                if (om.playerUnit.currentHP > om.playerUnit.maxHP)
                {
                    om.playerUnit.currentHP = om.playerUnit.maxHP;
                }
            }
            else
            {
                CombatManager cm = GameObject.Find("CombatManager").GetComponent<CombatManager>();
                Debug.Log("Combat Data Loaded");
                cm.playerUnit.currentHP += amount;

                if (cm.playerUnit.currentHP > cm.playerUnit.maxHP)
                {
                    cm.playerUnit.currentHP = cm.playerUnit.maxHP;
                }
            }
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

            if (amount >= 1000)
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
                OverworldManager om = GameObject.Find("OverworldManager").GetComponent<OverworldManager>();

                om.playerUnit.maxHP -= secondHalf;

                om.playerUnit.currentHP += firstHalf;

                if (om.playerUnit.currentHP > om.playerUnit.maxHP)
                {
                    om.playerUnit.currentHP = om.playerUnit.maxHP;
                }
            }
            else
            {
                CombatManager cm = GameObject.Find("CombatManager").GetComponent<CombatManager>();

                cm.playerUnit.maxHP -= secondHalf;
                Debug.Log("Combat Data Loaded");
                cm.playerUnit.currentHP += firstHalf;

                if (cm.playerUnit.currentHP > cm.playerUnit.maxHP)
                {
                    cm.playerUnit.currentHP = cm.playerUnit.maxHP;
                }
            }
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
                OverworldManager om = GameObject.Find("OverworldManager").GetComponent<OverworldManager>();

                om.playerUnit.maxHP += amount;
            }
            else
            {
                CombatManager cm = GameObject.Find("CombatManager").GetComponent<CombatManager>();
                Debug.Log("Combat Data Loaded");
                cm.playerUnit.maxHP += amount;
            }
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
