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
    [SerializeField] private AudioSource MagicImpact;
    [SerializeField] private AudioSource QTEchargeUp;
    [SerializeField] private AudioSource QTEchargeAlt;
    [SerializeField] private AudioSource QTEsuccess;
    [SerializeField] private AudioSource QTEfailure;
    [SerializeField] private AudioSource CurseShatter;


    [Header("Attack Sounds")]
    [SerializeField] private AudioSource PlayerAttack_Orb;
    [SerializeField] private AudioSource PlayerAttack_Burst;
    [SerializeField] private AudioSource PlayerAttack_Beam;
    [SerializeField] private AudioSource PlayerAttack_Focus;
    [SerializeField] private AudioSource EnemyAttack_Burst;
    [SerializeField] private AudioSource EnemyAttack_Beam;
    [SerializeField] private AudioSource EnemyAttack_Portal;
    [SerializeField] private AudioSource PF_AttackCharge;
    [SerializeField] private AudioSource PF_AttackAlt;



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


    public void PlayButtonSelect()
    {
        buttonSelect.Play();
    }

    public void PlayTalismanSelect()
    {
        talismanSelect.Play();
    }

    public void PlayLuckyNode()
    {
        luckyNode.Play();
    }

    public void PlayGameOver()
    {
        gameOver.Play();
    }

    public void PlayWinSound() 
    {
        winSound.Play();
    }

    public void PlayPlayerHit()
    {
        playerHit.Play();
    }
    public void PlayMagicImpact()
    {
        MagicImpact.Play();
    }

    public void PlayQTEchargeUp()
    {
       QTEchargeUp.Play();
    }
    public void PlayQTEchargeAlt()
    {
        QTEchargeAlt.Play();
    }

    public void PlayQTEsuccess()
    {
        QTEsuccess.Play();
    }

    public void PlayQTEFailure()
    {
        QTEfailure.Play();
    }

    public void PlayCurseShatter()
    {
        CurseShatter.Play();
    }
    public void PlayPlayerAttack_Orb()
    {
        PlayerAttack_Orb.Play();
    }
    public void PlayPlayerAttack_Beam()
    {
        PlayerAttack_Beam.Play();
    }
    public void PlayPlayerAttack_Burst()
    {
        PlayerAttack_Burst.Play();
    }
    public void PlayPlayerAttack_Focus()
    {
        PlayerAttack_Focus.Play();
    }
    public void PlayEnemyAttack_Beam()
    {
        EnemyAttack_Beam.Play();
    }
    public void PlayEnemyAttack_Burst()
    {
        EnemyAttack_Burst.Play();
    }
    public void PlayEnemyAttack_Portal()
    {
        EnemyAttack_Portal.Play();
    }
    public void PlayPF_AttackCharge()
    {
        PF_AttackCharge.Play();
    }
    public void PlayPF_AttackAlt()
    {
        PF_AttackAlt.Play();
    }

}
