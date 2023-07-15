using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool initializeDataIfNull;

    [Header("File Storage Config")]

    [SerializeField] private string fileName;

    private GameData gameData;

    private List<IDataPersistence> dataPersistenceObjects;

    private FileDataHandler dataHandler;

    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }

        else if (instance != null) 
        {
            Destroy(gameObject);
        }

        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void Start()
    {
        LoadGame();
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
    {
        //Debug.Log("OnSceneLoaded Called");
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void OnSceneUnloaded(Scene scene) 
    {
        //Debug.Log("OnSceneUnoaded Called");
        SaveGame();
    }

    public void NewGame() 
    {
        CreateTalismans tg = GameObject.Find("TalismanGenerator").GetComponent<CreateTalismans>();

        if (tg.talismans != null)
        {
            foreach (GameObject talisman in tg.talismans)
            {
                tg.talismans[0].SetActive(true);

                Destroy(tg.talismans[0]);
            }

            tg.talismans.Clear();

            tg.action.Clear();
        }

        gameData = new GameData();
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public void LoadGame() 
    {
        // Load any saved data from a file using the data handler
        gameData = dataHandler.Load();

        if (gameData == null && initializeDataIfNull) 
        {
            NewGame();
        }

        // if no data can be loaded, initialize to a new game
        if (gameData == null) 
        {
            Debug.Log("No data was found. A New Game needs to be started before data can be loaded");
            return;
        }

        // push the loaded data to all other scripts that need it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public void SaveGame() 
    {
        if (gameData == null) 
        {
            Debug.Log("No data was found when trying to save game. A New Game needs to be started before data can be saved");
        }

        // pass the data to other scripts so they can update it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(gameData);
        }


        // Save that data to a file using the data handler
        dataHandler.Save(gameData);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public bool HasGameData()
    {
        return gameData != null;
    }
}
