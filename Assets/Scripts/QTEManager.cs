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
    [Header("UI Connections")]
    public GameObject eventPanel;
    public TextMeshProUGUI inputDisplayText;
    public TextMeshProUGUI resultDisplayText;

    [Header("General Variables")]
    private float timerDuration;
    private int eventGen; // QTE generator
    private bool isWaitingForInput;
    private string eventType;
    private bool inputWasCorrect;
    private bool timerIsActive;
    public QTEResult eventResult;


    [Header("Advanced Variables")]
    private int correctKeyPressedNum;
    private bool eventOngoing;
    private float amountFilled;

    

    [SerializeField] private GameObject buttonDisplay;
    private GameObject currentButtonDisplay;
    private Transform displayStartPos;


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
        eventType = "Timed";
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

    public void MashQTETest()
    {
        StartCoroutine(GenerateMashQTE(5, 1, 10));
    }

    IEnumerator GenerateMashQTE(float duration, int keyToPress, float amountToFill)
    {
        StopCoroutine(ClearQTESequence());
        eventType = "Mash";
        eventPanel.SetActive(true);
        resultDisplayText.text = "Mash!";
        //eventGen = Random.Range(1, 5);
        eventOngoing = true;
        timerDuration = duration;
        StartCoroutine(Timer());

        eventGen = keyToPress;
        amountFilled = 0;

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
        isWaitingForInput = true;

        yield return new WaitUntil(() => amountFilled >= amountToFill);
        timerIsActive = false;
        isWaitingForInput = false;
        resultDisplayText.text = "Nice!";
        inputDisplayText.color = Color.yellow;

        eventResult = QTEResult.HIGH;
        StartCoroutine(ClearQTESequence());

    }

    IEnumerator GenerateQTEArray(float duration, int[] keysToPress)
    {
        StopCoroutine(ClearQTESequence());
        eventPanel.SetActive(true);
        eventType = "Array";
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

        if (correctKeyPressedNum == totalKeys)
        {
            eventResult = QTEResult.HIGH;
            //print("Perfect end");
        }

        else if (correctKeyPressedNum == 0)
        {
            eventResult = QTEResult.LOW;
            //print("Missed end");
        }

        else 
        {
            eventResult = QTEResult.MID;
            //print("Mid end");
        }

        StartCoroutine(ClearQTESequence());

    }

    public void GenerateStandardQTE(float duration) 
    {
        StopCoroutine(ClearQTESequence());
        eventType = "Standard";
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
        if (eventType != "Mash") 
        {
            eventGen = 0; // Reset
        }
        
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
                if (eventType != "Mash")
                {
                    isWaitingForInput = false;
                    
                }

                else 
                {
                    resultDisplayText.text = "Keep Going!";
                    amountFilled += 1;
                }
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
        yield return new WaitForSeconds(1f);             
        inputDisplayText.text = "";
        resultDisplayText.text = "";
        correctKeyPressedNum = 0;
        amountFilled = 0;
        eventPanel.SetActive(false);
        inputDisplayText.color = Color.white;
    }
}
