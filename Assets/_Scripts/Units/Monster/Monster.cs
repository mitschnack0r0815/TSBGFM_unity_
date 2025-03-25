using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : BaseMonster {

    protected override void Awake()
    {
        // Call the base class's Awake method
        base.Awake();

        // Additional logic for PlayerUnit
        Debug.Log("PlayerUnit Awake called.");
        
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }

    void Start() {
        // Example usage of a static system
    }
}
