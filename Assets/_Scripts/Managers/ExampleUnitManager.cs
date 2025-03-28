using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// An example of a scene-specific manager grabbing resources from the resource system
/// Scene-specific managers are things like grid managers, unit managers, environment managers etc
/// </summary>
public class ExampleUnitManager : StaticInstance<ExampleUnitManager> {

    public List<PlayerUnit> Players = new();

    public PlayerUnit LogInPlayerUnit;

    public void SpawnPlayers(Character[] characters) {
        foreach (var character in characters) {
            SpawnPlayer(character);
        }
    }

    PlayerUnit SpawnPlayer(Character character) {

        /* TODO: This is a bit of a hack, but I don't want to make a new prefab for each unit type.
         * Ideally, you would have a prefab for each unit type and use the scriptable object to set the stats
         * and other properties. But this is an example so I'm keeping it simple.
         */
        var playerUnit = ResourceSystem.Instance.Units.Find(unit => unit != null).UnitPrefab;
        if (playerUnit == null) {
            Debug.LogError("Player unit not found in the resource system");
            return null;
        }        

        var spawnedUnit = (PlayerUnit) Instantiate(playerUnit);

        // Set the position of the unit
        var spawnTile = GridManager.Instance.GetTileAtPosition(new Vector2(character.position.x, character.position.y));
        spawnTile.SetUnit(spawnedUnit);

        spawnedUnit.Character = character;
        spawnedUnit.name = "Unit_" + character.name;
        Players.Add(spawnedUnit);

        // Apply possible modifications here such as potion boosts, team synergies, etc
        // var stats = playerScriptable.BaseStats;
        // stats.Health += 20;

        // spawned.SetStats(stats);
        return spawnedUnit;
    }

    public PlayerUnit GetPlayer(string name) {
        return Players.Find(player => player.Character.name == name);
    }
}

