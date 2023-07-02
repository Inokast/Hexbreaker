using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCamera : MonoBehaviour
{
    public GameObject playerCam;
    public GameObject enemyCam;

    public TrackSwitcher playerSwitch;
    public TrackSwitcher enemySwitch;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartPlayerCam() 
    {
        playerCam.gameObject.SetActive(true);
        enemyCam.gameObject.SetActive(false);
    }

    public void StartEnemyCam()
    {
        playerCam.gameObject.SetActive(false);
        enemyCam.gameObject.SetActive(true);
    }
}
