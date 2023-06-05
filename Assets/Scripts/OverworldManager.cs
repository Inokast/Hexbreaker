using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OverworldManager : MonoBehaviour, IDataPersistence
{

    [Header("Overworld UI")]
    [SerializeField] private GameObject informationPanel;
    [SerializeField] private TextMeshProUGUI informationText;
    [SerializeField] private TextMeshProUGUI eventText;
    [SerializeField] private MapNode[] mapNodes;


    [Header("World Data")]
    [SerializeField] private GameObject player;
    private Unit playerUnit;
    private bool worldGenerated;
    private bool combatFinished;

    [SerializeField] private Vector3 startPos = new Vector3(0, .5f, -8);
    [SerializeField] private Vector3 playerPosInWorld;

    [Header("Combat Node Data")]
    [SerializeField] private GameObject[] EnemyPrefabs; // We put all possible enemy prefabs here. When we want to load a combat scene, we change
    [SerializeField] private int lastSelectedNodeEnemyID;     
    private string lastSelectedNodeID;

    private LevelManager levelManager;

    private MapNode selectedNode;
    private Vector3 endPos;
    private float moveTimer = 0;

    private bool movementInitiated = false;

    // Start is called before the first frame update
    void Start()
    {
        // If there is no previous world data, GenerateNewWorld()

        levelManager = FindAnyObjectByType<LevelManager>();
        eventText.text = "";
        player.transform.position = playerPosInWorld;

        if (worldGenerated == false) 
        {
            GenerateNewWorld();
        }

        if (playerUnit.currentHP == 0) 
        {
            GenerateNewWorld();
            ResetPlayerStats();
        }

        if (combatFinished == true) 
        {
            CheckNodeCompleted();
        }

    }
    public void LoadData(GameData gameData)
    {
        playerUnit = player.GetComponent<Unit>();
        playerUnit.unitName = gameData.playerUnitName;
        playerUnit.highDamage = gameData.playerHighDamage;
        playerUnit.midDamage = gameData.playerMidDamage;
        playerUnit.lowDamage = gameData.playerLowDamage;
        playerUnit.defense = gameData.playerDefense;
        playerUnit.maxHP = gameData.playerMaxHP;
        playerUnit.currentHP = gameData.playerCurrentHP;
        worldGenerated = gameData.worldGenerated;

        playerPosInWorld = gameData.playerPositionInWorld;

        combatFinished = gameData.combatFinished;
        worldGenerated = gameData.worldGenerated;
    }

    public void SaveData(GameData gameData)
    {
        gameData.enemyUnitToLoadID = lastSelectedNodeEnemyID;

        gameData.playerUnitName = playerUnit.unitName;
        gameData.playerHighDamage = playerUnit.highDamage;
        gameData.playerMidDamage = playerUnit.midDamage;
        gameData.playerLowDamage = playerUnit.lowDamage;
        gameData.playerDefense = playerUnit.defense;
        gameData.playerMaxHP = playerUnit.maxHP;
        gameData.playerCurrentHP = playerUnit.currentHP;

        gameData.playerPositionInWorld = endPos;

        gameData.worldGenerated = worldGenerated;

        gameData.lastSelectedNodeID = lastSelectedNodeID;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.collider.CompareTag("Node"))
                {
                    print("Hit a node");
                    selectedNode = hit.collider.gameObject.GetComponent<MapNode>();


                    if (selectedNode.isActive)
                    {
                        DisplayNodeInfo(selectedNode);
                        endPos = selectedNode.gameObject.transform.position;
                        moveTimer = 0;
                        movementInitiated = true;
                        lastSelectedNodeID = selectedNode.nodeID;

                        if (selectedNode.isCompleted)
                        {
                            informationPanel.GetComponentInChildren<Button>().interactable = false;
                        }

                        else 
                        {                            
                            informationPanel.GetComponentInChildren<Button>().interactable = true;
                        }                       
                    }
                }

                else
                {
                    informationPanel.SetActive(false);
                }
            }
        }

        if (movementInitiated == true)
        {
            player.transform.position = Vector3.Lerp(player.transform.position, endPos, moveTimer / 8);
            moveTimer += Time.deltaTime;

            if (player.transform.position == endPos)
            {
                movementInitiated = false;
            }
        }
    }

    public void OnConfirm()
    {
        print("Confirmed");

        switch (selectedNode.nodeType)
        {
            case 1:
                // Node type Combat
                
                lastSelectedNodeEnemyID = selectedNode.nodeEnemyID;

                if (selectedNode.mapSection == "Marsh") 
                {
                    levelManager.LoadSceneWithName("CombatSceneForest");
                }
                
                break;

            case 2:
                // Node type Lucky (free talisman choice)
                break;

            case 3:
                // Node type Heal
                break;

            case 4:
                
                // Node type Upgrade
                break;

            default:
                print("Node type not recognized");
                break;
        }
    }

    IEnumerator EventTextDisplay(string textToDisplay) 
    {
        eventText.text = textToDisplay;
        yield return new WaitForSeconds(3f);

        eventText.text = "";
    }

    private void CheckNodeCompleted() 
    {
        foreach (MapNode node in mapNodes)
        {
            if (node.nodeID == lastSelectedNodeID) 
            {
                node.CompleteNode();
                combatFinished = false;
            }
        }
    }

    private void DisplayNodeInfo(MapNode node) 
    {
        informationPanel.SetActive(true);
        informationText.text = node.nodeDescription;
    }

    // Please use this function to initialize the world. Use this for any randomization that sets what each node is. - Dan
    private void GenerateNewWorld() 
    {
        worldGenerated = true;
        player.transform.position = startPos;
        mapNodes[0].isActive = true;

        // foreach mapNode node in mapNodes[], randomize values
    }

     // Resets the player unit to default values - Dan
    private void ResetPlayerStats() 
    {
        playerUnit.highDamage = 6;
        playerUnit.midDamage = 4;
        playerUnit.lowDamage = 2;
        playerUnit.defense = 0;
        playerUnit.maxHP = 80;
        playerUnit.currentHP = 80;

        // Also remove any talismans
    }
}
