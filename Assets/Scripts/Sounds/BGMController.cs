using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BGMController : MonoBehaviour, IDataPersistence
{
    static public BGMController BGM;

    public float bgmVolumeLevel;
    public AudioMixer bgmMixer;

    public AudioSource menuMusic;
    public AudioSource overworldMusic;
    public AudioSource normalBattleMusic;
    public AudioSource cathedralBattleMusic;
    public AudioSource bossBattleMusic;

    void Awake()
    {
        if (BGM == null)
        {
            DontDestroyOnLoad(gameObject);
            BGM = this;
        }
        else if (BGM != null)
        {
            Destroy(gameObject);
        }
    }

    public void SetMusicLevel(float volumeLevel)
    {
        bgmVolumeLevel = volumeLevel;
        //parameters use exposed parameter from Unity and mathf.Log10 to convert from linear to logarithmic, then multiplied times 20
        bgmMixer.SetFloat("BGMmixer", Mathf.Log10(bgmVolumeLevel) * 20);
    }

    public void LoadData(GameData data)
    {
        bgmVolumeLevel = data.bgmVolumeLevel;
        SetMusicLevel(bgmVolumeLevel);
    }

    public void SaveData(GameData data)
    {
        data.bgmVolumeLevel = bgmVolumeLevel;
    }

    public void PlayMusicWithID(int songID) 
    {
        switch (songID)
        {
            case 0:
                PlayMenuMusic();
                break;

            case 1: PlayOverworldMusic();
                break;

            case 2:
                PlayNormalBattleMusic();
                break;

            case 3:
                PlayCathedralBattleMusic();
                break;

            case 4:
                PlayBossBattleMusic();
                break;

            default:
                StopMusic();
                break;
        }
    }

    public void PlayMenuMusic()
    {
        StopMusic();
        menuMusic.Play();
    }

    public void PlayOverworldMusic()
    {
        StopMusic();
        overworldMusic.Play();
    }

    public void PlayNormalBattleMusic()
    {
        StopMusic();
        normalBattleMusic.Play();
    }

    public void PlayCathedralBattleMusic()
    {
        StopMusic();
        cathedralBattleMusic.Play();
    }

    public void PlayBossBattleMusic()
    {
        StopMusic();
        bossBattleMusic.Play();
    }

    public void StopMusic()
    {
        menuMusic.Stop();
        overworldMusic.Stop();
        normalBattleMusic.Stop();
        cathedralBattleMusic.Stop();
        bossBattleMusic.Stop();
    }
}
