using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillTimer : MonoBehaviour
{
    [SerializeField] private float timeUntilKill;

    private void Start()
    {
        StartCoroutine(Kill());
    }

    IEnumerator Kill() 
    {
        yield return new WaitForSeconds(timeUntilKill);
        Destroy(gameObject);
    }
}
