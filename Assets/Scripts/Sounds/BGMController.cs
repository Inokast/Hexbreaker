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
    public AudioSource world1Music;
    public AudioSource world2Music;
    public AudioSource world3Music;

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

            case 1: PlayWorld1Music();
                break;

            case 2:
                PlayWorld2Music();
                break;

            case 3:
                PlayWorld3Music();
                break;


            default:
                PlayMenuMusic();
                break;
        }
    }

    public void PlayMenuMusic()
    {
        StopMusic();
        menuMusic.Play();
    }

    public void PlayWorld1Music()
    {
        StopMusic();
        world1Music.Play();
    }

    public void PlayWorld2Music()
    {
        StopMusic();
        world2Music.Play();
    }

    public void PlayWorld3Music()
    {
        StopMusic();
        world3Music.Play();
    }

    public void StopMusic()
    {
        menuMusic.Stop();
        //world1Music.Stop();
        ////world2Music.Stop();
        //world3Music.Stop();
    }
}
