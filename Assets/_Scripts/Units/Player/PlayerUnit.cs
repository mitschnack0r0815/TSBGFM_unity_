using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : BaseUnit {
    public bool isLogInPlayerUnit;

    public List<Vector2> PossibleMoves;

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

    public override void ExecuteMove() {
        // Perform tarodev specific animation, do damage, move etc.
        // You'll obviously need to accept the move specifics as an argument to this function. 
        // I go into detail in the Grid Game #2 video
        base.ExecuteMove(); // Call this to clean up the move
    }
}
