using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Nice, easy to understand enum-based game manager. For larger and more complex games, look into
/// state machines. But this will serve just fine for most games.
/// </summary>
public class ExampleGameManager : StaticInstance<ExampleGameManager> {
    public static event Action<GameState> OnBeforeStateChanged;
    public static event Action<GameState> OnAfterStateChanged;

    public GameState State { get; private set; }

    private DatabaseManager dbManager;

    // Kick the game off with the first state
    void Start() {
        dbManager = FindFirstObjectByType<DatabaseManager>();
        if (dbManager == null) {
            Debug.LogError("DatabaseManager not found");
        }
        ChangeState(GameState.Starting);
    }

    public void ChangeState(GameState newState) {
        OnBeforeStateChanged?.Invoke(newState);

        State = newState;
        switch (newState) {
            case GameState.Starting:
                HandleStarting();
                break;
            case GameState.SpawningHeroes:
                HandleSpawningHeroes();
                break;
            case GameState.SpawningEnemies:
                // HandleSpawningEnemies();
                break;
            case GameState.HeroTurn:
                // HandleHeroTurn();
                break;
            case GameState.EnemyTurn:
                break;
            case GameState.Win:
                break;
            case GameState.Lose:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnAfterStateChanged?.Invoke(newState);
        
        Debug.LogWarning($"New state: {newState}");
    }

    private void HandleStarting() {
        // Do some start setup, could be environment, cinematics etc
        GridManager.Instance.GenerateGrid(10, 10);

        // Start a coroutine to wait for the db data to be loaded
        StartCoroutine(WaitForDataAndChangeState());
    }

    private IEnumerator WaitForDataAndChangeState() {
        if (dbManager != null) {
            // Wait until the data is loaded
            while (!dbManager.IsDataLoaded) {
                yield return null;
            }

            // Data is loaded, change to the next state
            ChangeState(GameState.SpawningHeroes);
        } else {
            Debug.LogError("DatabaseManager not found");
        }
    }

    private void HandleSpawningHeroes() {
        if (dbManager != null && dbManager.Characters != null)
        {
            // foreach (Character character in dbManager.Characters)
            // {
            //     Debug.Log($"Name: {character.name}, Health: {character.life}, Armor: {character.armor}, Weapon: {character.weapon.type}");
            // }

            ExampleUnitManager.Instance.SpawnHeroes(dbManager.GameStatus.chars);
            ChangeState(GameState.SpawningEnemies);
        }
        else
        {
            Debug.LogError("DatabaseManager not found or Characters not loaded");
        }
    }

    private void HandleSpawningEnemies() {
        
        // Spawn enemies
        
        ChangeState(GameState.HeroTurn);
    }

    private void HandleHeroTurn() {
        // If you're making a turn based game, this could show the turn menu, highlight available units etc
        
        // Keep track of how many units need to make a move, once they've all finished, change the state. This could
        // be monitored in the unit manager or the units themselves.
    }
}

/// <summary>
/// This is obviously an example and I have no idea what kind of game you're making.
/// You can use a similar manager for controlling your menu states or dynamic-cinematics, etc
/// </summary>
[Serializable]
public enum GameState {
    Starting = 0,
    SpawningHeroes = 1,
    SpawningEnemies = 2,
    HeroTurn = 3,
    EnemyTurn = 4,
    Win = 5,
    Lose = 6,
}