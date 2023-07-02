using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TrackSwitcher : MonoBehaviour
{
    private Cinemachine.CinemachineDollyCart cart;

    public Cinemachine.CinemachineSmoothPath startPath;
    public Cinemachine.CinemachineSmoothPath[] alternatePaths;

    private void Awake()
    {
        cart = GetComponent<Cinemachine.CinemachineDollyCart>();
    }

    private void Reset()
    {
        StopAllCoroutines();
        cart.m_Path = startPath;
        StartCoroutine(ChangeTrack());
    }

    IEnumerator ChangeTrack() 
    {
        yield return new WaitForSeconds(Random.Range(4,6));

        var path = alternatePaths[Random.Range(0, alternatePaths.Length)];
        cart.m_Path = path;

        StartCoroutine(ChangeTrack());
    }
}
