// Hexbreaker - Quick Time Event System
// Last modified: 05/28/23 - Dan Sanchez
// Notes: The goal is to make this flexible enough to be used in any scene where we want a QTE.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum QTEResult { NONE, LOW, MID, HIGH }
public class QTEManager : MonoBehaviour
{
    public GameObject eventPanel;
    public TextMeshProUGUI inputDisplayText;
    public TextMeshProUGUI resultDisplayText;

    private float timerDuration;
    private int eventGen; // QTE generator
    private bool isWaitingForInput;
    private bool inputWasCorrect;
    private bool timerIsActive;

    public QTEResult eventResult;

    public void Start()
    {

        resultDisplayText.text = "";
        inputDisplayText.text = "";
        isWaitingForInput = false;
        eventPanel.SetActive(false);
        eventResult = QTEResult.NONE;    
    }

    // Update is called once per frame
    void Update()
    {
        if (isWaitingForInput == true) 
        {
            if (Input.anyKeyDown) 
            {
                switch (eventGen)
                {
                    case 0: print("Event did not generate");
                        break;

                    case 1:
                        if (Input.GetButtonDown("QTE Up"))
                        {
                            inputWasCorrect = true;
                            InputReceived();
                        }

                        else
                        {
                            inputWasCorrect = false;
                            InputReceived();
                        }
                        break;

                    case 2:
                        if (Input.GetButtonDown("QTE Right"))
                        {
                            inputWasCorrect = true;
                            InputReceived();
                        }

                        else
                        {
                            inputWasCorrect = false;
                            InputReceived();
                        }
                        break;

                    case 3:
                        if (Input.GetButtonDown("QTE Down"))
                        {
                            inputWasCorrect = true;
                            InputReceived();
                        }

                        else
                        {
                            inputWasCorrect = false;
                            InputReceived();
                        }
                        break;

                    case 4:
                        if (Input.GetButtonDown("QTE Left"))
                        {
                            inputWasCorrect = true;
                            InputReceived();
                        }

                        else
                        {
                            inputWasCorrect = false;
                            InputReceived();
                        }
                        break;

                    default:
                        Debug.Log("Error! QTE Event generated not recognized.");
                        break;                       
                }
            }            
        }
    }

    public void GenerateQTE(float duration) 
    {
        StopCoroutine(ClearQTESequence());
        eventPanel.SetActive(true);
        resultDisplayText.text = "";
        eventGen = Random.Range(1, 5);
        timerDuration = duration;
        StartCoroutine(Timer());

        switch (eventGen)
        {
            case 1:
                inputDisplayText.text = "[W]";
                break;

            case 2:
                inputDisplayText.text = "[D]";
                break;

            case 3:
                inputDisplayText.text = "[S]";
                break;

            case 4:
                inputDisplayText.text = "[A]";
                break;

            default:
                Debug.Log("Error! QTE Event generated not recognized.");
                break;
        }

        isWaitingForInput = true;
    }

    private void InputReceived() 
    {
        eventGen = 0; // Reset
        print("Input was detected");
        if (inputWasCorrect) 
        {
            eventResult = QTEResult.HIGH;
            resultDisplayText.text = "Nice!";
            inputDisplayText.color = Color.yellow;
            StartCoroutine(ClearQTESequence());
        }

        else if (inputWasCorrect == false)
        {
            eventResult = QTEResult.LOW;
            resultDisplayText.text = "Fail...";
            inputDisplayText.color = Color.red;
            StartCoroutine(ClearQTESequence());

        }

        else 
        {
            print("Error. Does not know if input result is null");
        }
    }

    IEnumerator Timer() 
    {
        timerIsActive = true;
        yield return new WaitForSeconds(timerDuration);
        if (timerIsActive == true) 
        {
            eventGen = 0;
            eventResult = QTEResult.LOW;            
            resultDisplayText.text = "Missed...";
            inputDisplayText.color = Color.red;
            StartCoroutine(ClearQTESequence());
        }
    }

    IEnumerator ClearQTESequence() 
    {
        timerIsActive = false;
        //yield return new WaitForSeconds(1.5f);
        inputWasCorrect = false;
        yield return new WaitForSeconds(2f);     
        isWaitingForInput = false;
        inputDisplayText.text = "";
        resultDisplayText.text = "";
        eventPanel.SetActive(false);
        inputDisplayText.color = Color.white;
    }
}
