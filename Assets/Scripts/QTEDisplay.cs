using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QTEDisplay : MonoBehaviour
{
    [Header("Particle Effects")]
    [SerializeField] ParticleSystem sparkleEffect;
    [SerializeField] ParticleSystem flamesEffect;
    [SerializeField] ParticleSystem otherEffect;


    [Header("Ring Connections")]
    [SerializeField] private GameObject ring;
    [SerializeField] private Vector3 targetScale;
    private Vector3 defaultScale;
    private bool isScaling = false;

    [SerializeField] private Color defaultColor;

    [SerializeField] private Sprite button;
    [SerializeField] private Sprite pressedButton;
  

    [HideInInspector] public float waitTime;

    [HideInInspector] public float colorGoal;

    [SerializeField] private Vector2 defaultPosition;

    public Image currentImage;

    private void Start()
    {
        if (ring != null) 
        {
            defaultScale = ring.transform.localScale;
        }

        defaultPosition = gameObject.GetComponent<RectTransform>().anchoredPosition;
    }

    public void MovePosition(int keyToPress)
    {
        RectTransform position = GetComponent<RectTransform>();

        switch (keyToPress)
        {
            case 1:
                position.Translate(Vector2.up * 100);
                break;

            case 2:
                position.Translate(Vector2.right * 100);
                break;

            case 3:
                position.Translate(Vector2.down * 100);
                break;

            case 4:
                position.Translate(Vector2.left * 100);
                break;

            default:
                Debug.Log("keyToPress not recognized at MovePosition function.");
                break;
        }
    }

    public void ResetPosition() 
    {
        RectTransform position = GetComponent<RectTransform>();
        position.anchoredPosition = defaultPosition;
    }

    public void DisplayNormal()
    {
        currentImage.sprite = button;
    }

    public void DisplayPressed()
    {
        currentImage.sprite = pressedButton;
    }

    public void ResetColor() 
    {
        currentImage.color = defaultColor;
    }

    public void ResetRingScale()
    {
        if (isScaling == true) 
        {
            ring.transform.localScale = defaultScale;
            isScaling = false;
        }       
    }

    public void TimerRing(float duration) 
    {
        StartCoroutine(ShrinkRingOverTime(duration));
    }

    IEnumerator ShrinkRingOverTime(float duration) 
    {
        if (isScaling)
        {
            yield break; //exit if this is still running
        }

        isScaling = true;

        duration = duration + .2f;

        float counter = 0;
        //Get the current scale of the object to be moved
        Vector3 startScaleSize = ring.transform.localScale;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            ring.transform.localScale = Vector3.Lerp(startScaleSize, targetScale, counter / duration);
            yield return null;
        }

        yield return new WaitForSeconds(1);
        ResetRingScale();
        
    }

    public void PlaySparkleEffect() 
    {
        sparkleEffect.Play();
    }

    public void PlayFlamesEffect() 
    {
        flamesEffect.Play();
    }

    public void PlayOtherEffect() 
    {
        otherEffect.Play();
    }
}
