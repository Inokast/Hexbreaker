using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundFXController : MonoBehaviour, IDataPersistence
{

    public float sfxVolumeLevel;
    public AudioMixer sfxMixer;

    [Header("UI Sounds")]
    [SerializeField] private AudioSource buttonSelect;
    [SerializeField] private AudioSource talismanSelect;
    [SerializeField] private AudioSource luckyNode;
    [SerializeField] private AudioSource gameOver;
    [SerializeField] private AudioSource winSound;

    [Header("Battle Sounds")]
    [SerializeField] private AudioSource playerHit;
    [SerializeField] private AudioSource enemyHit;
    [SerializeField] private AudioSource qtePass;
    [SerializeField] private AudioSource qteFail;


    //[Header("Enemy Sounds")]
    //[SerializeField] private AudioSource dogSound;


    //[Header("Impact Sounds")]
    //[SerializeField] private AudioSource glassHit;

    public void SetSoundLevel(float volumeLevel)
    {
        sfxVolumeLevel = volumeLevel;
        //parameters use exposed parameter from Unity and mathf.Log10 to convert from linear to logarithmic, then multiplied times 20
        sfxMixer.SetFloat("SFXmixer", Mathf.Log10(sfxVolumeLevel) * 20);
    }

    public void LoadData(GameData data)
    {
        sfxVolumeLevel = data.sfxVolumeLevel;
        SetSoundLevel(sfxVolumeLevel);
    }

    public void SaveData(GameData data)
    {
        data.sfxVolumeLevel = sfxVolumeLevel;
    }


    public void PlaySelect()
    {
        select.Play();
    }

    



}
