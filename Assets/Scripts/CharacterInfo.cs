using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo : MonoBehaviour
{
    public UnitData characterClass;
    public OverlayTile activeTile;
    public float currentHealth;
    public int team;
    public bool hasMoved = false;
    public bool isPlayed = false;

    private void Start()
    {
        currentHealth = characterClass.healthPoints;
    }
}
