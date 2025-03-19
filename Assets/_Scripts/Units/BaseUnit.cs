using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This will share logic for any unit on the field. Could be friend or foe, controlled or not.
/// Things like taking damage, dying, animation triggers etc
/// </summary>
public class BaseUnit : MonoBehaviour {
    public string UnitName;
    public Tile OccupiedTile;
    public Faction Faction;

    public void Update()
    {
        
    }

    public void moveUnit(Tile targetTile)
    {
        if (targetTile == null)
        {
            Debug.LogError("Target tile is null");
            return;
        }

        transform.Translate(targetTile.transform.position - transform.position);

        targetTile.SetUnit(this);
    }
}