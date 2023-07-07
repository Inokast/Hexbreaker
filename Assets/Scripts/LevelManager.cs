using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManager : MonoBehaviour
{
    private SoundFXController sfx;
    private BGMController bgm;

    [Header("Scene Configuration")]
    private LevelTransitionScreen transition;
    private string nextLevelString;
    [SerializeField] private int playMusicID = 0;
    
    
    private void Start()
    {
        transition = FindObjectOfType<LevelTransitionScreen>();      
        sfx = FindObjectOfType<SoundFXController>();      
        bgm = FindObjectOfType<BGMController>();
        bgm.PlayMusicWithID(playMusicID);

        if (SceneManager.GetActiveScene().name == "EndScene") 
        {
            FindAnyObjectByType<DataPersistenceManager>().NewGame();
        }
    }



    public void PauseGame() 
    {
        Time.timeScale = 0;        
    }

    public void ResumeGame() 
    {
        Time.timeScale = 1;
    }

    IEnumerator RestartLevel()
    {
        transition.StartTransition();

        yield return new WaitForSeconds(transition.transitionTime);

        string currentScene = SceneManager.GetActiveScene().name;

        SceneManager.LoadScene(currentScene);
    }

    IEnumerator LoadNextLevel()
    {        
        transition.StartTransition();

        yield return new WaitForSeconds(transition.transitionTime);

        SceneManager.LoadSceneAsync(nextLevelString);
    }
    public void LoadSceneWithName(string newLevelString) 
    {
        ResumeGame();
        //sfx.PlayLoadingLevel();
        nextLevelString = newLevelString;
        StartCoroutine(LoadNextLevel());
    }
    public void NextLevel()
    {
        ResumeGame();
        StartCoroutine(LoadNextLevel());
    }

    public void Restart()
    {
        ResumeGame();
        StartCoroutine(RestartLevel());
    }
}
