using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.UI;

public class MouseController : MonoBehaviour
{
    public float speed;

    public GameObject characterPrefab;
    public CharacterInfo character;

    private PathFinder pathFinder;
    private RangeFinder rangeFinder;
    private List<OverlayTile> path = new List<OverlayTile>();
    public List<OverlayTile> inRangeTiles = new List<OverlayTile>();

    MapManager mapManager;
    bool phaseTwo = false;
    public bool isMovingUnit;
    bool isChoosingUnit = true;
    [HideInInspector] public bool isAttackingUnit;
    public bool isMagic;
    public bool isMagicProperty
    {
        get { return isMagic; } set { isMagic = value; }
    }
    private OverlayTile targetUnitTile;

    [Header("Gameplay UI Gameobjects")]
    public GameObject unitData;
    public GameObject enemyData;
    public GameObject playerActions;
    public GameObject confirmAttack;
    public GameObject winScreen;
    public GameObject loseScreen;

    [SerializeField] private List<CharacterInfo> myUnits;

    private void Awake()
    {
        RegisterEvents();
        //UnregisterEvents();
    }

    // Start is called before the first frame update
    void Start()
    {
        pathFinder = new PathFinder();
        rangeFinder = new RangeFinder();
        mapManager = MapManager.Instance;

    }
        

    // Update is called once per frame
    void LateUpdate()
    {
        var focusedTileHit = GetFocusedOnTile();

        // Check if raycast has a tile
        if (focusedTileHit.HasValue)
        {
            // Gets tile that has an overlay tile (That means that the tile is accessible)
            OverlayTile overlayTile = focusedTileHit.Value.collider.gameObject.GetComponent<OverlayTile>();
            transform.position = overlayTile.transform.position;

            gameObject.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder + 1;

            // Highlights the tile when clicked
            if (Input.GetMouseButtonDown(0))
            {
                // If theres a unit then find walk range
                if (overlayTile.unitOnTile && !overlayTile.unitOnTile.isPlayed && isChoosingUnit && !isAttackingUnit && !phaseTwo) // <-- Gets unit info when choosing units
                {
                    if ((overlayTile.unitOnTile.team == 0 && mapManager.isPlayerOneTurn && mapManager.currentTeam == 0) || 
                        (overlayTile.unitOnTile.team == 1 && !mapManager.isPlayerOneTurn && mapManager.currentTeam == 1) &&
                        !overlayTile.unitOnTile.isDead)
                    {
                        character = overlayTile.unitOnTile;
                        PositionCharacterOnTile(character, overlayTile);
                        GetUnitInfoAndShowUI();
                        //GetInRangeTiles();
                    }
                } else if ((character && overlayTile.isAccesible) && isMovingUnit) // <-- Gets target tile when moving
                {
                    character.activeTile.unitOnTile = null;

                    // find path to the clicked tile
                    FindPath(character.activeTile.grid2DLocation.x, character.activeTile.grid2DLocation.y, overlayTile.grid2DLocation.x, overlayTile.grid2DLocation.y);

                    // NET Implementation
                    NetMakeMove mm = new NetMakeMove();
                    mm.originalX = character.activeTile.grid2DLocation.x;
                    mm.originalY = character.activeTile.grid2DLocation.y;
                    mm.targetX = overlayTile.grid2DLocation.x;
                    mm.targetY = overlayTile.grid2DLocation.y;
                    mm.teamId = mapManager.currentTeam;
                    Client.Instance.SendToServer(mm);

                    overlayTile.unitOnTile = character;
                    foreach (var item in inRangeTiles)
                    {
                        item.HideTile();
                    }
                } else if (character && isAttackingUnit && !confirmAttack.activeSelf) // <-- Gets target Unit when is attacking
                {
                    if (!overlayTile.isAccesible)
                    {
                        foreach (var item in inRangeTiles)
                        {
                            item.HideTile();
                        }
                        isAttackingUnit = false;
                        playerActions.SetActive(true);
                    }else if (overlayTile.unitOnTile.team != mapManager.currentTeam && overlayTile.isAccesible && !overlayTile.unitOnTile.isDead)
                    {
                        targetUnitTile = overlayTile;
                        confirmAttack.SetActive(true);

                        Debug.Log("Attacked " + overlayTile.unitOnTile.name);
                        foreach (var item in inRangeTiles)
                        {
                            item.HideTile();
                        }

                        GetEnemyData(targetUnitTile.unitOnTile);
                    }
                } else if (!isMovingUnit && !isChoosingUnit && !isAttackingUnit && !phaseTwo)
                {
                    character = null;
                    isMovingUnit = false;
                    isAttackingUnit = false;
                    unitData.SetActive(false);
                    playerActions.SetActive(false);

                    foreach (var item in inRangeTiles)
                    {
                        item.HideTile();
                    }   
                }
            }
        }

        // After clicking a tile, move the character along path
        if (path.Count > 0)
        {
            MoveAlongPath();
        }
    }

    private void GetUnitInfoAndShowUI()
    {
        GameUI.Instance.ShowUnitInfo(character);
        unitData.SetActive(true);
        playerActions.SetActive(true);
        playerActions.transform.Find("Move Button").GetComponent<Button>().interactable = true;

    }

    private void GetEnemyData(CharacterInfo character)
    {
        GameUI.Instance.ShowEnemyInfo(character);
        enemyData.SetActive(true);

    }

    public void GetInRangeTiles()
    {
        foreach (var item in inRangeTiles)
        {
            item.HideTile();
        }

        isChoosingUnit = false;
        isMovingUnit = true;
        inRangeTiles = rangeFinder.GetTilesInRange(character.activeTile, character.characterClass.moveRange, character);

        foreach (var item in inRangeTiles)
        {
            item.ShowTile();
        }
    }
    public void GetInRangeAttack()
    {
        foreach (var item in inRangeTiles)
        {
            item.HideTile();
        }

        isChoosingUnit = true;
        isAttackingUnit = true;

        int attackRange;
        if (isMagic)
            attackRange = character.characterClass.magicRange;
        else
            attackRange = character.characterClass.attackRange;

        inRangeTiles = rangeFinder.GetTilesInRange(character.activeTile, attackRange, character, true);

        foreach (var item in inRangeTiles)
        {
            item.ShowAttackTile();
        }
    }

    public void WaitAction()
    {
        character.isPlayed = true;
        phaseTwo = false;
        character = null;
        isMovingUnit = false;
        isAttackingUnit = false;
        isChoosingUnit = true;

        CheckAllUnits();
    }

    private void CheckAllUnits()
    {
        int i = 0;
        foreach (CharacterInfo unit in myUnits)
        {
            if (unit.isPlayed)
                i++;
        }

        if (i == myUnits.Count())
        {
            Debug.Log("my turn has ended!");
            mapManager.isPlayerOneTurn = !mapManager.isPlayerOneTurn;

            NetSwitchTurns st = new NetSwitchTurns();
            if (mapManager.isPlayerOneTurn)
                st.isPlayerOneTurn = 0;
            else
                st.isPlayerOneTurn = 1;

            Client.Instance.SendToServer(st);

            foreach (CharacterInfo unit in myUnits)
            {
                unit.isPlayed = false;
                unit.hasMoved = false;
            }
        }
    }

    private void CheckAllUnitsHealth()
    {
        int i = 0;
        foreach(CharacterInfo unit in myUnits)
        {
            if (unit.isDead)
                i++;

            if (i == myUnits.Count())
            {
                Debug.Log("I lost!");

                loseScreen.SetActive(true);
                winScreen.SetActive(false);

                NetEndGame eg = new NetEndGame();
                eg.losingTeamId = mapManager.currentTeam;
                Client.Instance.SendToServer(eg);
            }
        }
    }

    private void FindPath(int originalX, int originalY, int targetX, int targetY)
    {
        Vector2Int startLoc = new Vector2Int(originalX, originalY);
        Vector2Int endLoc = new Vector2Int(targetX, targetY);

        OverlayTile startTile = mapManager.map[startLoc];
        OverlayTile endTile = mapManager.map[endLoc];
        path = pathFinder.FindPath(startTile, endTile, new List<OverlayTile>());
    }

    public void ConfirmAttack()
    {
        float damage;
        if (isMagic)
            damage = character.characterClass.magicDamage;
        else
            damage = character.characterClass.damage;

        AttackUnit(targetUnitTile.unitOnTile, damage, isMagic);

        // NET Implementation
        NetMakeAttack ma = new NetMakeAttack();
        ma.unitX = targetUnitTile.grid2DLocation.x;
        ma.unitY = targetUnitTile.grid2DLocation.y;
        ma.damage = damage;
        ma.teamId = mapManager.currentTeam;

        ma.isMagic = (isMagic) ? 1 : 0;
        Client.Instance.SendToServer(ma);

        isMagic = false;
        enemyData.SetActive(false);

        WaitAction();
        
    }

    private void AttackUnit(CharacterInfo targetUnit, float damage, bool isMagic = false)
    {
        float hitDamage;
        if (!isMagic)
        {
            hitDamage = damage - targetUnit.characterClass.defense;
        } else
        {
            hitDamage = damage - targetUnit.characterClass.resistance;
        }

        targetUnit.currentHealth -= hitDamage;

        if (targetUnit.currentHealth <= 0)
        {
            targetUnit.isDead = true;
        }

        CheckAllUnitsHealth();
    }

    // Function to move along the path
    private void MoveAlongPath()
    {
        var step = speed * Time.deltaTime;

        var zIndex = path[0].transform.position.z;

        character.transform.position = Vector2.MoveTowards(character.transform.position, path[0].transform.position, step);
        character.transform.position = new Vector3(character.transform.position.x, character.transform.position.y, zIndex);

        if (Vector2.Distance(character.transform.position, path[0].transform.position) < 0.0001f)
        {
            PositionCharacterOnTile(character, path[0]);
            path.RemoveAt(0);
        }

        if (path.Count == 0)
        {
            if (character.team == mapManager.currentTeam)
            {
                character.hasMoved = true;
                phaseTwo = true;
                isChoosingUnit = false;
                isMovingUnit = false;
                foreach (var item in inRangeTiles)
                {
                    item.HideTile();
                }
                playerActions.SetActive(true);
                playerActions.transform.Find("Move Button").GetComponent<Button>().interactable = false;

            }

            //character = null;
            //mapManager.isPlayerOneTurn = !mapManager.isPlayerOneTurn;

            //GetInRangeTiles();
        }
    }

    // Shoots a raycast to the tiles
    public RaycastHit2D? GetFocusedOnTile()
    {
        // Gets rayhit position
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2D, Vector2.zero);

        if (hits.Length > 0)
        {
            return hits.OrderByDescending(i => i.collider.transform.position.z).First();
        }
        return null;
    }

    public void PositionCharacterOnTile(CharacterInfo unit, OverlayTile tile)
    {
        unit.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z);
        unit.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder + 5;
        unit.activeTile = tile;
    }

    private void RegisterEvents()
    {
        NetUtility.C_WELCOME += OnWelcomeClient;
        NetUtility.C_MAKE_MOVE += OnMakeMoveClient;
        NetUtility.C_MAKE_ATTACK += OnMakeAttackClient;
        NetUtility.C_SWITCH_TURNS += OnSwitchTurnsClient;
        NetUtility.C_END_GAME += OnWinLoseClient;

        NetUtility.S_MAKE_MOVE += OnMakeMoveServer;
        NetUtility.S_MAKE_ATTACK += OnMakeAttackServer;
        NetUtility.S_SWITCH_TURNS += OnSwitchTurnsServer;
        NetUtility.S_END_GAME += OnWinLoseServer;

    }

    private void UnregisterEvents()
    {
        NetUtility.C_WELCOME -= OnWelcomeClient;
        NetUtility.C_MAKE_MOVE -= OnMakeMoveClient;

        NetUtility.S_MAKE_MOVE -= OnMakeMoveServer;
    }


    // Client
    private void OnWelcomeClient(NetMessage msg)
    {
        NetWelcome nw = msg as NetWelcome;

        GameObject[] units = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject unit in units)
        {
            if (unit.GetComponent<CharacterInfo>().team == nw.AssignedTeam)
                myUnits.Add(unit.GetComponent<CharacterInfo>());
        }
    }

    private void OnMakeMoveServer(NetMessage msg, NetworkConnection cnn)
    {
        NetMakeMove mm = msg as NetMakeMove;

        // Receive, and just broadcast it back
        Server.Instance.Broadcast(mm);
    }
    private void OnMakeMoveClient(NetMessage msg)
    {
        NetMakeMove mm = msg as NetMakeMove;
        Debug.Log("I should be able to move now");

        if (mm.teamId != mapManager.currentTeam)
        {
            Vector2Int movingUnit = new Vector2Int(mm.originalX, mm.originalY);
            character = mapManager.map[movingUnit].unitOnTile;

            FindPath(mm.originalX, mm.originalY, mm.targetX, mm.targetY);

            Vector2Int targetUnitTile = new Vector2Int(mm.targetX, mm.targetY);
            mapManager.map[targetUnitTile].unitOnTile = character;
        }
    }

    private void OnMakeAttackServer(NetMessage msg, NetworkConnection cnn)
    {
        NetMakeAttack ma = msg as NetMakeAttack;

        // Receive, and just broadcast it back
        Server.Instance.Broadcast(ma);
    }
    private void OnMakeAttackClient(NetMessage msg)
    {
        NetMakeAttack ma = msg as NetMakeAttack;


        if (ma.teamId != mapManager.currentTeam)
        {
            Debug.Log("I should be attacked");

            Vector2Int targetUnit = new Vector2Int(ma.unitX, ma.unitY);
            CharacterInfo unit = mapManager.map[targetUnit].unitOnTile;
            AttackUnit(unit, ma.damage, (ma.isMagic == 1) ? true : false);
        }
    }

    private void OnSwitchTurnsServer(NetMessage msg, NetworkConnection cnn)
    {
        NetSwitchTurns st = msg as NetSwitchTurns;

        // Receive, and just broadcast it back
        Server.Instance.Broadcast(st);
    }
    private void OnSwitchTurnsClient(NetMessage msg)
    {
        NetSwitchTurns st = msg as NetSwitchTurns;

        Debug.Log("Switch Turns!");

        if (st.isPlayerOneTurn == 0)
        {
            mapManager.isPlayerOneTurn = true;
        } else
        {
            mapManager.isPlayerOneTurn = false;
        }
    }

    private void OnWinLoseServer(NetMessage msg, NetworkConnection cnn)
    {
        NetEndGame eg = msg as NetEndGame;

        // Receive, and just broadcast it back
        Server.Instance.Broadcast(eg);
    }
    private void OnWinLoseClient(NetMessage msg)
    {
        NetEndGame st = msg as NetEndGame;

        if (st.losingTeamId != mapManager.currentTeam)
        {
            winScreen.SetActive(true);
            loseScreen.SetActive(false);
        }
    }
}
