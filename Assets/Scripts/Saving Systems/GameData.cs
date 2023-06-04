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

    [Header("Settings Data")]
    public float bgmVolumeLevel;
    public float sfxVolumeLevel;

    [Header("Combat Data")]
    public int enemyUnitToLoadID;

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
        //lastActiveSceneName = "CombatSceneForest";

        bgmVolumeLevel = .5f;
        sfxVolumeLevel = .5f;
        
        enemyUnitToLoadID = 0;

        playerUnitName = "Player";
        playerHighDamage = 6;
        playerMidDamage = 4;
        playerLowDamage = 2;
        playerDefense = 0;
        playerMaxHP = 80;
        playerCurrentHP = 80;

    }

}
