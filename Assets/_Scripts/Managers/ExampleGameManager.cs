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

    [SerializeField] private Transform _cam;

    public GameState State { get; private set; }

    public Character LogInPlayer { get; set; }

    private DatabaseManager _dbManager;

    // Kick the game off with the first state
    void Start() {
        _dbManager = DatabaseManager.Instance;
        if (_dbManager == null) {
            Debug.LogError("DatabaseManager not found");
        }
        // Start a coroutine to wait for the db data to be loaded
        StartCoroutine(WaitForDataAndChangeState());
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

    private IEnumerator WaitForDataAndChangeState() {
        if (_dbManager != null) {
            // Wait until the data is loaded
            while (!_dbManager.IsDataLoaded) {
                yield return null;
            }           
            ChangeState(GameState.Starting);            
        } else {
            Debug.LogError("DatabaseManager not found");
        }
    }

    private void HandleStarting() {
        if (!_dbManager.IsDataLoaded) DatabaseNotLoaded();

        // For now, set the first character to the login player
        LogInPlayer = _dbManager.Characters[0]; 

        // Do some start setup, could be environment, cinematics etc
        GridManager.Instance.GenerateGrid(_dbManager.GameStatus.board);

        ChangeState(GameState.SpawningHeroes);
    }


    private void HandleSpawningHeroes() {
        if (!_dbManager.IsDataLoaded) DatabaseNotLoaded();
        
        ExampleUnitManager.Instance.SpawnPlayers(_dbManager.GameStatus.chars);

        var LogInUnit = ExampleUnitManager.Instance.GetLogInUnit;
        _cam.transform.position = LogInUnit.transform.position + new Vector3(0, 0, -10);
        
        // Highlight the movable tiles
        // var movableTiles = new Vector2[2] { 
        //     new Vector2(LogInPlayer.position.x + 1, LogInPlayer.position.y), 
        //     new Vector2(LogInPlayer.position.x, LogInPlayer.position.y + 1) 
        //     };
        var logInPlayerPos = new Vector2(LogInPlayer.position.x, LogInPlayer.position.y);
        var movableList = GridManager.Instance.GetMovableTiles(logInPlayerPos);
        GridManager.Instance.HighlightMovableTiles(movableList);

        ChangeState(GameState.SpawningEnemies);
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

    private void DatabaseNotLoaded() {
        Debug.LogError("Database not loaded");
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