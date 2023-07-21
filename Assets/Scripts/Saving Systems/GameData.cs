using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class GameData
{
    // If you want to add data to be tracked, make a new variable under an appropriate header and name it accordingly. Then reference it in the script you need it for via SaveGame and LoadGame functions. - Dan

    [Header("Overworld Data")]
    public Vector3 playerPositionInWorld;   
    //public string lastActiveSceneName;
    public bool worldGenerated;
    public string lastSelectedNodeID;
    public bool rightRoomLocked;
    public bool leftRoomLocked;
    public bool playerDied;
    public SerializableDictionary<string, bool> nodesActive;
    public SerializableDictionary<string, bool> nodesCompleted;
    public SerializableDictionary<string, int> nodesTypes;
    public SerializableDictionary<string, string> nodesDescriptions;
    public List<string> nodeIDForEnemies;
    public List<string> nodesEnemies;

    [Header("Settings Data")]
    public float bgmVolumeLevel;
    public float sfxVolumeLevel;

    [Header("Combat Data")]
    public List<int> enemyUnitToLoadID;
    public bool combatFinished;

    [Header("Player Unit Data")]
    public string playerUnitName;
    public int playerHighDamage;
    public int playerMidDamage;
    public int playerLowDamage;
    public int playerDefense;
    public int playerMaxHP;
    public int playerCurrentHP;

    [Header("Talisman Manager Settings")]
    //public List<GameObject> talismansCollected;
    public List<bool> isAction;
    public List<string> talisName;
    public List<int> talisFirstStat;
    public List<int> talisSecondStat;
    public List<int> talisRarity;


    // The values defined in this constructor will be the default values
    // the game starts with when there's no data to load - Dan

    public GameData() 
    {
        playerPositionInWorld = new Vector3(-.4f, .55f, -12f);
        worldGenerated = false;
        lastSelectedNodeID = "";
        rightRoomLocked = false;
        leftRoomLocked = false;
        playerDied = false;
        //lastActiveSceneName = "CombatSceneForest";

        nodesActive = new SerializableDictionary<string, bool>();
        nodesCompleted = new SerializableDictionary<string, bool>();
        nodesDescriptions = new SerializableDictionary<string, string>();
        nodesTypes = new SerializableDictionary<string, int>();

        nodeIDForEnemies = new List<string>();
        nodesEnemies = new List<string>();

        //talismansCollected = new List<GameObject>();
        isAction = new List<bool>();
        talisName = new List<string>();
        talisFirstStat = new List<int>();
        talisSecondStat = new List<int>();
        talisRarity = new List<int>();

        bgmVolumeLevel = .2f;
        sfxVolumeLevel = .2f;
        
        enemyUnitToLoadID = new List<int> {0};

        playerUnitName = "Rosalin";
        playerHighDamage = 6;
        playerMidDamage = 4;
        playerLowDamage = 2;
        playerDefense = 0;
        playerMaxHP = 70;
        playerCurrentHP = 70;



    }

}
