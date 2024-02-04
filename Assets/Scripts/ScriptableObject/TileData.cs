using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TileData : ScriptableObject
{
    public enum TileType
    {
        Plains,
        Forest,
        Mountains,
        Ruins
    }
    public TileBase[] tiles;
    [SerializeField] public TileType tileType;

    public int moveCost
    {
        get {
            if (tileType == TileType.Plains)
            {
                return 1;
            } else if (tileType == TileType.Forest)
            {
                return 2;
            } else if (tileType == TileType.Mountains)
            {
                return 3;
            } else
            {
                return 99;
            }
        }
    }
}
