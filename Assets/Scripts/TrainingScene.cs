using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrainingScene : MonoBehaviour
{
    private QTEManager eventManager;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button defendButton;
    [SerializeField] private Button breakButton;
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private Slider breakSlider;
    private int[] keyToPressArray = {5, 5, 5, 5};

    private void Start()
    {
        eventManager = FindAnyObjectByType<QTEManager>();
        DisplayText("Begin Training!");
        EnableButtons();
    }

    public void OnEventButtonPress(string textToDisplay) 
    {
        //StopAllCoroutines();

        DisplayText(textToDisplay);

        StartCoroutine(TriggerQTE());
    }

    public void OnBreakButtonPress(string textToDisplay)
    {
        //StopAllCoroutines();

        DisplayText(textToDisplay);
        StartCoroutine(TriggerBreak());
    }

    public void DisplayText(string textToDisplay) 
    {
        displayText.text = textToDisplay;
    }

    IEnumerator TriggerBreak()
    {
        breakSlider.value = 0;
        int[] breakArray = FindAnyObjectByType<Unit>().keyToPressArray;
        eventManager.TriggerQTEArray(4, breakArray, true);
        DisableButtons();
        yield return new WaitUntil(() => eventManager.eventCompleted == true);
        eventManager.StopAllCoroutines();
        EnableButtons();
        
        string endString = "Continue Training? ";

        if (eventManager.eventResult == QTEResult.HIGH)
        {
            endString += "Your last performance was Perfect!";
        }

        else if (eventManager.eventResult == QTEResult.MID)
        {
            endString += "Your last performance was Medium.";
        }

        else
        {
            endString += "Your last performance was Weak...";
        }

        DisplayText(endString);
    }

    IEnumerator TriggerQTE() 
    {
        int eventType = Random.Range(1,5);

        switch (eventType)
        {
            case 1: // Timed
                eventManager.TriggerTimedQTE(5, 5);
                //Change color of QTE circle
                break;

            case 2: // Mash
                eventManager.TriggerMashQTE(5, 5, 10);
                //Change color of QTE circle
                break;

            case 3: // Array
                /*
                foreach (int key in keyToPressArray)
                {

                }
                */
                eventManager.TriggerQTEArray(5, keyToPressArray, false);
                //Change color of QTE circle
                break;

            case 4: // Standard
                eventManager.GenerateStandardQTE(5);
                //Change color of QTE circle
                break;

            default:
                Debug.Log("ERROR! attackType not recognized. Defaulting to Standard attack");
                eventManager.GenerateStandardQTE(3);
                break;
        }
        DisableButtons();
        yield return new WaitUntil(() => eventManager.eventCompleted == true);

        string endString = "Continue Training? ";

        if (eventManager.eventResult == QTEResult.HIGH)
        {
            breakSlider.value += 34;
            endString += "Your last performance was Perfect!";
        }

        else if (eventManager.eventResult == QTEResult.MID) 
        {
            endString += "Your last performance was Medium.";
        }

        else
        {
            endString += "Your last performance was Weak...";
        }

        eventManager.StopAllCoroutines();
        EnableButtons();
        DisplayText(endString);

    }



    public void DisableButtons()
    {
        attackButton.gameObject.SetActive(false);
        defendButton.gameObject.SetActive(false);
        breakButton.gameObject.SetActive(false);
    }

    public void EnableButtons()
    {
        attackButton.gameObject.SetActive(true);
        defendButton.gameObject.SetActive(true);
        breakButton.gameObject.SetActive(true);

        if (breakSlider.value >= 100)
        {
            breakButton.interactable = true;
        }

        else
        {
            breakButton.interactable = false;
        }

    }



}

