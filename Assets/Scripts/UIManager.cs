using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    

    public void LoadNewScene(string sceneName) 
    {
        SceneManager.LoadScene(sceneName);
    }

    public void RestartCurrentScene() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnPlayButtonPress() 
    {
        LoadNewScene("CombatSceneForest");
    }

    public void OnQuitButtonPress() 
    {
        Application.Quit();
    }
}
