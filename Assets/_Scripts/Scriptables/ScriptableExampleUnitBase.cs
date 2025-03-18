using System;
using UnityEngine;

/// <summary>
/// Keeping all relevant information about a unit on a scriptable means we can gather and show
/// info on the menu screen, without instantiating the unit prefab.
/// </summary>
public abstract class ScriptableExampleUnitBase : ScriptableObject {
    public Faction Faction;

    [SerializeField] private Stats _stats;
    public Stats BaseStats => _stats;

    // Used in game
    public HeroUnitBase Prefab;
    
    // Used in menus
    public string Name;
    public Sprite MenuSprite;
}

/// <summary>
/// Keeping base stats as a struct on the scriptable keeps it flexible and easily editable.
/// We can pass this struct to the spawned prefab unit and alter them depending on conditions.
/// </summary>
[Serializable]
public struct Stats {
    public int Life;
    public int Armor;
    public string WeaponRange;
}

[Serializable]
public enum Faction {
    Heroes = 0,
    Enemies = 1
}