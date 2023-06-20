using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public List<int> nodeEnemyID;

    public string mapSection; // Marsh, Ruins, Cathedral, Finalboss.

    public bool isActive;
    public bool isCompleted;

    public void LoadData(GameData gameData) //I've definitely done something wrong here, there's no way it's right. -Dylan 5
    {
        gameData.nodesActive.TryGetValue(nodeID, out isActive);
        gameData.nodesCompleted.TryGetValue(nodeID, out isCompleted);
        gameData.nodesDescriptions.TryGetValue(nodeID, out nodeDescription);
        gameData.nodesTypes.TryGetValue(nodeID, out nodeType);
        if (nodeType == 1)
        {
            string enemiesAsString = string.Join(",", nodeEnemyID);
            gameData.nodesEnemies.TryGetValue(nodeID, out enemiesAsString);
            nodeEnemyID = enemiesAsString.Split(",").Select(Int32.Parse).ToList();
        }
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

        if (gameData.nodesDescriptions.ContainsKey(nodeID))
        {
            gameData.nodesDescriptions.Remove(nodeID);
        }
        gameData.nodesDescriptions.Add(nodeID, nodeDescription);

        if (gameData.nodesTypes.ContainsKey(nodeID))
        {
            gameData.nodesTypes.Remove(nodeID);
        }
        gameData.nodesTypes.Add(nodeID, nodeType);

        string enemiesAsString = string.Join(",", nodeEnemyID);

        if (gameData.nodesEnemies.ContainsKey(nodeID))
        {
            gameData.nodesEnemies.Remove(nodeID);
        }
        gameData.nodesEnemies.Add(nodeID, enemiesAsString);
    }

    public void CompleteNode() 
    {
        isCompleted = true;

        if (mapSection == "Boss") 
        {
            FindObjectOfType<LevelManager>().LoadSceneWithName("EndScene");
            return;
        }

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
