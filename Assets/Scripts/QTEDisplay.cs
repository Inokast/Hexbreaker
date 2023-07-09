using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QTEDisplay : MonoBehaviour
{
    [SerializeField] private Color defaultColor;

    [SerializeField] private Sprite button;
    [SerializeField] private Sprite pressedButton;

    public float colorGoal;

    public Image currentImage;


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

    public void PlaySparkleEffect() 
    {

    }

    public void PlayFlamesEffect() 
    {

    }
}
