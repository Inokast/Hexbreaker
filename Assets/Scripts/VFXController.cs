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
    [SerializeField] private VisualEffect QTEarray;
    [SerializeField] private VisualEffect QTEstandard;
    [SerializeField] private VisualEffect QTEfocus;
    [SerializeField] private VisualEffect QTEcharging;

    [SerializeField] private GameObject BurstButton;
    
    public void PlayParticleBurst(Transform spawnPoint)
    {
        VisualEffect particle = Instantiate(QTEburst,new Vector3 (spawnPoint.transform.position.x, spawnPoint.transform.position.y, spawnPoint.position.z +4), spawnPoint.transform.rotation);
       // VisualEffect particle = Instantiate(QTEburst);
        //QTEburst.transform.rotation = spawnPoint.transform.rotation;
        QTEburst.transform.position = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, spawnPoint.transform.position.z + 4);
        particle.Play();

    }

    public void PlayParticleMash(Transform spawnPoint)
    {
        VisualEffect particle = Instantiate(QTEspam, spawnPoint.position, spawnPoint.rotation);
        particle.Play();
      
    }

    public void PlayParticleTimed(Transform spawnPoint)
    {
        VisualEffect particle = Instantiate(QTEtiming, spawnPoint.position, spawnPoint.rotation);
        particle.Play();
    }

    public void PlayParticleArray(Transform spawnPoint)
    {
        VisualEffect particle = Instantiate(QTEarray, spawnPoint.position, spawnPoint.rotation);
        particle.Play();
    }

    public void PlayParticleStandard(Transform spawnPoint)
    {
        VisualEffect particle = Instantiate(QTEstandard, spawnPoint.position, spawnPoint.rotation);
        particle.Play();
    }

    public void PlayParticleHexbreak(Transform startPos, Transform endPos) 
    {
        VisualEffect particle = Instantiate(HexBreak, startPos.position, endPos.rotation);
        particle.Play();
    }

    public void PlayParticleEnemyCharge(Transform spawnPoint) 
    {
        VisualEffect particle = Instantiate(QTEcharging, spawnPoint.position, spawnPoint.rotation);
        particle.Play();
    }


}