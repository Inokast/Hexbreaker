using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurifyTalismansButton : MonoBehaviour
{
    private CreateTalismans talismanManager;

    public void PurifyTalismans()
    {
        talismanManager = GameObject.Find("TalismanGenerator").GetComponent<CreateTalismans>();

        talismanManager.GenerateEncounterTalismans();
    }
}
