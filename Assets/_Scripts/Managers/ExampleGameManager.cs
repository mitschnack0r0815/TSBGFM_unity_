using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using System.Linq;

/// <summary>
/// Nice, easy to understand enum-based game manager. For larger and more complex games, look into
/// state machines. But this will serve just fine for most games.
/// </summary>
public class ExampleGameManager : StaticInstance<ExampleGameManager> {
    public static event Action<GameState> OnBeforeStateChanged;
    public static event Action<GameState> OnAfterStateChanged;

    [SerializeField] public Transform Cam;

    public GameState State { get; private set; }

    public Unit LoginPlayerUnit { get; set; }

    private DatabaseManager _dbManager;

    // Kick the game off with the first state
    void Start() {
        _dbManager = DatabaseManager.Instance;
        if (_dbManager == null) {
            Debug.LogError("DatabaseManager not found");
        }
        ChangeState(GameState.Loading);
    }

    public void ChangeState(GameState newState) {
        OnBeforeStateChanged?.Invoke(newState);

        State = newState;
        switch (newState) {
            case GameState.Loading:
                // Start a coroutine to wait for the db data to be loaded
                StartCoroutine(HandleLoading());
                break;
            case GameState.Starting:
                HandleStarting();
                break;
            case GameState.SpawningPlayers:
                HandleHeroes();
                break;
            case GameState.SpawningEnemies:
                // HandleSpawningEnemies();
                break;
            case GameState.PlayerTurn:
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

    private IEnumerator HandleLoading() {
        if (_dbManager != null) {
            // Wait until the data is loaded
            while (!_dbManager.IsDataLoaded) {
                yield return null;
            }           

            MainMenuScreen.Instance.GenerateUI();
            GridManager.Instance.GenerateGrid(_dbManager.GameStatus.board);

            ChangeState(GameState.Starting);            
        } else {
            Debug.LogError("DatabaseManager not found");
        }
    }

    private void HandleStarting() {
        if (!_dbManager.IsDataLoaded) DatabaseNotLoaded();

        

        ChangeState(GameState.SpawningPlayers);
    }


    private void HandleHeroes() {
        if (!_dbManager.IsDataLoaded) {
            DatabaseNotLoaded();
            return;
        }

        if (ExampleUnitManager.Instance.PlayerUnits.Count > 0) {
            Debug.Log("Players already spawned");

            return;
        } else {
            Debug.LogWarning("Spawning players");

            // Spawn the players
            ExampleUnitManager.Instance.SpawnPlayerUnits(_dbManager.GameStatus.players);

            // For now, set the first character to the login player
            SetLoginPlayerUnit(_dbManager.GameStatus.players[0].units[0]);

            Debug.Log("Login player: " + ExampleUnitManager.Instance.LogInPlayerUnit.Unit.name);
            ExampleUnitManager.Instance.LogInPlayerUnit.isLogInPlayerUnit = true;

            // Set the player dropdown
            var dropdown = MainMenuScreen.Instance.PlayerDropdown;
            var playerNameList = _dbManager.GameStatus.players.Select(u => u.playerName).ToList();
            dropdown.choices = playerNameList;
            dropdown.value = "Choose Player";
            MainMenuScreen.OnPlayerDropdownChoose += () => {
                Debug.Log("Player dropdown changed: " + dropdown.value);

                // SetLoginPlayerUnit(ExampleUnitManager.Instance.GetPlayer(unitId).Unit);
            };
        }

        ChangeState(GameState.SpawningEnemies);
    }

    private void HandleEnemies() {
        
        // Spawn enemies
        
        ChangeState(GameState.PlayerTurn);
    }

    private void HandleHeroTurn() {
        // If you're making a turn based game, this could show the turn menu, highlight available units etc
        
        // Keep track of how many units need to make a move, once they've all finished, change the state. This could
        // be monitored in the unit manager or the units themselves.
    }

    private void DatabaseNotLoaded() {
        Debug.LogError("Database not loaded");
    }

    public void SetLoginPlayerUnit(Unit unit) {
        if (unit == null) {
            Debug.LogError("Unit is null");
            return;
        }

        LoginPlayerUnit = unit;
        if (LoginPlayerUnit != null) 
        {
            ExampleUnitManager.Instance.LogInPlayerUnit = ExampleUnitManager.Instance.
                GetPlayerUnit(LoginPlayerUnit.id);
        }

        ExampleUnitManager.Instance.LogInPlayerUnit.isLogInPlayerUnit = true;

        // Highlight the tiles the login player can move to
        var LogInUnit = ExampleUnitManager.Instance.LogInPlayerUnit;
        GridManager.Instance.GetMovableTiles(LogInUnit, highlight:true);

// TODO: This is commented out because it bugs...
        // List<Vector2> resetHighlight = ExampleUnitManager.Instance.LogInPlayerUnit.PossibleMoves;
        // if (ExampleUnitManager.Instance.LogInPlayerUnit.PossibleMoves.Count > 0) {
        //     GridManager.Instance.UnhighlightTiles(
        //         ExampleUnitManager.Instance.LogInPlayerUnit.PossibleMoves);
        // }
        

        // GridManager.Instance.GetMovableTiles(
        //     ExampleUnitManager.Instance.LogInPlayerUnit, highlight:true);
        MainMenuScreen.Instance.CurrPlayerLable.text = "- " + LoginPlayerUnit.name + " -";
    }
}



/// <summary>
/// This is obviously an example and I have no idea what kind of game you're making.
/// You can use a similar manager for controlling your menu states or dynamic-cinematics, etc
/// </summary>
[Serializable]
public enum GameState {
    Loading = 0,
    Starting = 1,
    SpawningPlayers = 2,
    SpawningEnemies = 3,
    PlayerTurn = 4,
    EnemyTurn = 5,
    Win = 6,
    Lose = 7,
}