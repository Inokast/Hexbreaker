using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

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

            if (amount >= 1000)
            {
                int firstHalf = Convert.ToInt32(amount.ToString().Substring(0, 2));

                int secondHalf = Convert.ToInt32(amount.ToString().Substring(2));
            }
            else if (amount <= 100)
            {
                int firstHalf = Convert.ToInt32(amount.ToString().Substring(0, 1));

                int secondHalf = Convert.ToInt32(amount.ToString().Substring(1));
            }

            //Combine with hp and max hp respectively.
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
            //Add amount to max hp.
        }

        GameObject[] buttons = GameObject.FindGameObjectsWithTag("TalismanButton");

        GameObject instantiatedButton = Instantiate(nextLevelButton, new Vector3(500f, 0f, 0f), Quaternion.identity, GameObject.Find("Canvas").transform);

        foreach (GameObject button in buttons)
        {
            button.SetActive(false);
        }
    }
}
