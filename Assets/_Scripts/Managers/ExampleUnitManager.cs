using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// An example of a scene-specific manager grabbing resources from the resource system
/// Scene-specific managers are things like grid managers, unit managers, environment managers etc
/// </summary>
public class ExampleUnitManager : StaticInstance<ExampleUnitManager> {

    public List<Player> Players = new();

    public Player LogInPlayerUnit;

    public void SpawnPlayers(Character[] characters) {
        foreach (var character in characters) {
            SpawnPlayer(character);
        }
    }

    Player SpawnPlayer(Character character) {

        var playerUnit = ResourceSystem.Instance.Units.Find(u => u.Faction == Faction.Player).UnitPrefab;
        if (playerUnit == null) {
            Debug.LogError("Player unit not found in the resource system");
            return null;
        }        

        var spawnedUnit = (Player) Instantiate(playerUnit);

        if (ExampleGameManager.Instance.LoginPlayerCharacter != null && 
            character.name == ExampleGameManager.Instance.LoginPlayerCharacter.name) 
        {
            LogInPlayerUnit = spawnedUnit;
        }

        // Set the position of the unit
        var spawnTile = GridManager.Instance.GetTileAtPosition(new Vector2(character.position.x, character.position.y));
        spawnTile.SetUnit(spawnedUnit);

        spawnedUnit.character = character;
        spawnedUnit.name = "Unit_" + character.name;
        Players.Add(spawnedUnit);

        // Apply possible modifications here such as potion boosts, team synergies, etc
        // var stats = playerScriptable.BaseStats;
        // stats.Health += 20;

        // spawned.SetStats(stats);
        return spawnedUnit;
    }

    public Player GetPlayer(string name) {
        return Players.Find(player => player.character.name == name);
    }
}

