using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToOverworld : MonoBehaviour
{
    public void Return()
    {
        SceneManager.LoadScene("Overworld");
    }
}
