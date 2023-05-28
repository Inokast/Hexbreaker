// Hexbreaker - Quick Time Event System
// Last modified: 05/27/23 - Dan Sanchez
// Notes: The goal is to make this flexible enough to be used in any scene where we want a QTE.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QTEManager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI inputDisplayText;
    [SerializeField] private TextMeshProUGUI resultDisplayText;

    private float timerDuration;
    private int eventGen; // QTE generator
    private bool isWaitingForInput;
    private bool inputWasCorrect;
    private bool timerIsActive;

    public void Start()
    {
        resultDisplayText.text = "";
        inputDisplayText.text = "";
        isWaitingForInput = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isWaitingForInput == false)  // I put this here just for testing. Will loop QTE in game scene.
        {
            GenerateQTE(3.5f);
        }

        if (isWaitingForInput) 
        {
            if (Input.anyKeyDown) 
            {
                switch (eventGen)
                {
                    case 0:
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
        if (inputWasCorrect) 
        {
            resultDisplayText.text = "PASS";
            StartCoroutine(ClearQTESequence());
        }

        if (inputWasCorrect!) 
        {
            resultDisplayText.text = "FAIL";
            StartCoroutine(ClearQTESequence());
            
        }

        //StopCoroutine(Timer());
    }

    IEnumerator Timer() 
    {
        timerIsActive = true;
        yield return new WaitForSeconds(timerDuration);
        if (timerIsActive == true) 
        {
            eventGen = 0;
            resultDisplayText.text = "FAIL";
            StartCoroutine(ClearQTESequence());
        }
    }

    IEnumerator ClearQTESequence() 
    {
        yield return new WaitForSeconds(1.5f);
        inputWasCorrect = false;
        resultDisplayText.text = "";
        inputDisplayText.text = "";
        yield return new WaitForSeconds(1.5f);
        timerIsActive = false;
        isWaitingForInput = false;
        
    }
}
