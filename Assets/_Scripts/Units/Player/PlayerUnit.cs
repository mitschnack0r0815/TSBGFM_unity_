using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : BaseUnit {
    public bool isLogInPlayerUnit;

    public List<Vector2> PossibleMoves;

    public List<Vector2> PossibleAttacks;

    protected override void Awake()
    {
        // Call the base class's Awake method
        base.Awake();
    }

    void Start() {
        transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
    }

    void Update()
    {
        if (isLogInPlayerUnit) {
            // Set the camera to the login player
            ExampleGameManager.Instance.Cam.transform.position = 
                ExampleUnitManager.Instance.LogInPlayerUnit.
                transform.position + new Vector3(0, 0, -10);
        }
    }

    public override void OnMouseDown() {
        // Only allow interaction when it's the hero turn
        if (ExampleGameManager.Instance.State != GameState.PlayerTurn) return;

        if (ActionsLeft <= 0 || this == ExampleUnitManager.Instance.LogInPlayerUnit) return;

        if (this.Unit.faction != ExampleGameManager.Instance.LoginPlayer.faction) {
            Debug.Log("You can't control this unit!");
            return;
        }

        if (ExampleUnitManager.Instance.LogInPlayerUnit.actionStartPosition.x != 
            ExampleGameManager.Instance.LoginUnit.position.x ||
            ExampleUnitManager.Instance.LogInPlayerUnit.actionStartPosition.y != 
            ExampleGameManager.Instance.LoginUnit.position.y) 
        {
            Debug.Log("Confirm your move first!");
            return;
        }

        ExampleGameManager.Instance.SetLoginPlayerUnit(this.Unit);
        Debug.Log("Unit " + this.name + " clicked");
            
        // Show movement/attack options

        base.OnMouseDown(); // Call the base class's OnMouseDown method
    }

    public override void ExecuteMove() {
        Debug.Log("PlayerUnit move executed by " + this.name);
        bool didMove = false;

        // If the move was an actual movement and not a click on the same tile, 
        // set the action start position to the current position
        if (ExampleUnitManager.Instance.LogInPlayerUnit.actionStartPosition.x != 
            ExampleGameManager.Instance.LoginUnit.position.x ||
            ExampleUnitManager.Instance.LogInPlayerUnit.actionStartPosition.y != 
            ExampleGameManager.Instance.LoginUnit.position.y) 
        {
            this.actionStartPosition = new Vector2(this.Unit.position.x, this.Unit.position.y);
            didMove = true;
        }

        if (didMove) {
            ActionsLeft--;
        } else {
            Debug.Log("No movement detected, not executing move.");
            return;
        }

        if (ActionsLeft <= 0) {
            GridManager.Instance.UnhighlightTiles();
        } else {
            // Highlight the tiles the login player can move to or attack
            GridManager.Instance.UnhighlightTiles();
            GridManager.Instance.GetMovableTiles(this, highlight:true);
            GridManager.Instance.GetAttackableTiles(this, highlight:true);
        }

        MainMenuScreen.Instance.SetUnitInfo(this); // Set the unit info in the UI
        base.ExecuteMove(); // Call this to clean up the move
    }
}
