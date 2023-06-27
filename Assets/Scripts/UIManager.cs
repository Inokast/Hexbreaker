using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIManager : MonoBehaviour
{

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject helpPanel;
    [SerializeField] private GameObject creditsPanel;
    private LevelManager levelManager;
    private DataPersistenceManager dataManager;

    private SoundFXController sfx;

    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        sfx = FindAnyObjectByType<SoundFXController>();
    }
    public void OnNewGameButton() 
    {
        sfx.PlayButtonSelect();
        // create a new game - which will initialize game data
        DataPersistenceManager.instance.NewGame();
        // Load the gameplay scene - which will in turn save the game
        levelManager.LoadSceneWithName("Overworld"); // Whatever the name of our tutorial scene is
        // disable the ability to select a different button so that it cannot load multiple games asynchronously
    }

    public void OnContinueGameButton() 
    {
        // Load the next scene - which will load the game data
        sfx.PlayButtonSelect();
        levelManager.LoadSceneWithName("Overworld");
    }

    public void OnPlayAgainButton()
    {
        dataManager = FindObjectOfType<DataPersistenceManager>();
        sfx.PlayButtonSelect();
        dataManager.NewGame();
        levelManager.LoadSceneWithName("MainMenu");
    }

    public void OnLoadNewSceneButton(string sceneName)
    {
        sfx.PlayButtonSelect();
        levelManager.LoadSceneWithName(sceneName);        
    }

    public void ToggleSettingsPanel()
    {
        if (settingsPanel.activeSelf == true)
        {
            settingsPanel.SetActive(false);
        }

        else { settingsPanel.SetActive(true); }

        sfx.PlayButtonSelect();
    }

    public void ToggleCreditsPanel()
    {
        if (creditsPanel.activeSelf == true)
        {
            creditsPanel.SetActive(false);
        }

        else { creditsPanel.SetActive(true); }

        sfx.PlayButtonSelect();
    }

    public void ToggleHelpPanel()
    {
        if (helpPanel.activeSelf == true)
        {
            helpPanel.SetActive(false);
        }

        else { helpPanel.SetActive(true); }

        sfx.PlayButtonSelect();
    }

    public void OnQuitButtonPress() 
    {
        Application.Quit();
    }
}
