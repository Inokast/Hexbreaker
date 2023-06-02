using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransitionScreen : MonoBehaviour
{
    public Animator screen;

    public float transitionTime = 1f;

    // Start is called before the first frame update

    public void StartTransition() 
    {
        screen.SetTrigger("start");
    }

}
