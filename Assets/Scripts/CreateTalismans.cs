//06/05/23 Dylan

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreateTalismans : MonoBehaviour
{
    [SerializeField] GameObject commonFrame;
    [SerializeField] GameObject uncommonFrame;
    [SerializeField] GameObject rareFrame;
    [SerializeField] GameObject legendaryFrame;
    [SerializeField] GameObject spectralFrame;

    [SerializeField] GameObject talismanChoicePanel;

    public void GenerateEncounterTalismans()
    {
        int amount = CombatManager.cursesToTalismans;

        GameObject.Find("Purify Button").SetActive(false);

        do
        {
            GameObject talisman = DecideTalismanRarity(amount);

            int talismanType = Random.Range(0, 76); //Change this when more talisman types exist. -Dylan 3
            //Placeholder values. Just need the system to work at the moment -Dylan 3
            CreateHealthTalisman(talisman);

            if (talismanType <= 35)
            {
                CreateHealthTalisman(talisman);
            }
            else if (talismanType > 35 && talismanType <= 55)
            {
                CreateExtendingTalisman(talisman);
            }
            else if (talismanType > 55 && talismanType <= 75)
            {
                CreateMaxHealthTalisman(talisman);
            }
            else if (talismanType > 80)
            {
                CreateDefenseTalisman(talisman);
            }
            else
            {
                Debug.LogWarning("Talisman Type randomization out of range.");
            }

            amount--;

        } while (amount > 0);
    }

    private void CreateHealthTalisman(GameObject frame)
    {
        int rarity = FetchRarity(frame);

        TMP_Text[] texts = frame.GetComponentsInChildren<TMP_Text>();

        texts[2].text = "Instantly heals " + (5 * rarity) + " HP";

        texts[1].text = "Restorative Talisman";
    }

    private void CreateMaxHealthTalisman(GameObject frame)
    {
        int rarity = FetchRarity(frame);

        TMP_Text[] texts = frame.GetComponentsInChildren<TMP_Text>();

        texts[2].text = "Instantly heals " + (8 * rarity) + " HP, but decreases your maximum HP by " + (2 * rarity);

        texts[1].text = "Surgical Talisman";
    }

    private void CreateExtendingTalisman(GameObject frame)
    {
        int rarity = FetchRarity(frame);

        TMP_Text[] texts = frame.GetComponentsInChildren<TMP_Text>();

        texts[2].text = "Increases your maximum HP by " + (3 * rarity);

        texts[1].text = "Expansive Talisman";
    }

    private void CreateAttackTalisman(GameObject frame)
    {

    }

    private void CreateDefenseTalisman(GameObject frame)
    {

    }

    private GameObject DecideTalismanRarity(int position)
    {
        int rarity = Random.Range(0, 101);

        GameObject rarityFrame;

        float posX = 0f;

        float posY = 0f;

        if (position == 3)
        {
            posX = 325f;
            posY = 84f;
        }
        else if (position == 2)
        {
            posX = 0f;
            posY = 84f;
        }
        else if (position == 1)
        {
            posX = -325f;
            posY = 84f;
        }
        else
        {
            Debug.LogWarning("Out of rarity position range.");
        }

        if (rarity <= 35)
        {
            rarityFrame = Instantiate(commonFrame, new Vector3(posX, posY, 0f), Quaternion.identity);
            rarityFrame.transform.SetParent(GameObject.Find("Canvas").transform, false);
            return rarityFrame;
        }
        else if (rarity > 35 && rarity <= 65)
        {
            rarityFrame = Instantiate(uncommonFrame, new Vector3(posX, posY, 0f), Quaternion.identity);
            rarityFrame.transform.SetParent(GameObject.Find("Canvas").transform, false);
            return rarityFrame;
        }
        else if (rarity > 65 && rarity <= 85)
        {
            rarityFrame = Instantiate(rareFrame, new Vector3(posX, posY, 0f), Quaternion.identity);
            rarityFrame.transform.SetParent(GameObject.Find("Canvas").transform, false);
            return rarityFrame;
        }
        else if (rarity > 85 && rarity <= 95)
        {
            rarityFrame = Instantiate(legendaryFrame, new Vector3(posX, posY, 0f), Quaternion.identity);
            rarityFrame.transform.SetParent(GameObject.Find("Canvas").transform, false);
            return rarityFrame;
        }
        else if (rarity > 95 && rarity <= 100)
        {
            rarityFrame = Instantiate(spectralFrame, new Vector3(posX, posY, 0f), Quaternion.identity);
            rarityFrame.transform.SetParent(GameObject.Find("Canvas").transform, false);
            return rarityFrame;
        }
        else
        {
            Debug.LogWarning("Rarity messed up!");
            return null;
        }
    }

    private int FetchRarity(GameObject frame)
    {
        if (frame.CompareTag("SpectralFrame"))
        {
            return 5;
        }
        else if (frame.CompareTag("LegendaryFrame"))
        {
            return 4;
        }
        else if (frame.CompareTag("RareFrame"))
        {
            return 3;
        }
        else if (frame.CompareTag("UncommonFrame"))
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }

    public void GenerateLuckyTalismans()
    {
        int amount = 3;

        talismanChoicePanel.SetActive(true);

        do
        {
            GameObject talisman = DecideTalismanRarity(amount);

            int talismanType = Random.Range(0, 76); //Change this when more talisman types exist. -Dylan 3

            CreateHealthTalisman(talisman);

            if (talismanType <= 35)
            {
                CreateHealthTalisman(talisman);
            }
            else if (talismanType > 35 && talismanType <= 55)
            {
                CreateExtendingTalisman(talisman);
            }
            else if (talismanType > 55 && talismanType <= 75)
            {
                CreateMaxHealthTalisman(talisman);
            }
            else if (talismanType > 80)
            {
                CreateDefenseTalisman(talisman);
            }
            else
            {
                Debug.LogWarning("Talisman Type randomization out of range.");
            }

            amount--;

        } while (amount > 0);
    }

    public void GenerateHealingTalismans()
    {
        int amount = 3;

        talismanChoicePanel.SetActive(true);

        do
        {
            GameObject talisman = DecideTalismanRarity(amount);

            int talismanType = Random.Range(0, 101); //Change this when more talisman types exist. -Dylan 3

            CreateHealthTalisman(talisman);

            if (talismanType <= 50)
            {
                CreateHealthTalisman(talisman);
            }
            else if (talismanType > 50 && talismanType <= 75)
            {
                CreateExtendingTalisman(talisman);
            }
            else if (talismanType > 75 && talismanType <= 100)
            {
                CreateMaxHealthTalisman(talisman);
            }
            else
            {
                Debug.LogWarning("Talisman Type randomization out of range.");
            }

            amount--;

        } while (amount > 0);
    }

    public void HidePanel()
    {
        talismanChoicePanel.SetActive(false);

        GameObject[] commons = GameObject.FindGameObjectsWithTag("CommonFrame");

        GameObject[] uncommons = GameObject.FindGameObjectsWithTag("UncommonFrame");

        GameObject[] rares = GameObject.FindGameObjectsWithTag("RareFrame");

        GameObject[] legendaries = GameObject.FindGameObjectsWithTag("LegendaryFrame");

        GameObject[] spectrals = GameObject.FindGameObjectsWithTag("SpectralFrame");

        HideFrame(commons);
        HideFrame(uncommons);
        HideFrame(rares);
        HideFrame(legendaries);
        HideFrame(spectrals);
    }

    private void HideFrame(GameObject[] frames)
    {
        foreach (GameObject frame in frames)
        {
            Destroy(frame);
        }
    }
}
