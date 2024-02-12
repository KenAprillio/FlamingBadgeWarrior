using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RangeFinder
{
    public List<OverlayTile> GetTilesInRange(OverlayTile startingTile, int range, CharacterInfo selectedChar, bool isAttacking = false)
    {
        var inRangeTiles = new List<OverlayTile>();

        inRangeTiles.Add(startingTile);

        var tileForPreviousStep = new List<OverlayTile>();
        tileForPreviousStep.Add(startingTile);

        startingTile.isStartingTile = true;

        var surroundingTiles = new List<OverlayTile>();

        surroundingTiles.AddRange(GetNeighbouringTopTiles(startingTile, range, selectedChar, isAttacking));
        surroundingTiles.AddRange(GetNeighbouringRightTiles(startingTile, range, selectedChar, isAttacking));
        inRangeTiles.AddRange(surroundingTiles);

        return inRangeTiles.Distinct().ToList();
    }

    // Get surrounding top and bottom tiles
    public List<OverlayTile> GetNeighbouringTopTiles(OverlayTile overlayTile, int range, CharacterInfo character, bool isAttacking = false)
    {
        var surroundingTiles = new List<OverlayTile>();
        var map = MapManager.Instance.map;

        OverlayTile currentTile = overlayTile;

        // Get Top Tiles
        #region Top Tiles
        for (int i = range; i > 0;)
        {
            Vector2Int locationToCheck = new Vector2Int(currentTile.gridLocation.x, currentTile.gridLocation.y + 1);

            if (map.ContainsKey(locationToCheck))
            {
                if (map[locationToCheck].tileData.tileType == TileData.TileType.Ruins)
                    break;

                if (map[locationToCheck].tileData.moveCost <= i)
                {
                    if (!isAttacking)
                    {
                        if (map[locationToCheck].tileData.tileType == TileData.TileType.Mountains &&
                        character.characterClass.unitClass != UnitData.UnitClass.Flier)
                            break;
                        else if (map[locationToCheck].tileData.tileType == TileData.TileType.Forest &&
                            character.characterClass.unitClass == UnitData.UnitClass.Cavalry)
                            break;
                        else if (map[locationToCheck].tileData.tileType == TileData.TileType.Forest &&
                            character.characterClass.unitClass == UnitData.UnitClass.Infantry)
                            i -= map[locationToCheck].tileData.moveCost;
                        else
                            i--;
                    } else
                    {
                        i--;
                    }


                    surroundingTiles.Add(map[locationToCheck]);
                    currentTile = map[locationToCheck];

                    Vector2Int leftOfTile = new Vector2Int(currentTile.gridLocation.x - 1, currentTile.gridLocation.y);
                    Vector2Int rightOfTile = new Vector2Int(currentTile.gridLocation.x + 1, currentTile.gridLocation.y);

                    if (map.ContainsKey(leftOfTile))
                    {
                        if (map[leftOfTile].tileData.moveCost <= i)
                        {
                            if (isAttacking)
                                surroundingTiles.Add(map[leftOfTile]);
                            else if ((map[locationToCheck].tileData.tileType == TileData.TileType.Mountains && character.characterClass.unitClass == UnitData.UnitClass.Flier) ||
                                (map[locationToCheck].tileData.tileType == TileData.TileType.Forest && character.characterClass.unitClass != UnitData.UnitClass.Cavalry) ||
                                (map[locationToCheck].tileData.tileType == TileData.TileType.Plains))
                                surroundingTiles.Add(map[leftOfTile]);
                        }
                    }

                    if (map.ContainsKey(rightOfTile))
                    {
                        if (map[rightOfTile].tileData.moveCost <= i)
                        {
                            if (isAttacking)
                                surroundingTiles.Add(map[rightOfTile]);
                            else if ((map[locationToCheck].tileData.tileType == TileData.TileType.Mountains && character.characterClass.unitClass == UnitData.UnitClass.Flier) ||
                                (map[locationToCheck].tileData.tileType == TileData.TileType.Forest && character.characterClass.unitClass != UnitData.UnitClass.Cavalry) ||
                                (map[locationToCheck].tileData.tileType == TileData.TileType.Plains))
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
        #endregion

        // Get Bottom Tiles
        #region Bottom Tiles
        currentTile = overlayTile;
        for (int i = range; i > 0;)
        {
            Vector2Int locationToCheck = new Vector2Int(currentTile.gridLocation.x, currentTile.gridLocation.y - 1);

            if (map.ContainsKey(locationToCheck))
            {
                if (map[locationToCheck].tileData.tileType == TileData.TileType.Ruins)
                    break;
                if (map[locationToCheck].tileData.moveCost <= i)
                {
                    if (!isAttacking)
                    {
                        if (map[locationToCheck].tileData.tileType == TileData.TileType.Mountains &&
                        character.characterClass.unitClass != UnitData.UnitClass.Flier)
                            break;
                        else if (map[locationToCheck].tileData.tileType == TileData.TileType.Forest &&
                            character.characterClass.unitClass == UnitData.UnitClass.Cavalry)
                            break;
                        else if (map[locationToCheck].tileData.tileType == TileData.TileType.Forest &&
                            character.characterClass.unitClass == UnitData.UnitClass.Infantry)
                            i -= map[locationToCheck].tileData.moveCost;
                        else
                            i--;
                    }
                    else
                    {
                        i--;
                    }

                    surroundingTiles.Add(map[locationToCheck]);
                    currentTile = map[locationToCheck];

                    Vector2Int leftOfTile = new Vector2Int(currentTile.gridLocation.x - 1, currentTile.gridLocation.y);
                    Vector2Int rightOfTile = new Vector2Int(currentTile.gridLocation.x + 1, currentTile.gridLocation.y);

                    if (map.ContainsKey(leftOfTile))
                    {
                        if (map[leftOfTile].tileData.moveCost <= i)
                        {
                            if (isAttacking)
                                surroundingTiles.Add(map[leftOfTile]);
                            else if ((map[locationToCheck].tileData.tileType == TileData.TileType.Mountains && character.characterClass.unitClass == UnitData.UnitClass.Flier) ||
                                (map[locationToCheck].tileData.tileType == TileData.TileType.Forest && character.characterClass.unitClass != UnitData.UnitClass.Cavalry) ||
                                (map[locationToCheck].tileData.tileType == TileData.TileType.Plains))
                                surroundingTiles.Add(map[leftOfTile]);
                        }
                    }

                    if (map.ContainsKey(rightOfTile))
                    {
                        if (map[rightOfTile].tileData.moveCost <= i)
                        {
                            if (isAttacking)
                                surroundingTiles.Add(map[rightOfTile]);
                            else if ((map[locationToCheck].tileData.tileType == TileData.TileType.Mountains && character.characterClass.unitClass == UnitData.UnitClass.Flier) ||
                                (map[locationToCheck].tileData.tileType == TileData.TileType.Forest && character.characterClass.unitClass != UnitData.UnitClass.Cavalry) ||
                                (map[locationToCheck].tileData.tileType == TileData.TileType.Plains))
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
        #endregion
        return surroundingTiles;
    }

    // Get surrounding left and right tiles
    public List<OverlayTile> GetNeighbouringRightTiles(OverlayTile overlayTile, int range, CharacterInfo character, bool isAttacking = false)
    {
        var surroundingTiles = new List<OverlayTile>();
        var map = MapManager.Instance.map;

        OverlayTile currentTile = overlayTile;

        // Get Right Tiles
        #region Right Tiles
        for (int i = range; i > 0;)
        {
            Vector2Int locationToCheck = new Vector2Int(currentTile.gridLocation.x + 1, currentTile.gridLocation.y);

            if (map.ContainsKey(locationToCheck))
            {
                if (map[locationToCheck].tileData.tileType == TileData.TileType.Ruins)
                    break;
                if (map[locationToCheck].tileData.moveCost <= i)
                {
                    if (!isAttacking)
                    {
                        if (map[locationToCheck].tileData.tileType == TileData.TileType.Mountains &&
                        character.characterClass.unitClass != UnitData.UnitClass.Flier)
                            break;
                        else if (map[locationToCheck].tileData.tileType == TileData.TileType.Forest &&
                            character.characterClass.unitClass == UnitData.UnitClass.Cavalry)
                            break;
                        else if (map[locationToCheck].tileData.tileType == TileData.TileType.Forest &&
                            character.characterClass.unitClass == UnitData.UnitClass.Infantry)
                            i -= map[locationToCheck].tileData.moveCost;
                        else
                            i--;
                    }
                    else
                    {
                        i--;
                    }

                    surroundingTiles.Add(map[locationToCheck]);
                    currentTile = map[locationToCheck];

                    Vector2Int topOfTile = new Vector2Int(currentTile.gridLocation.x, currentTile.gridLocation.y + 1);
                    Vector2Int bottomOfTile = new Vector2Int(currentTile.gridLocation.x, currentTile.gridLocation.y - 1);

                    if (map.ContainsKey(topOfTile))
                    {
                        if (map[topOfTile].tileData.moveCost <= i)
                        {
                            if (isAttacking)
                                surroundingTiles.Add(map[topOfTile]);
                            else if ((map[locationToCheck].tileData.tileType == TileData.TileType.Mountains && character.characterClass.unitClass == UnitData.UnitClass.Flier) ||
                                (map[locationToCheck].tileData.tileType == TileData.TileType.Forest && character.characterClass.unitClass != UnitData.UnitClass.Cavalry) ||
                                (map[locationToCheck].tileData.tileType == TileData.TileType.Plains))
                                surroundingTiles.Add(map[topOfTile]);
                        }
                    }

                    if (map.ContainsKey(bottomOfTile))
                    {
                        if (map[bottomOfTile].tileData.moveCost <= i)
                        {
                            if (isAttacking)
                                surroundingTiles.Add(map[bottomOfTile]);
                            else if ((map[locationToCheck].tileData.tileType == TileData.TileType.Mountains && character.characterClass.unitClass == UnitData.UnitClass.Flier) ||
                                (map[locationToCheck].tileData.tileType == TileData.TileType.Forest && character.characterClass.unitClass != UnitData.UnitClass.Cavalry) ||
                                (map[locationToCheck].tileData.tileType == TileData.TileType.Plains))
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
        #endregion

        // Get Left Tiles
        #region Left Tiles
        currentTile = overlayTile;
        for (int i = range; i > 0;)
        {
            Vector2Int locationToCheck = new Vector2Int(currentTile.gridLocation.x - 1, currentTile.gridLocation.y);

            if (map.ContainsKey(locationToCheck))
            {
                if (map[locationToCheck].tileData.tileType == TileData.TileType.Ruins)
                    break;
                if (map[locationToCheck].tileData.moveCost <= i)
                {
                    if (!isAttacking)
                    {
                        if (map[locationToCheck].tileData.tileType == TileData.TileType.Mountains &&
                        character.characterClass.unitClass != UnitData.UnitClass.Flier)
                            break;
                        else if (map[locationToCheck].tileData.tileType == TileData.TileType.Forest &&
                            character.characterClass.unitClass == UnitData.UnitClass.Cavalry)
                            break;
                        else if (map[locationToCheck].tileData.tileType == TileData.TileType.Forest &&
                            character.characterClass.unitClass == UnitData.UnitClass.Infantry)
                            i -= map[locationToCheck].tileData.moveCost;
                        else
                            i--;
                    }
                    else
                    {
                        i--;
                    }

                    surroundingTiles.Add(map[locationToCheck]);
                    currentTile = map[locationToCheck];

                    Vector2Int topOfTile = new Vector2Int(currentTile.gridLocation.x, currentTile.gridLocation.y + 1);
                    Vector2Int bottomOfTile = new Vector2Int(currentTile.gridLocation.x, currentTile.gridLocation.y - 1);

                    if (map.ContainsKey(topOfTile))
                    {
                        if (map[topOfTile].tileData.moveCost <= i)
                        {
                            if (isAttacking)
                                surroundingTiles.Add(map[topOfTile]);
                            else if ((map[locationToCheck].tileData.tileType == TileData.TileType.Mountains && character.characterClass.unitClass == UnitData.UnitClass.Flier) ||
                                (map[locationToCheck].tileData.tileType == TileData.TileType.Forest && character.characterClass.unitClass != UnitData.UnitClass.Cavalry) ||
                                (map[locationToCheck].tileData.tileType == TileData.TileType.Plains))
                                surroundingTiles.Add(map[topOfTile]);
                        }
                    }

                    if (map.ContainsKey(bottomOfTile))
                    {
                        if (map[bottomOfTile].tileData.moveCost <= i)
                        {
                            if (isAttacking)
                                surroundingTiles.Add(map[bottomOfTile]);
                            else if ((map[locationToCheck].tileData.tileType == TileData.TileType.Mountains && character.characterClass.unitClass == UnitData.UnitClass.Flier) ||
                                (map[locationToCheck].tileData.tileType == TileData.TileType.Forest && character.characterClass.unitClass != UnitData.UnitClass.Cavalry) ||
                                (map[locationToCheck].tileData.tileType == TileData.TileType.Plains))
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
        #endregion
        return surroundingTiles;
    }
}


