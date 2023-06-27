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
        if(Input.GetKeyDown("w"))
        {
            playerAnim.SetTrigger("PlayerATK");
        }

        if(Input.GetKeyDown("q"))
        {
            playerAnim.SetTrigger("PlayerFocus");
        }

        if(Input.GetKeyDown("e"))
        {
            playerAnim.SetTrigger("FocusATK");
        }
    }
}
