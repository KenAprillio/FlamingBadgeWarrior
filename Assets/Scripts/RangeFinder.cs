using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RangeFinder
{
    public List<OverlayTile> GetTilesInRange(OverlayTile startingTile, int range, CharacterInfo selectedChar)
    {
        var inRangeTiles = new List<OverlayTile>();
        int stepCount = 0;

        inRangeTiles.Add(startingTile);

        var tileForPreviousStep = new List<OverlayTile>();
        tileForPreviousStep.Add(startingTile);

        startingTile.isStartingTile = true;

        var surroundingTiles = new List<OverlayTile>();

        surroundingTiles.AddRange(GetNeighbouringTopTiles(startingTile, range));
        surroundingTiles.AddRange(GetNeighbouringRightTiles(startingTile, range));
        inRangeTiles.AddRange(surroundingTiles);


        return inRangeTiles.Distinct().ToList();
    }

    // Get surrounding top and bottom tiles
    public List<OverlayTile> GetNeighbouringTopTiles(OverlayTile overlayTile, int range)
    {
        var surroundingTiles = new List<OverlayTile>();
        var map = MapManager.Instance.map;

        OverlayTile currentTile = overlayTile;
        for (int i = range; i > 0;)
        {
            Vector2Int locationToCheck = new Vector2Int(currentTile.gridLocation.x, currentTile.gridLocation.y + 1);

            if (map.ContainsKey(locationToCheck))
            {
                if (map[locationToCheck].tileData.moveCost <= i)
                {
                    surroundingTiles.Add(map[locationToCheck]);
                    currentTile = map[locationToCheck];
                    i -= map[locationToCheck].tileData.moveCost;

                    Vector2Int leftOfTile = new Vector2Int(currentTile.gridLocation.x - 1, currentTile.gridLocation.y);
                    Vector2Int rightOfTile = new Vector2Int(currentTile.gridLocation.x + 1, currentTile.gridLocation.y);

                    if (map.ContainsKey(leftOfTile))
                    {
                        if (map[leftOfTile].tileData.moveCost <= i)
                        {
                            surroundingTiles.Add(map[leftOfTile]);
                        }
                    }

                    if (map.ContainsKey(rightOfTile))
                    {
                        if (map[rightOfTile].tileData.moveCost <= i)
                        {
                            surroundingTiles.Add(map[rightOfTile]);
                        }
                    }
                } else
                {
                    break;
                }
            } else
            {
                break;
            }
        }
        currentTile = overlayTile;
        for (int i = range; i > 0;)
        {
            Vector2Int locationToCheck = new Vector2Int(currentTile.gridLocation.x, currentTile.gridLocation.y - 1);

            if (map.ContainsKey(locationToCheck))
            {
                if (map[locationToCheck].tileData.moveCost <= i)
                {
                    surroundingTiles.Add(map[locationToCheck]);
                    currentTile = map[locationToCheck];
                    i -= map[locationToCheck].tileData.moveCost;

                    Vector2Int leftOfTile = new Vector2Int(currentTile.gridLocation.x - 1, currentTile.gridLocation.y);
                    Vector2Int rightOfTile = new Vector2Int(currentTile.gridLocation.x + 1, currentTile.gridLocation.y);

                    if (map.ContainsKey(leftOfTile))
                    {
                        if (map[leftOfTile].tileData.moveCost <= i)
                        {
                            surroundingTiles.Add(map[leftOfTile]);
                        }
                    }

                    if (map.ContainsKey(rightOfTile))
                    {
                        if (map[rightOfTile].tileData.moveCost <= i)
                        {
                            surroundingTiles.Add(map[rightOfTile]);
                        }
                    }
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }
        return surroundingTiles;
    }

    // Get surrounding left and right tiles
    public List<OverlayTile> GetNeighbouringRightTiles(OverlayTile overlayTile, int range)
    {
        var surroundingTiles = new List<OverlayTile>();
        var map = MapManager.Instance.map;

        OverlayTile currentTile = overlayTile;
        for (int i = range; i > 0;)
        {
            Vector2Int locationToCheck = new Vector2Int(currentTile.gridLocation.x + 1, currentTile.gridLocation.y);

            if (map.ContainsKey(locationToCheck))
            {
                if (map[locationToCheck].tileData.moveCost <= i)
                {
                    surroundingTiles.Add(map[locationToCheck]);
                    currentTile = map[locationToCheck];
                    i -= map[locationToCheck].tileData.moveCost;

                    Vector2Int topOfTile = new Vector2Int(currentTile.gridLocation.x, currentTile.gridLocation.y + 1);
                    Vector2Int bottomOfTile = new Vector2Int(currentTile.gridLocation.x, currentTile.gridLocation.y - 1);

                    if (map.ContainsKey(topOfTile))
                    {
                        if (map[topOfTile].tileData.moveCost <= i)
                        {
                            surroundingTiles.Add(map[topOfTile]);
                        }
                    }

                    if (map.ContainsKey(bottomOfTile))
                    {
                        if (map[bottomOfTile].tileData.moveCost <= i)
                        {
                            surroundingTiles.Add(map[bottomOfTile]);
                        }
                    }

                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }

        currentTile = overlayTile;
        for (int i = range; i > 0;)
        {
            Vector2Int locationToCheck = new Vector2Int(currentTile.gridLocation.x - 1, currentTile.gridLocation.y);

            if (map.ContainsKey(locationToCheck))
            {
                if (map[locationToCheck].tileData.moveCost <= i)
                {
                    surroundingTiles.Add(map[locationToCheck]);
                    currentTile = map[locationToCheck];
                    i -= map[locationToCheck].tileData.moveCost;

                    Vector2Int topOfTile = new Vector2Int(currentTile.gridLocation.x, currentTile.gridLocation.y + 1);
                    Vector2Int bottomOfTile = new Vector2Int(currentTile.gridLocation.x, currentTile.gridLocation.y - 1);

                    if (map.ContainsKey(topOfTile))
                    {
                        if (map[topOfTile].tileData.moveCost <= i)
                        {
                            surroundingTiles.Add(map[topOfTile]);
                        }
                    }

                    if (map.ContainsKey(bottomOfTile))
                    {
                        if (map[bottomOfTile].tileData.moveCost <= i)
                        {
                            surroundingTiles.Add(map[bottomOfTile]);
                        }
                    }

                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }
        return surroundingTiles;
    }
}


