using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManager : MonoBehaviour
{
    private SoundFXController sfx;

    [Header("Scene Configuration")]
    private LevelTransitionScreen transition;
    [SerializeField] private string nextLevelString;
    private UIManager ui;
    [SerializeField] private int playMusicID = 0;
    private BGMController bgm;
    
    private void Start()
    {
        ui = FindObjectOfType<UIManager>();
        transition = FindObjectOfType<LevelTransitionScreen>();
        bgm = FindObjectOfType<BGMController>();
        sfx = FindObjectOfType<SoundFXController>();
        bgm.PlayMusicWithID(playMusicID);
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
        //transition.StartTransition();

        yield return new WaitForSeconds(transition.transitionTime);

        string currentScene = SceneManager.GetActiveScene().name;

        SceneManager.LoadScene(currentScene);
    }

    IEnumerator LoadNextLevel()
    {        
        //transition.StartTransition();

        yield return new WaitForSeconds(transition.transitionTime);

        SceneManager.LoadScene(nextLevelString);
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
