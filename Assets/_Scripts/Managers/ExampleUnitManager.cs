using UnityEngine;

/// <summary>
/// An example of a scene-specific manager grabbing resources from the resource system
/// Scene-specific managers are things like grid managers, unit managers, environment managers etc
/// </summary>
public class ExampleUnitManager : StaticInstance<ExampleUnitManager> {

    public void SpawnHeroes(Character[] characters) {
        foreach (var character in characters) {
            SpawnUnit(character);
        }
    }

    void SpawnUnit(Character character) {

        var playerUnit = ResourceSystem.Instance.Units.Find(u => u.Faction == Faction.Player).UnitPrefab;
        if (playerUnit == null) {
            Debug.LogError("Player unit not found in the resource system");
            return;
        }

        var spawnedUnit = Instantiate(playerUnit);

        // Set the position of the unit
        var spawnTile = GridManager.Instance.GetTileAtPosition(new Vector2(character.position.x, character.position.y));
        spawnTile.SetUnit(spawnedUnit);

        // Apply possible modifications here such as potion boosts, team synergies, etc
        // var stats = playerScriptable.BaseStats;
        // stats.Health += 20;

        // spawned.SetStats(stats);
    }
}