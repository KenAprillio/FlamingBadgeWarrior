using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo : MonoBehaviour
{
    public enum CharacterClass
    {
        Infantry,
        Cavalry,
        Flier
    }
    public CharacterClass characterClass;
    public OverlayTile activeTile;
}
