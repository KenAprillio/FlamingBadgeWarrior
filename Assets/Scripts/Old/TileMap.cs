using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TileMap : MonoBehaviour
{
    public TileType[] tileTypes;
    public GameObject selectedUnit;

    Node[,] graph;
    int[,] tiles;

    int mapSizeX = 8;
    int mapSizeY = 8;

    private void Start()
    {
        GenerateMapData();
        GenerateMapVisual();
    }

    void GenerateMapData()
    {
        // Allocating map tiles
        tiles = new int[mapSizeX, mapSizeY];

        // Initialize map tiles to be plains
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                tiles[x, y] = 0;
            }
        }

        tiles[4, 4] = 2;
        tiles[4, 5] = 2;
        tiles[4, 6] = 2;
        tiles[4, 7] = 2;

        tiles[5, 7] = 2;
        tiles[6, 7] = 2;
        tiles[7, 7] = 2;

        tiles[7, 6] = 2;
        tiles[7, 5] = 2;
        tiles[7, 4] = 2;
    }

    class Node
    {
        public List<Node> neighbours;

        public Node()
        {
            neighbours = new List<Node>();
        }
    }


    void GeneratePathfindingGraph()
    {
        graph = new Node[mapSizeX, mapSizeY];

        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeX; y++)
            {
                if (x > 0)
                    graph[x, y].neighbours.Add(graph[x - 1, y]);
                if (x < mapSizeX - 1)
                    graph[x, y].neighbours.Add(graph[x - 1, y]);
                if (y > 0)
                    graph[x, y].neighbours.Add(graph[x, y - 1]);
                if (y > mapSizeY - 1)
                    graph[x, y].neighbours.Add(graph[x, y - 1]);
            }
        }
    }

    void GenerateMapVisual()
    {
        for (int x = 0; x < mapSizeX; x++)
        {
            for(int y = 0; y < mapSizeX; y++)
            {
                TileType tt = tileTypes[tiles[x,y]];

                GameObject go = Instantiate(tt.tileVisualPrefab, new Vector3(x, y, 0), Quaternion.identity);

                ClickableTile ct = go.GetComponent<ClickableTile>();
                ct.tileX = x;
                ct.tileY = y;
                ct.map = this;
            }
        }
    }

    public Vector2 TileCoordToWorldCoord(int x, int y)
    {
        return new Vector2(x, y);
    }

    public void MoveSelectedUnitTo(int x, int y)
    {
        selectedUnit.GetComponent<Unit>().tileX = x;
        selectedUnit.GetComponent<Unit>().tileY = y;
        selectedUnit.transform.position = TileCoordToWorldCoord(x,y);
    }

}
