using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit",menuName = "Scriptable Unit")]
public class ScriptableUnit : ScriptableObject {
    public Faction Faction;
    public BaseUnit UnitPrefab;
}

/// <summary>
/// Keeping base stats as a struct on the scriptable keeps it flexible and easily editable.
/// We can pass this struct to the spawned prefab unit and alter them depending on conditions.
/// </summary>
[Serializable]
public struct Stats {
    public int Life;
    public int Armor;
    public Weapon Weapon;
    public int MoveRadius;

}

[Serializable]
public enum Faction {
    Player = 0,
    Monster = 1
}