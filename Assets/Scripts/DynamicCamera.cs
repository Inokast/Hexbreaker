using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCamera : MonoBehaviour
{
    [SerializeField] private GameObject mainCam;

    [SerializeField] private GameObject[] trackCams;

    [SerializeField] private GameObject enemyCam;

    //private float targetHeightOffset = 1.5f;

    public Transform enemyTarget;

    //[SerializeField] private TrackSwitcher playerSwitch;
    //[SerializeField] private TrackSwitcher enemySwitch;

    public void Start()
    {
        ResetMainCam();
    }

    IEnumerator ChangeTrackCam() 
    {
        yield return new WaitForSeconds(Random.Range(10, 16));

        int newInt = Random.Range(0, trackCams.Length);
        GameObject newCam = trackCams[newInt];
        StopAllCams();
        newCam.SetActive(true);

        StartCoroutine(ChangeTrackCam());
    }

    public void ResetMainCam() 
    {
        StopAllCoroutines();
        StopAllCams();
        mainCam.SetActive(true);

        StartCoroutine(ChangeTrackCam());
    }

    public void ResetEnemyCam()
    {
        StopAllCoroutines();
        StopAllCams();
        enemyCam.gameObject.SetActive(true);
    }


    private void StopAllCams() 
    {
        mainCam.SetActive(false);
        enemyCam.SetActive(false);

        foreach (GameObject cam in trackCams)
        {
            cam.SetActive(false);
        }
    }
}
