using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance;
    public static MapManager Instance { get { return _instance; } }

    [Header("Cursor Object")]
    public MouseController cursor;
    public bool isPlayerOneTurn = true;

    [Header("Tile Overlay Prefab")]
    public OverlayTile overlayTilePrefab;
    public GameObject overlayContainer;

    [Header("Tile Data List")]
    public List<TileData> tileDatas;

    [Header("Unit Prefabs")]
    [SerializeField] private GameObject[] unitPrefabs;

    public Dictionary<Vector2Int, OverlayTile> map;
    public Dictionary<TileBase, TileData> dataFromTiles;

    [SerializeField] private GameObject tileMapObj;

    // Multiplayer Logic
    private int playerCount = -1;
    /*[HideInInspector] */public int currentTeam = -1;

    private void Awake()
    {
        #region Singleton
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        #endregion

        #region Tiledata
        // Gets tiledata scriptable object from the list and adds them to the corresponding tile
        dataFromTiles = new Dictionary<TileBase, TileData>();
        foreach (var tileData in tileDatas)
        {
            foreach (var tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData);
            }
        }
        #endregion

        RegisterEvents();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Gets tilemap component from this gameobject
        var tileMap = tileMapObj.GetComponent<Tilemap>();
        map = new Dictionary<Vector2Int, OverlayTile>();


        // Gets tilemap bounds
        BoundsInt bounds = tileMap.cellBounds;

        // looping through every single tile
        for(int y = bounds.min.y; y <= bounds.max.y; y++)
        {
            for (int x = bounds.min.x; x < bounds.max.x; x++)
            {
                // Gets coord for each tiles
                var tileLocation = new Vector3Int(x, y);
                var tileKey = new Vector2Int(x, y);

                TileBase tile = tileMap.GetTile(tileLocation);

                if (tileMap.HasTile(tileLocation) && !map.ContainsKey(tileKey) && dataFromTiles[tile].tileType != TileData.TileType.Ruins)
                {
                    // Instantiate a "Selection" Overlay for each tile and transform its position to world coords
                    var overlayTile = Instantiate(overlayTilePrefab, overlayContainer.transform);
                    var cellWorldPosition = tileMap.GetCellCenterWorld(tileLocation);

                    overlayTile.transform.position = new Vector3(cellWorldPosition.x, cellWorldPosition.y, cellWorldPosition.z + 1);
                    overlayTile.GetComponent<SpriteRenderer>().sortingOrder = tileMap.GetComponent<TilemapRenderer>().sortingOrder + 1;
                    overlayTile.gridLocation = tileLocation;
                    overlayTile.tileData = dataFromTiles[tile];


                    map.Add(tileKey, overlayTile);
                }
            }
        }

        SpawnAllUnits();
    }

    private void SpawnAllUnits()
    {
        // Spawning first player units
        Vector2Int unitPos = new Vector2Int(1, 1);
        CharacterInfo unitSpawn = Instantiate(unitPrefabs[0]).GetComponent<CharacterInfo>();
        cursor.PositionCharacterOnTile(unitSpawn, map[unitPos]);
        map[unitPos].unitOnTile = unitSpawn;
        unitSpawn.team = 0;

        unitPos = new Vector2Int(2, 1);
        unitSpawn = Instantiate(unitPrefabs[1]).GetComponent<CharacterInfo>();
        cursor.PositionCharacterOnTile(unitSpawn, map[unitPos]);
        map[unitPos].unitOnTile = unitSpawn;
        unitSpawn.team = 0;

        unitPos = new Vector2Int(3,1);
        unitSpawn = Instantiate(unitPrefabs[2]).GetComponent<CharacterInfo>();
        cursor.PositionCharacterOnTile(unitSpawn, map[unitPos]);
        map[unitPos].unitOnTile = unitSpawn;
        unitSpawn.team = 1;

        /*unitPos = new Vector2Int(4,1);
        unitSpawn = Instantiate(unitPrefabs[2]).GetComponent<CharacterInfo>();
        cursor.PositionCharacterOnTile(unitSpawn, map[unitPos]);
        map[unitPos].unitOnTile = unitSpawn;
        unitSpawn.team = 0;


        // Spawning second player units
        unitPos = new Vector2Int(10, 10);
        unitSpawn = Instantiate(unitPrefabs[0]).GetComponent<CharacterInfo>();
        cursor.PositionCharacterOnTile(unitSpawn, map[unitPos]);
        map[unitPos].unitOnTile = unitSpawn;
        unitSpawn.team = 1;

        unitPos = new Vector2Int(9, 10);
        unitSpawn = Instantiate(unitPrefabs[1]).GetComponent<CharacterInfo>();
        cursor.PositionCharacterOnTile(unitSpawn, map[unitPos]);
        map[unitPos].unitOnTile = unitSpawn;
        unitSpawn.team = 1;

        unitPos = new Vector2Int(8, 10);
        unitSpawn = Instantiate(unitPrefabs[2]).GetComponent<CharacterInfo>();
        cursor.PositionCharacterOnTile(unitSpawn, map[unitPos]);
        map[unitPos].unitOnTile = unitSpawn;
        unitSpawn.team = 1;

        unitPos = new Vector2Int(7, 10);
        unitSpawn = Instantiate(unitPrefabs[2]).GetComponent<CharacterInfo>();
        cursor.PositionCharacterOnTile(unitSpawn, map[unitPos]);
        map[unitPos].unitOnTile = unitSpawn;
        unitSpawn.team = 1;*/
    }

    // Check neighbouring tiles in top, bottom, left, and right of the tile
    public List<OverlayTile> GetNeighbourTiles(OverlayTile currentOverlayTile, List<OverlayTile> searchableTiles, int range)
    {
        Dictionary<Vector2Int, OverlayTile> tileToSearch = new Dictionary<Vector2Int, OverlayTile>();

        if (searchableTiles.Count > 0)
        {
            foreach (var item in searchableTiles)
            {
                tileToSearch.Add(item.grid2DLocation, item);
            }
        } else
        {
            Debug.Log("Now searching the whole map");
            tileToSearch = map;
        }

        List<OverlayTile> neighbours = new List<OverlayTile>();

        // Top
        Vector2Int locationToCheck = new Vector2Int(
            currentOverlayTile.gridLocation.x,
            currentOverlayTile.gridLocation.y + 1
            );

        if (tileToSearch.ContainsKey(locationToCheck))
        {
            if (tileToSearch[locationToCheck].tileData.moveCost + currentOverlayTile.moveCost <= range)
            {
                neighbours.Add(tileToSearch[locationToCheck]);
            }
        }

        // Bottom
        locationToCheck = new Vector2Int(
            currentOverlayTile.gridLocation.x,
            currentOverlayTile.gridLocation.y - 1
            );

        if (tileToSearch.ContainsKey(locationToCheck))
        {
            if (tileToSearch[locationToCheck].tileData.moveCost + currentOverlayTile.moveCost <= range)
            {
                neighbours.Add(tileToSearch[locationToCheck]);
            }
        }

        // Right
        locationToCheck = new Vector2Int(
            currentOverlayTile.gridLocation.x + 1,
            currentOverlayTile.gridLocation.y
            );

        if (tileToSearch.ContainsKey(locationToCheck))
        {
            if (tileToSearch[locationToCheck].tileData.moveCost + currentOverlayTile.moveCost <= range)
            {
                neighbours.Add(tileToSearch[locationToCheck]);
            }
        }

        // Left
        locationToCheck = new Vector2Int(
            currentOverlayTile.gridLocation.x - 1,
            currentOverlayTile.gridLocation.y
            );

        if (tileToSearch.ContainsKey(locationToCheck))
        {
            if (tileToSearch[locationToCheck].tileData.moveCost + currentOverlayTile.moveCost <= range)
            {
                neighbours.Add(tileToSearch[locationToCheck]);
            }
        }

        return neighbours;
    }

    private void RegisterEvents()
    {
        NetUtility.S_WELCOME += OnWelcomeServer;

        NetUtility.C_WELCOME += OnWelcomeClient;
        NetUtility.C_START_GAME += OnStartGameClient;
    }

    // Server
    private void OnWelcomeServer(NetMessage msg, NetworkConnection cnn)
    {
        // Client has connected, assign a team and return the message to them
        NetWelcome nw = msg as NetWelcome;

        // Assign a team
        nw.AssignedTeam = ++playerCount;

        // Return back to client
        Server.Instance.SendToClient(cnn, nw);

        // If full, start the game
        if (playerCount == 1)
            Server.Instance.Broadcast(new NetStartGame());
    }

    // Client
    private void OnWelcomeClient(NetMessage msg)
    {
        // Receive the connection message
        NetWelcome nw = msg as NetWelcome;

        // Assign the team
        currentTeam = nw.AssignedTeam;

        Debug.Log($"My Assigned team is {nw.AssignedTeam}");
    }
    private void OnStartGameClient(NetMessage obj)
    {
        GameUI.Instance.ChangeCamera((currentTeam == 0) ? CameraAngle.playerOne : CameraAngle.playerTwo);
    }
}
