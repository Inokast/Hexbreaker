using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode : MonoBehaviour , IDataPersistence
{
    public string nodeID; // This will be relevant once we need to save the data of individual nodes upon randomization. - Dan

    [ContextMenu("Generate guid for id")]

    private void GenerateGuid() 
    {
        nodeID = System.Guid.NewGuid().ToString();
    }


    [Header("Pathing")]
    [SerializeField] private MapNode leftPath;
    [SerializeField] private MapNode rightPath;
    private bool rightPathLocked;
    private bool leftPathLocked;



    public int nodeType;
    public string nodeDescription;
    public string[] nodeRewards; // Replace String[] with Talisman[] or follow the idea as needed to be able to track it across scenes. - Dan
    public int nodeEnemyID;

    public string mapSection; // Marsh, Ruins, Cathedral, Finalboss.

    public bool isActive;
    public bool isCompleted;

    public void LoadData(GameData gameData) 
    {
        gameData.nodesActive.TryGetValue(nodeID, out isActive);
        gameData.nodesActive.TryGetValue(nodeID, out isCompleted);
    }

    public void SaveData(GameData gameData)
    {
        if (gameData.nodesActive.ContainsKey(nodeID)) 
        {
            gameData.nodesActive.Remove(nodeID);
        }
        gameData.nodesActive.Add(nodeID, isActive);

        if (gameData.nodesCompleted.ContainsKey(nodeID))
        {
            gameData.nodesCompleted.Remove(nodeID);
        }
        gameData.nodesCompleted.Add(nodeID, isCompleted);
    }

    public void CompleteNode() 
    {
        isCompleted = true;

        if (rightPath == leftPath)
        {
            rightPath.isActive = true;
        }

        else 
        {
            if (rightPathLocked == false) 
            {
                rightPath.isActive = true;
            }

            if (leftPathLocked == false) 
            {
                leftPath.isActive = true;
            }
        }

        rightPathLocked = false;
        leftPathLocked = false;
    }

}
