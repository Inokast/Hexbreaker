using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapNode : MonoBehaviour, IDataPersistence
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

    [SerializeField] private GameObject fogEffect;
    [SerializeField] private GameObject[] nodeMeshes;
    private GameObject newMesh;

    public int nodeType;
    public string nodeDescription;
    public string[] nodeRewards; // Replace String[] with Talisman[] or follow the idea as needed to be able to track it across scenes. - Dan
    public List<int> nodeEnemyID;

    public string mapSection; // Marsh, Ruins, Cathedral, Finalboss.

    public bool isActive;
    public bool isCompleted;

    private void Start()
    {
        InitializeNode();
    }

    public void InitializeNode() 
    {
        UpdateMesh();
        if (isActive == false)
        {
            fogEffect.SetActive(true);
        }

        else
        {
            ClearFog();
        }
    }

    public void ClearFog() 
    {
        fogEffect.SetActive(false);
    }

    public void UpdateMesh() 
    {
        switch (nodeType)
        {
            case 1:
                newMesh = Instantiate(nodeMeshes[0], new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                newMesh.transform.parent = gameObject.transform;
                break;

            case 2:
                newMesh = Instantiate(nodeMeshes[1], new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                newMesh.transform.parent = gameObject.transform;
                break;

            case 3:
                newMesh = Instantiate(nodeMeshes[2], new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                newMesh.transform.parent = gameObject.transform;
                break;

            default:
                break;
        }
    }

    public void LoadData(GameData gameData) //I've definitely done something wrong here, there's no way it's right. -Dylan 5
    {
        gameData.nodesActive.TryGetValue(nodeID, out isActive);
        gameData.nodesCompleted.TryGetValue(nodeID, out isCompleted);
        gameData.nodesDescriptions.TryGetValue(nodeID, out nodeDescription);
        gameData.nodesTypes.TryGetValue(nodeID, out nodeType);
        for (int i = 0; i < gameData.nodesEnemies.Count; i++)
        {
            if (gameData.nodeIDForEnemies[i] == nodeID)
            {
                nodeEnemyID.Clear();
                foreach (char c in gameData.nodesEnemies[i])
                {
                    nodeEnemyID.Add(int.Parse(c.ToString()));
                }
            }
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

        string enemiesAsString = "";

        if (gameData.nodeIDForEnemies.Contains(nodeID))
        {
            for (int i = 0; i < gameData.nodeIDForEnemies.Count; i++)
            {
                if (gameData.nodeIDForEnemies[i] == nodeID)
                {
                    gameData.nodeIDForEnemies.RemoveAt(i);

                    gameData.nodesEnemies.RemoveAt(i);
                }
            }
        }
        
        foreach (int i in nodeEnemyID)
        {
            enemiesAsString += i.ToString();
        }


        gameData.nodeIDForEnemies.Add(nodeID);

        gameData.nodesEnemies.Add(enemiesAsString);
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
            rightPath.ClearFog();
        }

        else 
        {
            if (rightPathLocked == false)
            {
                rightPath.isActive = true;
                rightPath.ClearFog();
            }

            if (leftPathLocked == false)
            {
                leftPath.isActive = true;
                leftPath.ClearFog();
            }
        }

        rightPathLocked = false;
        leftPathLocked = false;
    }

}
