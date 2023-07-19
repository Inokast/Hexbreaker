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

    private CreateTalismans talismanManager;

    private bool attachTalismanNow = false;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "Overworld")
        {
            om = GameObject.Find("OverworldManager").GetComponent<OverworldManager>();
        }
        else if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            cm = GameObject.Find("CombatManager").GetComponent<CombatManager>();
        }

        talismanManager = GameObject.Find("TalismanGenerator").GetComponent<CreateTalismans>();
    }

    private void Update()
    {
        if (!attachTalismanNow)
        {
            if (Input.GetMouseButtonDown(0))
            {
                attachTalismanNow = true;
            }
        }
    }

    public void GetTalisman()
    {
        if (GameObject.Find("CombatManager") != null)
        {
            if (GameObject.Find("CombatManager").GetComponent<CombatManager>().combatFinished)
            {
                MainTalismanAcquire();
            }
            else //This is where the action talismans are handled. -Dylan 7
            {
                TMP_Text[] texts = gameObject.GetComponentsInChildren<TMP_Text>();

                string name = texts[1].text;

                string description = texts[2].text;

                string extractedNumber = "";

                for (int i = 0; i < description.Length; i++)
                {
                    if (Char.IsDigit(description[i]))
                    {
                        extractedNumber += description[i];
                    }
                }

                int amount = int.Parse(extractedNumber);

                cm = GameObject.Find("CombatManager").GetComponent<CombatManager>();

                if (name == "Multistrike Talisman")
                {
                    if (cm.activeTalismanNames.Contains("Multistrike"))
                    {
                        return;
                    }

                    cm.activeTalismanNames.Add("Multistrike");

                    cm.activeTalismanPotency.Add(amount);
                }
                else if (name == "Vampiric Talisman")
                {
                    if (cm.activeTalismanNames.Contains("Vampiric"))
                    {
                        return;
                    }

                    cm.activeTalismanNames.Add("Vampiric");

                    cm.activeTalismanPotency.Add(amount);
                }
                else if (name == "Conflicting Talisman")
                {
                    if (cm.activeTalismanNames.Contains("Conflicting"))
                    {
                        return;
                    }

                    cm.activeTalismanNames.Add("Conflicting");

                    cm.activeTalismanPotency.Add(amount);
                }
                else if (name == "Purification Talisman")
                {
                    if (cm.activeTalismanNames.Contains("Purification"))
                    {
                        return;
                    }

                    cm.activeTalismanNames.Add("Purification");

                    cm.activeTalismanPotency.Add(amount);
                }
                else if (name == "Contending Talisman")
                {
                    if (cm.activeTalismanNames.Contains("Contending"))
                    {
                        return;
                    }

                    cm.activeTalismanNames.Add("Contending");

                    cm.activeTalismanPotency.Add(amount);
                }
                else if (name == "Omnipotent Talisman")
                {
                    if (cm.activeTalismanNames.Contains("Omnipotent"))
                    {
                        return;
                    }

                    cm.activeTalismanNames.Add("Omnipotent");

                    cm.activeTalismanPotency.Add(amount);
                }

                cm.OnSkipButton();
                gameObject.SetActive(false);

                cm.ChangeTalismanCharges(cm.activeTalismanNames.Count); //For talisman charges -Dylan 9
            }
        }
        else if (SceneManager.GetActiveScene().name == "Overworld")
        {
            MainTalismanAcquire();
        }
        else
        {
            Debug.Log("CM/OM Error -- TakeTalisman.cs");
        }
    }

    private void MainTalismanAcquire()
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

            talismanManager.action.Add(false);

            talismanManager.talismans.Add(gameObject);

            talismanManager.talismanNames.Add("Restorative Talisman");

            talismanManager.talismanFirstStats.Add(amount);

            talismanManager.talismanSecondStats.Add(0);

            talismanManager.talismanRarities.Add(amount / 5);
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

            talismanManager.action.Add(false);

            talismanManager.talismans.Add(gameObject);

            talismanManager.talismanNames.Add("Surgical Talisman");

            talismanManager.talismanFirstStats.Add(firstHalf);

            talismanManager.talismanSecondStats.Add(secondHalf);

            talismanManager.talismanRarities.Add(firstHalf / 8);
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

            talismanManager.action.Add(false);

            talismanManager.talismans.Add(gameObject);

            talismanManager.talismanNames.Add("Expansive Talisman");

            talismanManager.talismanFirstStats.Add(amount);

            talismanManager.talismanSecondStats.Add(0);

            talismanManager.talismanRarities.Add(amount / 3);
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

                talismanManager.talismanNames.Add("Training Talisman");

                talismanManager.talismanFirstStats.Add(amount);

                talismanManager.talismanSecondStats.Add(0);

                talismanManager.talismanRarities.Add(amount / 1);
            }
            else if (name == "Training Talisman")
            {
                cm.playerUnit.lowDamage += amount;

                talismanManager.talismanNames.Add("Training Talisman");

                talismanManager.talismanFirstStats.Add(amount);

                talismanManager.talismanSecondStats.Add(0);

                talismanManager.talismanRarities.Add(amount / 1);
            }
            else if (SceneManager.GetActiveScene().name == "Overworld" && name == "Powerful Talisman")
            {
                om.playerUnit.midDamage += amount;

                talismanManager.talismanNames.Add("Powerful Talisman");

                talismanManager.talismanFirstStats.Add(amount);

                talismanManager.talismanSecondStats.Add(0);

                talismanManager.talismanRarities.Add(amount / 2);
            }
            else if (name == "Powerful Talisman")
            {
                cm.playerUnit.midDamage += amount;

                talismanManager.talismanNames.Add("Powerful Talisman");

                talismanManager.talismanFirstStats.Add(amount);

                talismanManager.talismanSecondStats.Add(0);

                talismanManager.talismanRarities.Add(amount / 2);
            }
            else if (SceneManager.GetActiveScene().name == "Overworld" && name == "Perfect Talisman")
            {
                om.playerUnit.highDamage += amount;

                talismanManager.talismanNames.Add("Perfect Talisman");

                talismanManager.talismanFirstStats.Add(amount);

                talismanManager.talismanSecondStats.Add(0);

                talismanManager.talismanRarities.Add(amount / 3);
            }
            else if (name == "Perfect Talisman")
            {
                cm.playerUnit.highDamage += amount;

                talismanManager.talismanNames.Add("Perfect Talisman");

                talismanManager.talismanFirstStats.Add(amount);

                talismanManager.talismanSecondStats.Add(0);

                talismanManager.talismanRarities.Add(amount / 3);
            }

            talismanManager.action.Add(false);

            talismanManager.talismans.Add(gameObject);
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

            talismanManager.action.Add(false);

            talismanManager.talismans.Add(gameObject);

            talismanManager.talismanNames.Add("Impenetrable Talisman");

            talismanManager.talismanFirstStats.Add(amount);

            talismanManager.talismanSecondStats.Add(0);

            talismanManager.talismanRarities.Add(amount / 1);
        }
        else //Is action talisman -Dylan 10
        {
            talismanManager.action.Add(true);

            string extractedNumber = "";

            for (int i = 0; i < description.Length; i++)
            {
                if (Char.IsDigit(description[i]))
                {
                    extractedNumber += description[i];
                }
            }

            int amount = int.Parse(extractedNumber);

            if (tag == "SpectralFrame")
            {
                talismanManager.talismanRarities.Add(5);
            }
            else if (tag == "LegendaryFrame")
            {
                talismanManager.talismanRarities.Add(4);
            }
            else if (tag == "RareFrame")
            {
                talismanManager.talismanRarities.Add(3);
            }
            else if (tag == "UncommonFrame")
            {
                talismanManager.talismanRarities.Add(2);
            }
            else
            {
                talismanManager.talismanRarities.Add(1);
            }

            talismanManager.talismanNames.Add(name);

            talismanManager.talismanFirstStats.Add(amount);

            talismanManager.talismanSecondStats.Add(0);

            talismanManager.talismans.Add(gameObject);
        }

        gameObject.SetActive(false);

        GameObject[] buttons = GameObject.FindGameObjectsWithTag("TalismanButton");

        if (SceneManager.GetActiveScene().name != "Overworld")
        {
            GameObject instantiatedButton = Instantiate(nextLevelButton, new Vector3(968f, 100f, 0f), Quaternion.identity, GameObject.Find("Canvas").transform);
        }

        foreach (GameObject button in buttons)
        {
            button.SetActive(false);
        }

        gameObject.SetActive(true);

        OtherCardsVanish();
    }

    private void OtherCardsVanish() //Makes it possible for the talisman to stay on-screen by creating a copy of it. -Dylan 13
    {
        GameObject[] cTalis = GameObject.FindGameObjectsWithTag("CommonFrame");
        GameObject[] uTalis = GameObject.FindGameObjectsWithTag("UncommonFrame");
        GameObject[] rTalis = GameObject.FindGameObjectsWithTag("RareFrame");
        GameObject[] lTalis = GameObject.FindGameObjectsWithTag("LegendaryFrame");
        GameObject[] sTalis = GameObject.FindGameObjectsWithTag("SpectralFrame");

        if (cTalis != null)
        {
            foreach (GameObject t in cTalis)
            {
                t.SetActive(false);
            }
        }

        if (uTalis != null)
        {
            foreach (GameObject t in uTalis)
            {
                t.SetActive(false);
            }
        }

        if (rTalis != null)
        {
            foreach (GameObject t in rTalis)
            {
                t.SetActive(false);
            }
        }

        if (lTalis != null)
        {
            foreach (GameObject t in lTalis)
            {
                t.SetActive(false);
            }
        }

        if (sTalis != null)
        {
            foreach (GameObject t in sTalis)
            {
                t.SetActive(false);
            }
        }

        gameObject.SetActive(true);

        GameObject talisman = Instantiate(gameObject, gameObject.transform.position, Quaternion.identity);

        talisman.transform.SetParent(GameObject.Find("Canvas").transform, true);

        talisman.transform.localScale = new Vector3(1f, 1f, 1f);

        gameObject.transform.SetParent(GameObject.Find("TalismanGenerator").transform, false);

        gameObject.SetActive(false);

        GameObject[] buttons2 = GameObject.FindGameObjectsWithTag("TalismanButton");

        foreach (GameObject btn in buttons2)
        {
            btn.SetActive(false);
        }
        /*yield return new WaitUntil(() => attachTalismanNow == true);

        gameObject.transform.SetParent(GameObject.Find("TalismanGenerator").transform, false);
        gameObject.SetActive(false);
        attachTalismanNow = false;*/
    }
}
