//Hexbreaker - Break Meter System
//Last Modified 05/28/23
//Script that manages the break meter, it needs to tie into a great many other systems.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreakMeter : MonoBehaviour
{
    //I don't usually like assigning values to public variables, so
    //instead I've found the slider and game object with code.

    private Slider breakSlider;

    public static float charge;

    private void Start()
    {
        breakSlider = gameObject.GetComponentInChildren<Slider>();

        breakSlider.maxValue = 100;

        breakSlider.value = 0;

        charge = 0;
    }

    public void ChangeMeterValue(int value)
    {
        breakSlider.value += value;

        charge = breakSlider.value;

        if (breakSlider.value > 100)
        {
            breakSlider.value = 100;

            charge = breakSlider.value;
        }
    }

    //I don't know how I want to do the below system yet. I might do it with an array/list
    //of integers, or I might use GameObjects. We'll see.

    public void BreakCurse(GameObject curse)
    {
        //Play an animation of the modifier being destroyed maybe.

        ChangeMeterValue(-100);

        curse.SetActive(false); //If applicable, also stop all coroutines before setting inactive, as they will continue otherwise.

        //Display text saying that the modifier is broken. Use the game output text? Or use a different text?
    }
}
