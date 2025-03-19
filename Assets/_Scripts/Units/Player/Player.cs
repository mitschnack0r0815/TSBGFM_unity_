using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : BasePlayer {

    void Awake() {
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }

    void Start() {
        // Example usage of a static system
    }
    
    public override void ExecuteMove() {
        // Perform tarodev specific animation, do damage, move etc.
        // You'll obviously need to accept the move specifics as an argument to this function. 
        // I go into detail in the Grid Game #2 video
        base.ExecuteMove(); // Call this to clean up the move
    }
}
