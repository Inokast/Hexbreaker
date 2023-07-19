using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXController : MonoBehaviour
{
    [SerializeField] private VisualEffect QTEspam;
    [SerializeField] private VisualEffect QTEtiming;
    [SerializeField] private VisualEffect HexBreak;
    [SerializeField] private VisualEffect QTEburst;
    //  [SerializeField] private VisualEffect QTEarray;
    //  [SerializeField] private VisualEffect QTEstandard;
    // [SerializeField] private VisualEffect QTEfocus;
    //[SerializeField] private VisualEffect QTEcharging;

    [SerializeField] private GameObject BurstButton;
    [SerializeField] private GameObject EBS1;
    [SerializeField] private GameObject EBS2;
    [SerializeField] private GameObject EBS3;

    public void Start()
    {
        
    }

    public void PlayParticleBurst()
    {
        VisualEffect particle = Instantiate(QTEburst, BurstButton.transform.position, BurstButton.transform.rotation);
        particle.Play();

    }

    public void PlayParticleMash()
        {
        // VisualEffect particle = Instantiate(QTEspam, EnemyBattleStation1.transform.position, EnemyBattleStation1.transform.rotation);
        //particle.Play();
      
        }

    public void PlayParticleTimed()
    {
        // QTEtiming
    }

    public void PlayParticleArray()
    {
        // QTEarray
    }

    public void PlayParticleStandard()
    {

    }

    public void KillParticles()
    {
        
    }

}