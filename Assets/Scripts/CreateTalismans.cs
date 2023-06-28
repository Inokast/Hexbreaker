//06/05/23 Dylan

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CreateTalismans : MonoBehaviour, IDataPersistence 
{
    [SerializeField] GameObject commonFrame;
    [SerializeField] GameObject uncommonFrame;
    [SerializeField] GameObject rareFrame;
    [SerializeField] GameObject legendaryFrame;
    [SerializeField] GameObject spectralFrame;

    private GameObject talismanChoicePanel;

    public List<GameObject> talismans = new List<GameObject>();
    public List<bool> action = new List<bool>();

    public static CreateTalismans talismanManager { get; private set; }

    void Awake()
    {
        if (talismanManager == null)
        {
            DontDestroyOnLoad(gameObject);
            talismanManager = this;
        }
        else if (talismanManager != null)
        {
            Destroy(gameObject);
        }
    }

    public void LoadData(GameData gameData)
    {
        talismans = gameData.talismansCollected;
        action = gameData.isAction;

        if (SceneManager.GetActiveScene().name == "Overworld")
        {
            talismanChoicePanel = GameObject.Find("TalismanPanel");

            talismanChoicePanel.SetActive(false);
        }
    }

    public void SaveData(GameData gameData)
    {
        gameData.talismansCollected = talismans;
        gameData.isAction = action;
    }


    public void GenerateEncounterTalismans()
    {
        int amount = CombatManager.cursesToTalismans;

        GameObject.Find("Purify Button").SetActive(false);

        if (amount == 0)
        {
            CreateWholeTalisman(1, false);
        }

        do
        {
            CreateWholeTalisman(amount, false);

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
        int rarity = FetchRarity(frame);

        TMP_Text[] texts = frame.GetComponentsInChildren<TMP_Text>();

        int random = Random.Range(0, 101);

        if (random <= 33)
        {
            texts[2].text = "Increases your weak attack's damage by " + (1 * rarity);

            texts[1].text = "Training Talisman";
        }
        else if (random > 33 && random <= 66)
        {
            texts[2].text = "Increases your medium attack's damage by " + (2 * rarity);

            texts[1].text = "Powerful Talisman";
        }
        else if (random > 66)
        {
            texts[2].text = "Increases your perfect attack's damage by " + (3 * rarity);

            texts[1].text = "Perfect Talisman";
        }
        else
        {
            Debug.Log("Missing range exception for attack talisman.");
        }
    }

    private void CreateDefenseTalisman(GameObject frame)
    {
        int rarity = FetchRarity(frame);

        TMP_Text[] texts = frame.GetComponentsInChildren<TMP_Text>();

        texts[2].text = "Increases your base defense by " + (1 * rarity);

        texts[1].text = "Impenetrable Talisman";
    }

    private void CreateActionTalisman(GameObject frame)
    {
        int rarity = FetchRarity(frame);

        TMP_Text[] texts = frame.GetComponentsInChildren<TMP_Text>();

        int rando = Random.Range(1, 101);

        if (rando <= 20)
        {
            texts[2].text = "Action Talisman: Your next attack will hit all enemies on the field and deal an additional " + (rarity * 2) + " damage to them";

            texts[1].text = "Multistrike Talisman";
        }
        else if (rando > 20 && rando <= 40)
        {
            texts[2].text = "Action Talisman: Your next attack will heal you for " + (rarity * 3) + " and deal the usual attack damage";

            texts[1].text = "Vampiric Talisman";
        }
        else if (rando > 40 && rando <= 60)
        {
            texts[2].text = "Action Talisman: Your next weak attack will deal the damage of a medium hit, plus an additional " + (rarity * 1);

            texts[1].text = "Conflicting Talisman";
        }
        else if (rando > 60 && rando <= 80)
        {
            texts[2].text = "Action Talisman: Your next attack will grant " + (rarity * 15) + " break meter charge";

            texts[1].text = "Purification Talisman";
        }
        else if (rando > 80 && rando <= 92)
        {
            texts[2].text = "Action Talisman: Your next attack will deal the damage of a perfect hit, plus an additional " + (rarity * 2);

            texts[1].text = "Contending Talisman";
        }
        else if (rando > 92)
        {
            texts[2].text = "Action Talisman: Your next perfect attack will deal an additional " + (rarity * 4) + " damage";

            texts[1].text = "Omnipotent Talisman";
        }
    }

    private GameObject DecideTalismanRarity(int position, bool isLucky)
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

        if (!isLucky)
        {
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
        else
        {
            if (rarity <= 25)
            {
                rarityFrame = Instantiate(commonFrame, new Vector3(posX, posY, 0f), Quaternion.identity);
                rarityFrame.transform.SetParent(GameObject.Find("Canvas").transform, false);
                return rarityFrame;
            }
            else if (rarity > 25 && rarity <= 50)
            {
                rarityFrame = Instantiate(uncommonFrame, new Vector3(posX, posY, 0f), Quaternion.identity);
                rarityFrame.transform.SetParent(GameObject.Find("Canvas").transform, false);
                return rarityFrame;
            }
            else if (rarity > 50 && rarity <= 75)
            {
                rarityFrame = Instantiate(rareFrame, new Vector3(posX, posY, 0f), Quaternion.identity);
                rarityFrame.transform.SetParent(GameObject.Find("Canvas").transform, false);
                return rarityFrame;
            }
            else if (rarity > 75 && rarity <= 90)
            {
                rarityFrame = Instantiate(legendaryFrame, new Vector3(posX, posY, 0f), Quaternion.identity);
                rarityFrame.transform.SetParent(GameObject.Find("Canvas").transform, false);
                return rarityFrame;
            }
            else if (rarity > 90 && rarity <= 100)
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
            CreateWholeTalisman(amount, true);

            amount--;

        } while (amount > 0);
    }

    public void GenerateHealingTalismans()
    {
        int amount = 3;

        talismanChoicePanel.SetActive(true);

        do
        {
            GameObject talisman = DecideTalismanRarity(amount, false);

            int talismanType = Random.Range(0, 101);

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

    private void CreateWholeTalisman(int amount, bool isLucky) //This function just exists so I don't have to copy/paste the main code into the next iteration. -Dylan 6
    {
        GameObject talisman = DecideTalismanRarity(amount, isLucky);

        int talismanType = 0;

        if (!isLucky)
        {
            talismanType = Random.Range(0, 101);
        }
        else
        {
            talismanType = Random.Range(52, 101);
        }



        if (talismanType <= 17)
        {
            CreateHealthTalisman(talisman);
        }
        else if (talismanType > 17 && talismanType <= 34)
        {
            CreateExtendingTalisman(talisman);
        }
        else if (talismanType > 34 && talismanType <= 51)
        {
            CreateMaxHealthTalisman(talisman);
        }
        else if (talismanType > 51 && talismanType <= 68)
        {
            CreateAttackTalisman(talisman);
        }
        else if (talismanType > 68 && talismanType <= 85)
        {
            CreateDefenseTalisman(talisman);
        }
        else if (talismanType > 85)
        {
            CreateActionTalisman(talisman);
        }
        else
        {
            Debug.LogWarning("Talisman Type randomization out of range.");
        }
    }
}
