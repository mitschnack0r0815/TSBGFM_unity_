using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// This will share logic for any unit on the field. Could be friend or foe, controlled or not.
/// Things like taking damage, dying, animation triggers etc
/// </summary>
public class BaseUnit : MonoBehaviour {
    [SerializeField] public Vector3 OffsetPosition = new(0, 0);
    public Tile OccupiedTile;
    public Unit Unit;
    Animator m_Animator;
    private bool _movedAnimation = false;

    public int ActionsLeft = 2; // This will be set to the number of actions a unit can take in a turn.
    private bool _canMove = false;

    public Vector2 actionStartPosition = new(0, 0); // This will be set to the position of the unit at the start of the turn.
    private void OnDestroy() => ExampleGameManager.OnBeforeStateChanged -= OnStateChanged;

    private void OnStateChanged(GameState newState) {
        if (newState == GameState.PlayerTurn) _canMove = true;
    }

    protected virtual void Awake()
    {
        // Get the Animator attached to the GameObject you are intending to animate.
        m_Animator = GetComponentInChildren<Animator>();

        GetSpecificSprites("front");

        if (m_Animator == null)
        {
            Debug.LogError("Animator component not found on BaseUnit or its children.");
        }

        ExampleGameManager.OnBeforeStateChanged += OnStateChanged;
    }

    public virtual void OnMouseDown() {
        // Only allow interaction when it's the hero turn

        // Show movement/attack options

        // Eventually either deselect or ExecuteMove(). You could split ExecuteMove into multiple functions
        // like Move() / Attack() / Dance()

        // Debug.Log("Unit " + Unit.name + " clicked");
    }

    public virtual void ExecuteMove() 
    {
        
    }

    public void MoveUnit(Tile targetTile)
    {
        if (targetTile == null || ActionsLeft <= 0)
        {
            Debug.LogError("Target tile is null");
            return;
        }

        if (OccupiedTile != null)
        {
            OccupiedTile.IsOccupied = false;
            OccupiedTile.OccupiedUnit = null;
        }

        targetTile.IsOccupied = true;
        targetTile.OccupiedUnit = this;
        OccupiedTile = targetTile;
        targetTile.SetUnit(this);
    }

    public void ToogleMovedAnimation()
    {
        if (_movedAnimation)
        {
            _movedAnimation = false;
            GetSpecificSprites("front");
            m_Animator.SetBool("Moved", false);
        }
        else
        {
            _movedAnimation = true;
            GetSpecificSprites("side");
            m_Animator.SetBool("Moved", true);
        }
    }

    public void AttackUnit(BaseUnit targetUnit)
    {
        if (targetUnit == null)
        {
            Debug.LogError("Target unit is null");
            return;
        }

        // Perform attack logic here, e.g., reduce target unit's health
        // Example: targetUnit.TakeDamage(attackPower);
    }

    public void RetreatUnit()
    {
        // Perform retreat logic here, e.g., move the unit off the battlefield or to a safe location
        // Example: transform.position = retreatPosition;
    }

    public void WaitUnit()
    {
        // Perform wait logic here, e.g., disable the unit or make it inactive
        // Example: gameObject.SetActive(false);
    }

    private void GetSpecificSprites(string childFolder)
    {
        // ListChildNames();

        foreach (Transform child in transform)
        {
            var spriteRenderers = child.GetComponentsInChildren<SpriteRenderer>();

            // Check if the child's name is not "side" (case-insensitive and trimmed)
            if (!string.Equals(child.name.Trim(), childFolder, System.StringComparison.OrdinalIgnoreCase))
            {
                // Get all SpriteRenderer components in the child and its nested children
                foreach (var spriteRenderer in spriteRenderers)
                {
                    spriteRenderer.enabled = false; // Make all sprites invisible
                    // if (Unit.position.y != 0) {
                    //     // Increase the sorting order by y position
                    //     spriteRenderer.sortingOrder += (DatabaseManager.Instance.GameStatus.board.y - Unit.position.y) *100; 
                    // }
                }
            } else {
                // If the child is named "side", we want to keep it visible
                foreach (var spriteRenderer in spriteRenderers)
                {
                    spriteRenderer.enabled = true; // Make all sprites visible
                    // if (Unit.position.y != 0) {
                    //     // Increase the sorting order by y position
                    //     spriteRenderer.sortingOrder += (DatabaseManager.Instance.GameStatus.board.y - Unit.position.y) *100; 
                    // }
                }
            }
        }
    }

    public void ChangeSortingOrder(float amount) {
        foreach (Transform child in transform)
        {
            var spriteRenderers = child.GetComponentsInChildren<SpriteRenderer>();
            foreach (var spriteRenderer in spriteRenderers)
            {
                    // Increase the sorting order by y position
                    spriteRenderer.sortingOrder += 100 * (int)amount; 
            }
        }
    }

    public void FlipAllSprites(bool trueOrFalse)
    {
        foreach (Transform child in transform)
        {
            var spriteRenderers = child.GetComponentsInChildren<SpriteRenderer>();
            foreach (var spriteRenderer in spriteRenderers)
            {
                spriteRenderer.flipX = trueOrFalse; // Flip the sprite
            }
        }
    }
    
}