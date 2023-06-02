using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class GameData
{

    public float bgmVolumeLevel;
    public float sfxVolumeLevel;
    public string lastActiveSceneName;

    // The values defined in this constructor will be the default values
    // the game starts with when there's no data to load - Dan

    public GameData() 
    {
        this.bgmVolumeLevel = .5f;
        this.sfxVolumeLevel = .5f;
        lastActiveSceneName = "CombatSceneForest";
    }

}
