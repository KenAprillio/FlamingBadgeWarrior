using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    public bool isMovingUnit;
    [HideInInspector] public bool isAttackingUnit;

    [Header("Gameplay UI Gameobjects")]
    public GameObject unitData;
    public GameObject playerActions;



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
                if (overlayTile.unitOnTile && !overlayTile.unitOnTile.isPlayed)
                {
                    if ((overlayTile.unitOnTile.team == 0 && mapManager.isPlayerOneTurn && mapManager.currentTeam == 0) || 
                        (overlayTile.unitOnTile.team == 1 && !mapManager.isPlayerOneTurn && mapManager.currentTeam == 1))
                    {
                        character = overlayTile.unitOnTile;
                        PositionCharacterOnTile(character, overlayTile);
                        GetUnitInfoAndShowUI();
                        //GetInRangeTiles();
                    }
                } else if ((character && overlayTile.isAccesible) && isMovingUnit)
                {
                    character.activeTile.unitOnTile = null;
                    
                    // find path to the clicked tile
                    path = pathFinder.FindPath(character.activeTile, overlayTile, inRangeTiles);
                    overlayTile.unitOnTile = character;
                    foreach (var item in inRangeTiles)
                    {
                        item.HideTile();
                    }
                } else if (!isMovingUnit)
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
    }

    public void GetInRangeTiles()
    {
        foreach (var item in inRangeTiles)
        {
            item.HideTile();
        }

        isMovingUnit = true;
        inRangeTiles = rangeFinder.GetTilesInRange(character.activeTile, character.characterClass.moveRange, character);

        foreach (var item in inRangeTiles)
        {
            item.ShowTile();
        }
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
            character.isPlayed = true;
            foreach (var item in inRangeTiles)
            {
                item.HideTile();
            }
            character = null;
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
}
