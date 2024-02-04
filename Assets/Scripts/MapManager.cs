using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance;

    public static MapManager Instance { get { return _instance; } }

    public OverlayTile overlayTilePrefab;
    public GameObject overlayContainer;
    public List<TileData> tileDatas;

    public Dictionary<Vector2Int, OverlayTile> map;
    public Dictionary<TileBase, TileData> dataFromTiles;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        dataFromTiles = new Dictionary<TileBase, TileData>();
        foreach (var tileData in tileDatas)
        {
            foreach (var tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Gets tilemap component from this gameobject
        var tileMap = gameObject.GetComponentInChildren<Tilemap>();
        map = new Dictionary<Vector2Int, OverlayTile>();


        // Gets tilemap bounds
        BoundsInt bounds = tileMap.cellBounds;

        // looping through every single tile
        for(int y = bounds.max.y; y >= bounds.min.y; y--)
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
}
