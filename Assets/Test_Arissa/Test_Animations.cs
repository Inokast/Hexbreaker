using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Animations : MonoBehaviour
{
    public Animator playerAnim;

    // Start is called before the first frame update
    void Start()
    {
        // playerAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("q"))
        {
            playerAnim.SetTrigger("Jump");
        }

        if(Input.GetKeyDown("w"))
        {
            playerAnim.SetTrigger("Land");
        }

        if(Input.GetKeyDown("e"))
        {
            playerAnim.SetTrigger("FocusATK");
        }

        if(Input.GetKeyDown("r"))
        {
            playerAnim.SetTrigger("FocusHit");
        }

        if(Input.GetKeyDown("t"))
        {
            playerAnim.SetTrigger("Hit");
        }

        if(Input.GetKeyDown("y"))
        {
            playerAnim.SetTrigger("Death");
        }
    }
}
