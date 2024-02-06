using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flaming Badge Warrior/Unit Data")]
public class UnitData : ScriptableObject
{
    public enum UnitClass
    {
        Infantry,
        Cavalry,
        Flier
    }

    

    [SerializeField] public UnitClass unitType;
    [Header("Unit Health")]
    public float healthPoints;
    public int moveRange;

    [Header("Damage")]
    public float damage;
    public int attackRange;

    [Header("Magic Damage")]
    public float magicDamage;
    public int magicRange;

    [Header("Unit's Defense and Resistance")]
    public float defense;
    public float resistance;
}
