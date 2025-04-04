using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : BaseUnit {
    private ExampleUnitManager UnitManager => ExampleUnitManager.Instance;
    private ExampleGameManager GameManager => ExampleGameManager.Instance;
    public bool isLogInPlayerUnit;

    protected override void Awake()
    {
        // Call the base class's Awake method
        base.Awake();
    }

    void Start() {
        // transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
    }

    void Update()
    {
        if (isLogInPlayerUnit) {
            // Set the camera to the login player
            GameManager.Cam.transform.position = 
                UnitManager.LogInPlayerUnit.
                transform.position + new Vector3(0, 1, -10);
        }
    }

    public override void OnMouseDown() {
        // Only allow interaction when it's the hero turn
        if (GameManager.State != GameState.PlayerTurn) return;

        

        if (ActionsLeft <= 0 || this == UnitManager.LogInPlayerUnit) return;

        if (UnitManager.LogInPlayerUnit.actionStartPosition.x != 
            GameManager.LoginUnit.position.x ||
            UnitManager.LogInPlayerUnit.actionStartPosition.y != 
            GameManager.LoginUnit.position.y) 
        {
            Debug.Log("Confirm your move first!");
            MainMenuScreen.Instance.UpdateGeneralInfo("Confirm your move first!", true);
            return;
        }

        // this refelcts the clicked unit here
        if (this.Unit.faction != GameManager.LoginPlayer.faction) {
            UnitManager.LogInPlayerUnit.wantsToAttack = new Vector2(this.Unit.position.x, this.Unit.position.y);
            UnitManager.LogInPlayerUnit.GetSpecificSprites("side");
            if (UnitManager.LogInPlayerUnit.Unit.position.x > this.Unit.position.x) {
                UnitManager.LogInPlayerUnit.FlipAllSprites(true);
            } else {
                UnitManager.LogInPlayerUnit.FlipAllSprites(false);
            }
            return;
        }

        GameManager.SetLoginPlayerUnit(this.Unit);
        Debug.Log("Unit " + this.name + " clicked");
            
        // Show movement/attack options

        base.OnMouseDown(); // Call the base class's OnMouseDown method
    }

    public override void ExecuteMove() {
        Debug.Log("PlayerUnit move executed by " + this.name);
        bool didMove = false;

        // If the move was an actual movement and not a click on the same tile, 
        // set the action start position to the current position
        if (UnitManager.LogInPlayerUnit.actionStartPosition.x != 
            GameManager.LoginUnit.position.x ||
            UnitManager.LogInPlayerUnit.actionStartPosition.y != 
            GameManager.LoginUnit.position.y) 
        {
            this.actionStartPosition = new Vector2(this.Unit.position.x, this.Unit.position.y);
            didMove = true;
        }

        if (!didMove) {
            // attack ?
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
            GridManager.Instance.UpdateUnitViewTiles(this);
        }

        MainMenuScreen.Instance.SetUnitInfo(this); // Set the unit info in the UI
        base.ExecuteMove(); // Call this to clean up the move
    }
}
