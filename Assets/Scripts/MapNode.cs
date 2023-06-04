using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode : MonoBehaviour
{
    public string nodeID; // This will be relevant once we need to save the data of individual nodes upon randomization. - Dan
    public int nodeType;
    public string nodeDescription;
    public string[] nodeRewards; // Replace String[] with Talisman[] or follow the idea as needed to be able to track it across scenes. - Dan
    public int nodeEnemyID;

    public bool isActive;
    public bool isCompleted;

    // Start is called before the first frame update
    
}
