using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayTile : MonoBehaviour
{
    public int G;
    public int H;

    public int F { get { return G + H; } }

    public bool isBlocked;

    public OverlayTile previous;
    public Vector3Int gridLocation;
    public TileData tileData;
    public int moveCost;

    public bool isStartingTile = false;

    public Vector2Int grid2DLocation { get { return new Vector2Int(gridLocation.x, gridLocation.y); } }

    

    // Show the Selection Highlight
    public void ShowTile()
    {
        if (isStartingTile)
        {
            moveCost = 0;
        } else
        {
            moveCost = tileData.moveCost;
        }
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }

    // Hide the Selection Highlight
    public void HideTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        isStartingTile = false;
    }
}
