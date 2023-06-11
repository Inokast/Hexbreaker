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

    private int correctKeyPressedNum;
    private bool eventOngoing;

    private string eventType;

    [SerializeField] private GameObject buttonDisplay;
    private GameObject currentButtonDisplay;
    private Transform displayStartPos;

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

    public void GenerateTimedQTE(float duration) 
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

    public void QTEArrayTest() 
    {
        int[] keysToPress = {1, 2, 3, 4};
        StartCoroutine(GenerateQTEArray(5, keysToPress));
    }

    IEnumerator GenerateQTEArray(float duration, int[] keysToPress)
    {
        StopCoroutine(ClearQTESequence());
        eventPanel.SetActive(true);
        resultDisplayText.text = "";
        //eventGen = Random.Range(1, 5);
        eventOngoing = true;
        timerDuration = duration;
        StartCoroutine(Timer());

        int totalKeys = keysToPress.Length;

        for (int i = 0; i < totalKeys; i++)
        {
            if (timerIsActive == true)
            {
                print("Loop for key " + keysToPress[i]);
                isWaitingForInput = true;
                eventGen = keysToPress[i];

                switch (eventGen)
                {
                    case 1:
                        inputDisplayText.color = Color.white;
                        inputDisplayText.text = "[W]";
                        break;

                    case 2:
                        inputDisplayText.color = Color.white;
                        inputDisplayText.text = "[D]";
                        break;

                    case 3:
                        inputDisplayText.color = Color.white;
                        inputDisplayText.text = "[S]";
                        break;

                    case 4:
                        inputDisplayText.color = Color.white;
                        inputDisplayText.text = "[A]";
                        break;

                    default:
                        Debug.Log("Error! QTE Event generated not recognized. Please input 1 (Up), 2 (Right), 3 (Down), or 4 (Left).");
                        isWaitingForInput = false;
                        break;
                }

                if (i == totalKeys - 1)
                {
                    eventOngoing = false;
                    print("Fourth loop " + i);
                }

                yield return new WaitUntil(() => isWaitingForInput == false);
            }

            else
            {
                eventOngoing = false;
                yield break;
            }
        }

        timerIsActive = false;
        //isWaitingForInput = false;
        print("Made it out of loop");

        if (correctKeyPressedNum == totalKeys)
        {
            eventResult = QTEResult.HIGH;
            ClearQTESequence();
            print("Perfect end");
        }

        else if (correctKeyPressedNum == 0)
        {
            eventResult = QTEResult.LOW;
            ClearQTESequence();
            print("Missed end");
        }

        else 
        {
            eventResult = QTEResult.MID;
            ClearQTESequence();
            print("Mid end");
        }

    }

    public void GenerateStandardQTE(float duration) 
    {
        StopCoroutine(ClearQTESequence());
        eventOngoing = true;
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
            correctKeyPressedNum += 1;
            eventResult = QTEResult.HIGH;
            resultDisplayText.text = "Nice!";
            inputDisplayText.color = Color.yellow;

            if (!eventOngoing) 
            {
                StartCoroutine(ClearQTESequence());
            }

            else 
            {
                isWaitingForInput = false;
            }
        }

        else if (inputWasCorrect == false)
        {
            eventResult = QTEResult.LOW;
            resultDisplayText.text = "Fail...";
            inputDisplayText.color = Color.red;

            if (!eventOngoing)
            {
                StartCoroutine(ClearQTESequence());
            }

            else
            {
                isWaitingForInput = false;
            }

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
            eventOngoing = false;
            eventGen = 0;
            eventResult = QTEResult.LOW;            
            resultDisplayText.text = "Missed...";
            inputDisplayText.color = Color.red;
            StartCoroutine(ClearQTESequence());
        }
    }

    IEnumerator ClearQTESequence() 
    {
        eventOngoing = false;
        timerIsActive = false;
        //yield return new WaitForSeconds(1.5f);
        inputWasCorrect = false;
        isWaitingForInput = false;
        yield return new WaitForSeconds(2f);             
        inputDisplayText.text = "";
        resultDisplayText.text = "";
        correctKeyPressedNum = 0;
        eventPanel.SetActive(false);
        inputDisplayText.color = Color.white;
    }
}
