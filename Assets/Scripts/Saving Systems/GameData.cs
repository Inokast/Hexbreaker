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

    [Header("Settings Data")]
    public float bgmVolumeLevel;
    public float sfxVolumeLevel;

    [Header("Combat Data")]
    public int enemyUnitToLoadID;
    public bool combatFinished;

    [Header("Player Unit Data")]
    public string playerUnitName;
    public int playerHighDamage;
    public int playerMidDamage;
    public int playerLowDamage;
    public int playerDefense;
    public int playerMaxHP;
    public int playerCurrentHP;

    
    
    

    // The values defined in this constructor will be the default values
    // the game starts with when there's no data to load - Dan

    public GameData() 
    {
        playerPositionInWorld = new Vector3(0, .5f, -8);
        worldGenerated = false;
        lastSelectedNodeID = "";
        rightRoomLocked = false;
        leftRoomLocked = false;
        playerDied = false;
        //lastActiveSceneName = "CombatSceneForest";

        nodesActive = new SerializableDictionary<string, bool>();
        nodesCompleted = new SerializableDictionary<string, bool>();

        bgmVolumeLevel = .5f;
        sfxVolumeLevel = .5f;
        
        enemyUnitToLoadID = 0;

        playerUnitName = "Rosalin";
        playerHighDamage = 6;
        playerMidDamage = 4;
        playerLowDamage = 2;
        playerDefense = 0;
        playerMaxHP = 70;
        playerCurrentHP = 70;

    }

}
