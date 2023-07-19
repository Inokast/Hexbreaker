using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Specialized;

public class OverworldManager : MonoBehaviour, IDataPersistence
{

    [Header("Overworld UI")]
    [SerializeField] private GameObject informationPanel;
    [SerializeField] private TextMeshProUGUI informationText;
    [SerializeField] private TextMeshProUGUI eventText;
    [SerializeField] private MapNode[] mapNodes;
    [SerializeField] private BattleHUD playerHUD;
    [SerializeField] private GameObject mainMenuButton;

    [SerializeField] private GameObject enterButton;

    private SoundFXController sfx;

    private CreateTalismans tg;

    [Header("World Data")]
    [SerializeField] private MapNode[] parallelNodes;
    [SerializeField] private GameObject player;
    [HideInInspector] public Unit playerUnit;
    private bool worldGenerated;
    private bool combatFinished;

    [SerializeField] private Vector3 startPos = new Vector3(-.4f, .55f, -12f);
    [SerializeField] private Vector3 playerPosInWorld;

    [Header("Combat Node Data")]
    private List<int> lastSelectedNodeEnemyID;
    private string lastSelectedNodeID;

    private bool playerDied;

    private LevelManager levelManager;


    private MapNode selectedNode;
    private Vector3 endPos;
    private float moveTimer = 0;

    private bool movementInitiated = false;

    // Start is called before the first frame update
    void Awake()
    {
        tg = GameObject.Find("TalismanGenerator").GetComponent<CreateTalismans>();
    }

    void Start()
    {
        // If there is no previous world data, GenerateNewWorld()
        sfx = FindAnyObjectByType<SoundFXController>();
        levelManager = FindAnyObjectByType<LevelManager>();
        eventText.text = "";
        
        playerHUD = FindAnyObjectByType<BattleHUD>();
        

        if (playerDied == true)
        {
            //ResetPlayerStats(); We don't want it to reset stats. -Dylan 12

            playerUnit.currentHP = playerUnit.maxHP;

            for (int i = 0; i < tg.action.Count; i++)
            {
                if (tg.action[i])
                {
                    Destroy(tg.talismans[i]);

                    tg.talismans.RemoveAt(i);

                    tg.action.RemoveAt(i);

                    tg.talismanNames.RemoveAt(i);

                    tg.talismanFirstStats.RemoveAt(i);

                    tg.talismanSecondStats.RemoveAt(i);

                    tg.talismanRarities.RemoveAt(i);
                }
            }

            worldGenerated = false;

            playerPosInWorld = startPos;

            endPos = startPos;
            player.transform.position = playerPosInWorld;
            //selectedNode = mapNodes[0];

            StopAllCoroutines();
            EventTextDisplay("The spirits weakened you. May your strength in this life carry to the next.");
        }

        playerDied = false;

        if (worldGenerated == false)
        {
            GenerateNewWorld();
        }
      

        tg = GameObject.Find("TalismanGenerator").GetComponent<CreateTalismans>();

        if (combatFinished == true)
        {
            //print(lastSelectedNodeID);
            CheckNodeCompleted();
        }
        combatFinished = false;

        if (playerPosInWorld != startPos)
        {
            player.transform.position = playerPosInWorld;
        }

        else
        {
            player.transform.position = startPos;
        }

        playerHUD.SetHUD(playerUnit);
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
        endPos = playerPosInWorld;

        combatFinished = gameData.combatFinished;
        worldGenerated = gameData.worldGenerated;

        playerDied = gameData.playerDied;

        lastSelectedNodeID = gameData.lastSelectedNodeID;

        //print("Combat finished state has loaded: " + combatFinished);
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
        gameData.combatFinished = combatFinished;

        gameData.worldGenerated = worldGenerated;

        gameData.lastSelectedNodeID = lastSelectedNodeID;

        gameData.playerDied = playerDied;

        //print(gameData.lastSelectedNodeID);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && GameObject.Find("TalismanPanel") == null)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.collider.CompareTag("Node"))
                {
                    

                    selectedNode = hit.collider.gameObject.GetComponent<MapNode>();

                    if (selectedNode.isActive)
                    {
                        if (lastSelectedNodeID != null) 
                        {
                            foreach (MapNode node in mapNodes)
                            {
                                if (lastSelectedNodeID == node.nodeID)
                                {
                                    parallelNodes = node.GetParallelNodes();
                                }
                            }

                            foreach (MapNode node in parallelNodes)
                            {
                                if (selectedNode == node)
                                {
                                    return;
                                }
                            }
                        }

                        
                        DisplayNodeInfo(selectedNode);
                        endPos = new Vector3(selectedNode.gameObject.transform.position.x, selectedNode.gameObject.transform.position.y, selectedNode.gameObject.transform.position.z - 1);
                        moveTimer = 0;
                        movementInitiated = true;
                        lastSelectedNodeID = selectedNode.nodeID;
                        

                        sfx.PlayButtonSelect();
                        //informationPanel.GetComponentInChildren<Button>().interactable = true;

                        if (selectedNode.isCompleted)
                        {
                            //informationPanel.GetComponentInChildren<Button>().interactable = false;
                            StartCoroutine(EventTextDisplay("You've already cleared this path."));
                            informationPanel.SetActive(false);
                            enterButton.SetActive(false);
                        }

                        else if (selectedNode.isCompleted == false)
                        {
                            //informationPanel.GetComponentInChildren<Button>().interactable = true;
                            StartCoroutine(EventTextDisplay("Traverse this path?..."));                           
                        }
                    }

                    else
                    {
                        //print("Selected node is not activated");
                        StartCoroutine(EventTextDisplay("The fog is blocking this path..."));
                        informationPanel.SetActive(false);
                        enterButton.SetActive(false);
                    }
                }

                else if (hit.collider.CompareTag("EventButton") && !selectedNode.isCompleted && selectedNode.isActive)
                {
                    OnConfirm();
                }

                else
                {
                    informationPanel.SetActive(false);
                    enterButton.SetActive(false);
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

                if (selectedNode.isActive && !selectedNode.isCompleted) 
                {
                    enterButton.SetActive(true);
                }
                
            }
        }
    }

    public void OnConfirm()
    {
        //print("Confirmed");

        switch (selectedNode.nodeType)
        {
            case 1:
                // Node type Combat

                lastSelectedNodeEnemyID = selectedNode.nodeEnemyID;

                if (selectedNode.mapSection == "Tutorial")
                {
                    sfx.PlayButtonSelect();
                    levelManager.LoadSceneWithName("TutorialScene");
                }

                if (selectedNode.mapSection == "Marsh")
                {
                    sfx.PlayButtonSelect();
                    levelManager.LoadSceneWithName("CombatSceneMarsh");
                }

                if (selectedNode.mapSection == "Ruins")
                {
                    sfx.PlayButtonSelect();
                    levelManager.LoadSceneWithName("CombatSceneRuins");
                }

                if (selectedNode.mapSection == "Cathedral")
                {
                    sfx.PlayButtonSelect();
                    levelManager.LoadSceneWithName("CombatSceneCathedral");
                }

                if (selectedNode.mapSection == "Boss")
                {
                    sfx.PlayButtonSelect();
                    levelManager.LoadSceneWithName("CombatSceneBoss");
                }

                break;

            case 2:
                sfx.PlayButtonSelect();
                tg.GenerateHealingTalismans();
                break;

            case 3:
                sfx.PlayLuckyNode();
                tg.GenerateLuckyTalismans();
                break;

            case 4:

                // Node type Upgrade
                break;

            default:
                print("Node type not recognized");
                break;
        }
    }

    public void MarkCurrentNodeComplete()
    {
        selectedNode.CompleteNode();

        mainMenuButton.SetActive(true);

        tg.HidePanel();

        selectedNode.ClearFog();

        playerHUD.SetHUD(playerUnit);
    }

    IEnumerator EventTextDisplay(string textToDisplay)
    {

        eventText.text = textToDisplay;
        yield return new WaitForSeconds(5f);

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
                //print("Node " + node.name + " completed");
            }
        }

        playerHUD.SetHUD(playerUnit);
    }

    private void DisplayNodeInfo(MapNode node)
    {
        sfx.PlayButtonSelect();
        informationPanel.SetActive(true);
        informationText.text = node.nodeDescription;
    }

    // Please use this function to initialize the world. Use this for any randomization that sets what each node is. - Dan
    private void GenerateNewWorld()
    {
        worldGenerated = true;
        player.transform.position = startPos;
        mapNodes[0].isActive = true;

        playerPosInWorld = startPos;

        foreach (MapNode node in mapNodes) //For on-death resets. -Dylan 12
        {
            Destroy(node.newMesh);

            node.isActive = false;

            node.isCompleted = false;
        }
        //Map rando in place is below, also randomizes enemies and amount of enemies. -Dylan 5
        foreach (MapNode node in mapNodes)
        {

            if (node.nodeDescription != "This is where your journey begins...")
            {
                int randomizedNumber = Random.Range(1, 101);

                if (randomizedNumber >= 1 && randomizedNumber < 55)
                {
                    node.nodeType = 1;

                    node.nodeDescription = "A dangerous combat encounter lurks here...";
                }
                else if (randomizedNumber >= 55 && randomizedNumber < 75)
                {
                    node.nodeType = 2;

                    node.nodeDescription = "A clean fountain that relieves the visitor of their injuries is found here...";
                }
                else if (randomizedNumber >= 75)
                {
                    node.nodeType = 3;

                    node.nodeDescription = "A treasure horde of shiny talismans is found here...";
                }

                int amountOfEnemies = Random.Range(1, 4);

                node.nodeEnemyID.Clear();

                do
                {
                    randomizedNumber = Random.Range(1, 101);

                    if (node.mapSection == "Marsh")
                    {
                        if (randomizedNumber >= 1 && randomizedNumber < 35)
                        {
                            node.nodeEnemyID.Add(0);
                        }
                        else if (randomizedNumber >= 36 && randomizedNumber < 70)
                        {
                            node.nodeEnemyID.Add(1);
                        }
                        else if (randomizedNumber >= 71)
                        {
                            node.nodeEnemyID.Add(2);
                        }
                    }
                    else if (node.mapSection == "Ruins")
                    {
                        if (randomizedNumber >= 1 && randomizedNumber < 35)
                        {
                            node.nodeEnemyID.Add(0);
                        }
                        else if (randomizedNumber >= 36 && randomizedNumber < 70)
                        {
                            node.nodeEnemyID.Add(1);
                        }
                        else if (randomizedNumber >= 71)
                        {
                            node.nodeEnemyID.Add(2);
                        }
                    }
                    else if (node.mapSection == "Cathedral")
                    {
                        if (randomizedNumber >= 1) //Only one enemy resides in the cathedral currently. -Dylan 5
                        {
                            node.nodeEnemyID.Add(0);
                        }
                    }

                    amountOfEnemies--;

                } while (amountOfEnemies > 0);
            }
        }

        mapNodes[0].nodeType = 1;
        mapNodes[0].mapSection = "Tutorial";
        mapNodes[0].nodeEnemyID.Clear();
        mapNodes[0].nodeEnemyID.Add(0);
        mapNodes[0].nodeDescription = "Your journey begins here...";

        mapNodes[0].isActive = true;

        RandomizeSpecificNode(6, 1, Random.Range(1, 4), "Marsh");

        RandomizeSpecificNode(7, 1, Random.Range(1, 4), "Marsh");

        RandomizeSpecificNode(8, 1, Random.Range(1, 4), "Marsh");

        RandomizeSpecificNode(9, 1, Random.Range(1, 4), "Marsh");

        RandomizeSpecificNode(17, 1, Random.Range(1, 4), "Cathedral");

        RandomizeSpecificNode(18, 1, Random.Range(1, 4), "Cathedral");

        RandomizeSpecificNode(19, 1, Random.Range(1, 4), "Cathedral");

        foreach (MapNode node in mapNodes)
        {
            node.UpdateMesh();
            if (node.gameObject.name == "BossNode") 
            {
                node.nodeType = 1;
                node.mapSection = "Boss";
                node.nodeEnemyID.Clear();
                node.nodeEnemyID.Add(0);
                node.nodeDescription = "You've reached the Phantasmal Sanctum. The air here is rife with melancholy...";
            }
        }
    }

    // Resets the player unit to default values - Dan
    private void ResetPlayerStats()
    {
        playerUnit.highDamage = 6;
        playerUnit.midDamage = 4;
        playerUnit.lowDamage = 2;
        playerUnit.defense = 0;
        playerUnit.maxHP = 70;
        playerUnit.currentHP = 70;

        playerHUD.SetHP(playerUnit);

        playerPosInWorld = startPos;


    }

    private void RandomizeSpecificNode(int nodeIndex, int nodeType, int enemyAmount, string nodeRegion)
    {
        mapNodes[nodeIndex].nodeEnemyID.Clear();
        //Just a quicker way to specify the row of nodes we want to always be combat. -Dylan 10
        do
        {

            if (nodeRegion == "Marsh")
            {
                mapNodes[nodeIndex].nodeType = nodeType;
                mapNodes[nodeIndex].nodeDescription = "A dangerous combat encounter lurks here...";

                int randomizedNumber = Random.Range(1, 101);

                if (randomizedNumber >= 1 && randomizedNumber < 35)
                {
                    mapNodes[nodeIndex].nodeEnemyID.Add(0);
                }
                else if (randomizedNumber >= 36 && randomizedNumber < 70)
                {
                    mapNodes[nodeIndex].nodeEnemyID.Add(1);
                }
                else if (randomizedNumber >= 71)
                {
                    mapNodes[nodeIndex].nodeEnemyID.Add(2);
                }
            }
            else if (nodeRegion == "Ruins")
            {
                mapNodes[nodeIndex].nodeType = nodeType;
                mapNodes[nodeIndex].nodeDescription = "A dangerous combat encounter lurks here...";

                int randomizedNumber = Random.Range(1, 101);


                if (randomizedNumber >= 1 && randomizedNumber < 35)
                {
                    mapNodes[nodeIndex].nodeEnemyID.Add(0);
                }
                else if (randomizedNumber >= 36 && randomizedNumber < 70)
                {
                    mapNodes[nodeIndex].nodeEnemyID.Add(1);
                }
                else if (randomizedNumber >= 71)
                {
                    mapNodes[nodeIndex].nodeEnemyID.Add(2);
                }
            }
            else if (nodeRegion == "Cathedral")
            {
                mapNodes[nodeIndex].nodeType = nodeType;
                mapNodes[nodeIndex].nodeDescription = "A dangerous combat encounter lurks here...";

                int randomizedNumber = Random.Range(1, 101);

                if (randomizedNumber >= 1) //Only one enemy resides in the cathedral currently. -Dylan 5
                {
                    mapNodes[nodeIndex].nodeEnemyID.Add(0);
                }
            }

            enemyAmount--;

        } while (enemyAmount > 0);
    }
}
