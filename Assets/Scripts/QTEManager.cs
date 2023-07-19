// Hexbreaker - Quick Time Event System
// Last modified: 06/08/23 - Dan Sanchez
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
    [SerializeField] private TextMeshProUGUI timerText;

    [SerializeField] private QTEDisplay mashQTEDisplay;
    [SerializeField] private QTEDisplay arrayQTEDisplay;
    [SerializeField] private QTEDisplay timedQTEDisplay;
    [SerializeField] private QTEDisplay breakQTEDisplay;
    [SerializeField] private QTEDisplay standardQTEDisplay;

    private QTEDisplay activeQTEDisplay;

    private VFXController vfx;



    [Header("General Variables")]
    private float timerDuration;
    private int eventGen; // QTE generator
    private bool isWaitingForInput;
    [HideInInspector] public string eventType;
    private bool inputWasCorrect;
    [HideInInspector] public bool timerIsActive;
    [HideInInspector] public bool eventCompleted = true;
    [HideInInspector] public QTEResult eventResult;

    private bool holdingDown;


    [Header("Advanced Variables")]
    private int correctKeyPressedNum;
    private bool eventOngoing;
    private float amountFilled;
    private bool perfectTiming = false;

    public void Start()
    {
        vfx = FindAnyObjectByType < VFXController> ();
        
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
            if (Input.anyKeyDown && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1) &&
                !Input.GetMouseButtonDown(2)) 
            {
                holdingDown = true;
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

            if (!Input.anyKey && holdingDown) 
            {
                activeQTEDisplay.DisplayNormal();
                resultDisplayText.text = "";
                holdingDown = false;
            }
        }
    }

    private void InputReceived()
    {
        if (eventType != "Mash")
        {
            eventGen = 0; // Reset
        }

        if (inputWasCorrect)
        {
            activeQTEDisplay.DisplayPressed();

            if (eventType == "Timed")
            {
                if (perfectTiming == true)
                {
                    eventResult = QTEResult.HIGH;
                    resultDisplayText.text = "Nice!";
                    inputDisplayText.color = Color.yellow;
                    timerText.color = Color.green;
                    timedQTEDisplay.PlaySparkleEffect();
                    print("Playing Sparkles");
                }

                else
                {
                    eventResult = QTEResult.MID;
                    resultDisplayText.text = "Ok!";
                    inputDisplayText.color = Color.yellow;
                }

            }

            else
            {
                if (eventType == "Mash")
                {
                    float currentG = mashQTEDisplay.currentImage.color.g;
                    float gToAdd = 255 / mashQTEDisplay.colorGoal * amountFilled;

                    Color newColor = new Color32(181, ((byte)(currentG + gToAdd)), 0, 255);

                    mashQTEDisplay.currentImage.color = newColor;
                    activeQTEDisplay.currentImage.color = newColor;
                    currentG = mashQTEDisplay.currentImage.color.g;
                }

                correctKeyPressedNum += 1;
                eventResult = QTEResult.HIGH;
                resultDisplayText.text = "Nice!";
                inputDisplayText.color = Color.yellow;
            }


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
                    vfx.PlayParticleBurst(mashQTEDisplay.transform);
                    amountFilled += 1;
                }
            }
        }

        else if (inputWasCorrect == false && eventType != "Mash")
        {
            eventResult = QTEResult.LOW;
            if (eventType != "Array")
            {
                resultDisplayText.text = "Fail...";
            }

            else { resultDisplayText.text = "Miss!"; }
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

        else if (eventType != "Mash")
        {
            print("Error. Does not know if input result is null");
        }
    }

    IEnumerator Timer()
    {
        eventCompleted = false;
        timerIsActive = true;
        float seconds;
        float milliseconds;
        while (timerDuration > 0 && timerIsActive)
        {
            timerDuration -= Time.deltaTime;
            seconds = Mathf.FloorToInt(timerDuration % 60);
            milliseconds = Mathf.FloorToInt((timerDuration * 100f) % 100);
            timerText.text = string.Format("{0:00} : {1:0}", seconds, milliseconds);
            yield return null;
        }

        if (timerIsActive == true)
        {
            timerDuration = 0;
            seconds = Mathf.FloorToInt(timerDuration % 60);
            milliseconds = Mathf.FloorToInt((timerDuration * 100f) % 100);
            timerText.text = string.Format("{0:00} : {1:0}", seconds, milliseconds);

            timerText.color = Color.red;
            eventOngoing = false;
            eventGen = 0;
            eventResult = QTEResult.LOW;
            resultDisplayText.text = "Missed...";
            inputDisplayText.color = Color.red;
            StartCoroutine(ClearQTESequence());
        }
    }

    private void ClearQTEDisplays()
    {
        activeQTEDisplay.ResetColor();
        arrayQTEDisplay.gameObject.SetActive(false);
        timedQTEDisplay.gameObject.SetActive(false);
        breakQTEDisplay.gameObject.SetActive(false);
        mashQTEDisplay.gameObject.SetActive(false);
        standardQTEDisplay.gameObject.SetActive(false);
    }
    IEnumerator ClearQTESequence()
    {
        

        if (eventType == "Timed") 
        {
            timedQTEDisplay.StopAllCoroutines();
        }

        timerIsActive = false;
        eventOngoing = false;
        inputWasCorrect = false;
        isWaitingForInput = false;
        perfectTiming = false;
        yield return new WaitForSeconds(1f);
        inputDisplayText.transform.position = mashQTEDisplay.transform.position;
        inputDisplayText.text = "";
        resultDisplayText.text = "";
        timerText.text = "";
        timerText.color = Color.white;
        correctKeyPressedNum = 0;
        amountFilled = 0;
        eventPanel.SetActive(false);
        activeQTEDisplay.ResetPosition();
        timedQTEDisplay.ResetRingScale();
        ClearQTEDisplays();
        inputDisplayText.color = Color.white;
        eventCompleted = true;
    }

    public void QTETestArray() 
    {
        int[] QTE = new int[] { 1, 2, 3, 4 };
        TriggerQTEArray(3f, QTE, false);
    }

    public void QTETestMash()
    {
        TriggerMashQTE(3f, 1, 5);
    }

    public void QTETestTimed()
    {
        TriggerTimedQTE(2.5f, 5);
    }

    public void QTETestBreak()
    {
        int[] QTE = new int[] { 1, 2, 3, 4, 1, 2, 3, 4 };
        TriggerQTEArray(3f, QTE, true);
    }

    public void TriggerQTEArray(float duration, int[] keysToPress, bool isBreak) 
    {
        StartCoroutine(GenerateQTEArray(duration, keysToPress, isBreak));
    }

    public void TriggerMashQTE(float duration, int keyToPress, float amountToFill)
    {
        StartCoroutine(GenerateMashQTE(duration, keyToPress, amountToFill));
    }

    public void TriggerTimedQTE(float duration, int keyToPress)
    {
        StartCoroutine(GenerateTimedQTE(duration, keyToPress));
    }

    #region QTECalls
    IEnumerator GenerateTimedQTE(float duration, int keyToPress)
    {
        StopCoroutine(ClearQTESequence());
        eventType = "Timed";
        timedQTEDisplay.gameObject.SetActive(true);
        activeQTEDisplay = timedQTEDisplay;
        eventPanel.SetActive(true);
        
        eventGen = keyToPress;
        timerDuration = duration;

        if (eventGen == 5)
        {
            eventGen = Random.Range(1, 5);
        }

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
        resultDisplayText.text = "Wait!";
        //yield return new WaitForSeconds(.5f);

        isWaitingForInput = true;
        

        StartCoroutine(Timer());
        timedQTEDisplay.TimerRing(duration);
        yield return new WaitForSeconds(duration * .8f);

        if (timerIsActive == true)
        {
            timedQTEDisplay.currentImage.color = Color.green;
            resultDisplayText.text = "Now!";
            perfectTiming = true;
            timerText.color = Color.yellow;
        }
    }
    IEnumerator GenerateMashQTE(float duration, int keyToPress, float amountToFill)
    {
        StopCoroutine(ClearQTESequence());
        eventType = "Mash";
        mashQTEDisplay.gameObject.SetActive(true);
        activeQTEDisplay = mashQTEDisplay;
        eventPanel.SetActive(true);
        resultDisplayText.text = "Mash!";
        eventOngoing = true;
        timerDuration = duration;
        StartCoroutine(Timer());

        eventGen = keyToPress;
        amountFilled = 0;
        if (eventGen == 5) 
        {
            eventGen = Random.Range(1, 5);
        }

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
        mashQTEDisplay.colorGoal = amountToFill;

        yield return new WaitUntil(() => amountFilled >= amountToFill);
        timerIsActive = false;
        isWaitingForInput = false;
        resultDisplayText.text = "Nice!";
        inputDisplayText.color = Color.yellow;

        eventResult = QTEResult.HIGH;
        StartCoroutine(ClearQTESequence());

    }
    IEnumerator GenerateQTEArray(float duration, int[] keysToPress, bool isBreak)
    {
        StopCoroutine(ClearQTESequence());
        if (isBreak)
        {
            activeQTEDisplay = breakQTEDisplay;
            breakQTEDisplay.gameObject.SetActive(true);
        }

        else { arrayQTEDisplay.gameObject.SetActive(true); activeQTEDisplay = arrayQTEDisplay; }

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

                if (eventGen == 5)
                {
                    eventGen = Random.Range(1, 5);
                }

                activeQTEDisplay.ResetPosition();
                inputDisplayText.transform.position = activeQTEDisplay.transform.position;
                

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

                activeQTEDisplay.MovePosition(eventGen);
                inputDisplayText.transform.position = activeQTEDisplay.transform.position;

                if (i == totalKeys - 1)
                {
                    eventOngoing = false;
                }

                yield return new WaitUntil(() => isWaitingForInput == false);
            }

            else
            {
                activeQTEDisplay.ResetPosition();
                inputDisplayText.transform.position = activeQTEDisplay.transform.position;
                eventOngoing = false;
                yield break;
            }
        }

        timerIsActive = false;

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
        activeQTEDisplay = standardQTEDisplay;
        activeQTEDisplay.gameObject.SetActive(true);       
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

        eventOngoing = false;
    }

    #endregion
    
}
