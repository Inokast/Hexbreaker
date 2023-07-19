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
    
    public void PlayParticleBurst(Transform spawnPoint)
    {
        VisualEffect particle = Instantiate(QTEburst, BurstButton.transform.position, BurstButton.transform.rotation);
        particle.Play();

    }

    public void PlayParticleMash(Transform spawnPoint)
    {
        VisualEffect particle = Instantiate(QTEspam, spawnPoint.position, spawnPoint.rotation);
        //particle.Play();
      
    }

    public void PlayParticleTimed(Transform spawnPoint)
    {
        // QTEtiming
    }

    public void PlayParticleArray(Transform spawnPoint)
    {
        // QTEarray
    }

    public void PlayParticleStandard(Transform spawnPoint)
    {

    }

    public void PlayParticleHexbreak(Transform startPos, Transform endPos) 
    {

    }

    public void PlayParticleEnemyCharge(Transform spawnPoint) 
    {

    }


}