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
    public string LoginPlayerName { get; set; } = TestData.loginPlayer;
    public Player LoginPlayer { get; set; }
    public Unit LoginUnit { get; set; }

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
        Debug.LogWarning($"New state: {newState}"); 

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
            case GameState.SpawningUnits:
                HandleUnits();
                break;
            case GameState.ReplayEnemyTurns:
                ChangeState(GameState.PlayerTurn);
                break;
            case GameState.PlayerTurn:
                HandleTurn();
                break;
            case GameState.EndTurn:
                break;
            case GameState.Win:
                break;
            case GameState.Lose:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnAfterStateChanged?.Invoke(newState);
    }

    private IEnumerator HandleLoading() {
        if (_dbManager != null) {
            // Wait until the data is loaded
            while (!_dbManager.IsDataLoaded) {
                yield return null;
            }           

            MainMenuScreen.Instance.GenerateUI();
            GridManager.Instance.GenerateGrid(_dbManager.GameStatus.board);

            // Set the player dropdown
            var dropdown = MainMenuScreen.Instance.PlayerDropdown;
            var playerNameList = _dbManager.GameStatus.players.Select(u => u.playerName).ToList();
            dropdown.choices = playerNameList;
            dropdown.value = "Choose Player";
            MainMenuScreen.OnPlayerDropdownChoose += () => {
                Debug.Log("Player dropdown changed: " + dropdown.value);
                LoginPlayerName = dropdown.value;
                // SetLoginPlayerUnit(ExampleUnitManager.Instance.GetPlayer(unitId).Unit);
            };

            ChangeState(GameState.Starting);  
        } else {
            Debug.LogError("DatabaseManager not found");
        }
    }

    private void HandleStarting() {
        if (!_dbManager.IsDataLoaded) DatabaseNotLoaded();

        ChangeState(GameState.SpawningUnits);
    }


    private void HandleUnits() {
        if (!_dbManager.IsDataLoaded) {
            DatabaseNotLoaded();
            return;
        }

        // Find the player with the name matching LoginPlayer
        LoginPlayer = _dbManager.GameStatus.players.FirstOrDefault(p => p.playerName == LoginPlayerName);

        if (ExampleUnitManager.Instance.PlayerUnits.Count > 0) {
            Debug.Log("HandleUnits: Units already spawned");
        } else {
            Debug.LogWarning("HandleUnits: Spawning fresh Units");
            // Spawn the player units
            ExampleUnitManager.Instance.SpawnPlayerUnits(_dbManager.GameStatus.players);
        }

        ExampleUnitManager.Instance.LoginPlayerPlayerUnits.Clear();
        foreach (var loginPlayerUnit in LoginPlayer.units)
        {
            ExampleUnitManager.Instance.LoginPlayerPlayerUnits.Add(ExampleUnitManager.Instance.PlayerUnits
                .FirstOrDefault(u => u.Unit.id == loginPlayerUnit.id));
        }

        if (LoginPlayer != null) {
            SetLoginPlayerUnit(LoginPlayer.units[0]);
        } else {
            Debug.LogError("Login player not found");
        }

        Debug.Log("SetLoginPlayerUnit: " + ExampleUnitManager.Instance.LogInPlayerUnit.Unit.name);
        ExampleUnitManager.Instance.LogInPlayerUnit.isLogInPlayerUnit = true;

        ChangeState(GameState.ReplayEnemyTurns);
    }

    private void HandleTurn() {
        // If you're making a turn based game, this could show the turn menu, highlight available units etc
        

        Debug.Log("Player " + LoginPlayerName + "'s turn with Unit " + ExampleUnitManager.Instance.LogInPlayerUnit.Unit.name +
            " and ID " + ExampleUnitManager.Instance.LogInPlayerUnit.Unit.id);
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

        LoginUnit = unit;
        if (LoginUnit != null) 
        {
            ExampleUnitManager.Instance.LogInPlayerUnit = ExampleUnitManager.Instance.
                GetPlayerUnit(LoginUnit.id);
        }

        ExampleUnitManager.Instance.LogInPlayerUnit.isLogInPlayerUnit = true;

        // Highlight the tiles the login player can move to or attack
        var LogInUnit = ExampleUnitManager.Instance.LogInPlayerUnit;
        GridManager.Instance.UpdateUnitViewTiles(LogInUnit);

        MainMenuScreen.Instance.CurrPlayerLable.text = "- " + LoginPlayerName + " -";
        MainMenuScreen.Instance.SetUnitInfo(ExampleUnitManager.Instance.LogInPlayerUnit);
    }

    public void SwitchLoginPlayerUnit(int indexJump) {
        // dir = -1 for left, 1 for right
        if (LoginPlayer == null || LoginPlayer.units == null || LoginPlayer.units[0] == null) {
            Debug.LogError("No units available to switch.");
            return;
        }

        int newIndex = -1;
        for (int i = 0; i < LoginPlayer.units.Length; i++) {
            if (LoginPlayer.units[i] == LoginUnit) {
                // Found the current unit, now switch to the next one
                newIndex = (i + indexJump) % LoginPlayer.units.Length;
                if (newIndex < 0) newIndex += LoginPlayer.units.Length; // Wrap around if negative
                Debug.Log("Switching to unit: " + LoginPlayer.units[newIndex].name);
                break;
            }
        }

        if (newIndex == -1) {
            Debug.LogError("No unit found to switch to.");
            return;
        }

        // Set the new LoginPlayerUnit
        SetLoginPlayerUnit(LoginPlayer.units[newIndex]);
    }

    public void EndTurn() {
        // Check if the player has finished their turn
        for (int i = 0; i < LoginPlayer.units.Length; i++) {
            if (ExampleUnitManager.Instance.LoginPlayerPlayerUnits[i].ActionsLeft > 0) {
                Debug.Log("Player has not finished their turn yet.");
            }
        }

        if (DatabaseManager.Instance.OfflineMode) {
            Debug.Log("Offline mode: No database submission.");

            // Reset the actions for all player units in offline mode
            // Get this from the DB in non-offline mode        
            for (int i = 0; i < LoginPlayer.units.Length; i++) {
                ExampleUnitManager.Instance.LoginPlayerPlayerUnits[i].ActionsLeft = 2; // Reset actions for the next turn
            }

            for (int i = 0; i < _dbManager.GameStatus.turnList.Length; i++) {
                if (_dbManager.GameStatus.turnList[i] == LoginPlayerName) {
                    int nextIndex = (i + 1) % _dbManager.GameStatus.turnList.Length;
                    LoginPlayerName = _dbManager.GameStatus.turnList[nextIndex];
                    Debug.Log("Start next turn: " + LoginPlayerName);
                    ChangeState(GameState.Starting);
                    break;
                }
            }
        }

        // DatabaseManager.Instance.SubmitTurn();

        // Change the game state to the next turn
        ChangeState(GameState.EndTurn);
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
    SpawningUnits = 2,
    ReplayEnemyTurns = 3,
    PlayerTurn = 4,
    EndTurn = 5,
    Win = 6,
    Lose = 7,
}