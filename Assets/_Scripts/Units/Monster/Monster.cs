using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : BaseMonster {

    void Awake() {
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }

    void Start() {
        // Example usage of a static system
    }
}
