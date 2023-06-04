using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIManager : MonoBehaviour
{

    [SerializeField] private GameObject settingsPanel;
    private LevelManager levelManager;

    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();

        /// if (!DataPersistenceManager.instance.HasGameData)
        /// {
        ///     continueGameButton.SetActive(false);
        /// ?
    }
    public void OnNewGameButton() 
    {
        // create a new game - which will initialize game data
        DataPersistenceManager.instance.NewGame();
        // Load the gameplay scene - which will in turn save the game
        levelManager.LoadSceneWithName("Overworld"); // Whatever the name of our tutorial scene is
        // disable the ability to select a different button so that it cannot load multiple games asynchronously
    }

    public void OnContinueGameButton() 
    {
        // Load the next scene - which will load the game data
        levelManager.LoadSceneWithName("Overworld");
    }
    public void OnLoadNewSceneButton(string sceneName) 
    {
        levelManager.LoadSceneWithName(sceneName);
    }

    public void ToggleSettingsPanel() 
    {
        if (settingsPanel.activeSelf == true)
        {
            settingsPanel.SetActive(false);
        }

        else { settingsPanel.SetActive(true); }
    }

    public void OnQuitButtonPress() 
    {
        Application.Quit();
    }
}
