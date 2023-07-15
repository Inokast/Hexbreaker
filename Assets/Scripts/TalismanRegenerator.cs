using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TalismanRegenerator : MonoBehaviour
{
    //public List<GameObject> talis;
    public List<bool> act;
    public List<string> talismanName;
    public List<int> talismanFirstStat;
    public List<int> talismanSecondStat;
    public List<int> talismanRarity;

    [SerializeField] GameObject commonFrame;
    [SerializeField] GameObject uncommonFrame;
    [SerializeField] GameObject rareFrame;
    [SerializeField] GameObject legendaryFrame;
    [SerializeField] GameObject spectralFrame;


    public void RegenerateTalismans()
    {
        GameObject tg = GameObject.Find("TalismanGenerator");

        CreateTalismans tgScript = tg.GetComponent<CreateTalismans>();

        act = tgScript.action;

        talismanName = tgScript.talismanNames;

        talismanFirstStat = tgScript.talismanFirstStats;

        talismanSecondStat = tgScript.talismanSecondStats;

        talismanRarity = tgScript.talismanRarities;

        foreach (GameObject t in tgScript.talismans)
        {
            Destroy(t);
        }

        tgScript.talismans.Clear();

        tgScript.action = new List<bool>();

        tgScript.talismanNames = new List<string>();

        tgScript.talismanFirstStats = new List<int>();

        tgScript.talismanSecondStats = new List<int>();

        tgScript.talismanRarities = new List<int>();

        for (int i = 0; i < act.Count; i++)
        {
            GameObject talisman = null;

            if (act[i])
            {
                tgScript.action.Add(true);
            }
            else
            {
                tgScript.action.Add(false);
            }

            tgScript.talismanRarities.Add(talismanRarity[i]);

            tgScript.talismanFirstStats.Add(talismanFirstStat[i]);

            tgScript.talismanSecondStats.Add(talismanSecondStat[i]);

            tgScript.talismanNames.Add(talismanName[i]);

            if (talismanRarity[i] == 1)
            {
                talisman = Instantiate(commonFrame, new Vector3(0, 0, 0), Quaternion.identity);
            }
            else if (talismanRarity[i] == 2)
            {
                talisman = Instantiate(uncommonFrame, new Vector3(0, 0, 0), Quaternion.identity);
            }
            else if (talismanRarity[i] == 3)
            {
                talisman = Instantiate(rareFrame, new Vector3(0, 0, 0), Quaternion.identity);
            }
            else if (talismanRarity[i] == 4)
            {
                talisman = Instantiate(legendaryFrame, new Vector3(0, 0, 0), Quaternion.identity);
            }
            else if (talismanRarity[i] == 5)
            {
                talisman = Instantiate(spectralFrame, new Vector3(0, 0, 0), Quaternion.identity);
            }

            TMP_Text[] texts = talisman.GetComponentsInChildren<TMP_Text>();

            if (talismanName[i] == "Restorative Talisman")
            {
                texts[1].text = "Restorative Talisman";

                texts[2].text = "Instantly heals " + talismanFirstStat[i] + " HP";
            }
            else if (talismanName[i] == "Surgical Talisman")
            {
                texts[2].text = "Instantly heals " + talismanFirstStat[i] + " HP, but decreases your maximum HP by " + talismanSecondStat[i];

                texts[1].text = "Surgical Talisman";
            }
            else if (talismanName[i] == "Expansive Talisman")
            {
                texts[2].text = "Increases your maximum HP by " + talismanFirstStat[i];

                texts[1].text = "Expansive Talisman";
            }
            else if (talismanName[i] == "Training Talisman")
            {
                texts[2].text = "Increases your weak attack's damage by " + talismanFirstStat[i];

                texts[1].text = "Training Talisman";
            }
            else if (talismanName[i] == "Powerful Talisman")
            {
                texts[2].text = "Increases your medium attack's damage by " + talismanFirstStat[i];

                texts[1].text = "Powerful Talisman";
            }
            else if (talismanName[i] == "Perfect Talisman")
            {
                texts[2].text = "Increases your perfect attack's damage by " + talismanFirstStat[i];

                texts[1].text = "Perfect Talisman";
            }
            else if (talismanName[i] == "Impenetrable Talisman")
            {
                texts[2].text = "Increases your base defense by " + talismanFirstStat[i];

                texts[1].text = "Impenetrable Talisman";
            }
            else if (talismanName[i] == "Multistrike Talisman")
            {
                texts[2].text = "Action Talisman: Your next attack will hit all enemies on the field and deal an additional " + talismanFirstStat[i] + " damage to them";

                texts[1].text = "Multistrike Talisman";
            }
            else if (talismanName[i] == "Vampiric Talisman")
            {
                texts[2].text = "Action Talisman: Your next attack will heal you for " + talismanFirstStat[i] + " and deal the usual attack damage";

                texts[1].text = "Vampiric Talisman";
            }
            else if (talismanName[i] == "Conflicting Talisman")
            {
                texts[2].text = "Action Talisman: Your next weak attack will deal the damage of a medium hit, plus an additional " + talismanFirstStat[i];

                texts[1].text = "Conflicting Talisman";
            }
            else if (talismanName[i] == "Purification Talisman")
            {
                texts[2].text = "Action Talisman: Your next attack will grant " + talismanFirstStat[i] + " break meter charge";

                texts[1].text = "Purification Talisman";
            }
            else if (talismanName[i] == "Contending Talisman")
            {
                texts[2].text = "Action Talisman: Your next attack will deal the damage of a perfect hit, plus an additional " + talismanFirstStat[i];

                texts[1].text = "Contending Talisman";
            }
            else if (talismanName[i] == "Omnipotent Talisman")
            {
                texts[2].text = "Action Talisman: Your next perfect attack will deal an additional " + talismanFirstStat[i] + " damage";

                texts[1].text = "Omnipotent Talisman";
            }

            tgScript.talismans.Add(talisman);

            talisman.transform.SetParent(tg.transform, false);

            talisman.SetActive(false);
        }
    }
}
