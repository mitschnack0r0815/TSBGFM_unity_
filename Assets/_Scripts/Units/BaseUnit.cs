using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// This will share logic for any unit on the field. Could be friend or foe, controlled or not.
/// Things like taking damage, dying, animation triggers etc
/// </summary>
public class BaseUnit : MonoBehaviour {
    public Tile OccupiedTile;
    public Unit Unit;
    Animator m_Animator;

    private int _actionsLeft = 2; // This will be set to the number of actions a unit can take in a turn.
    private bool _moved = false;
    private bool _canMove = false;

    private void OnDestroy() => ExampleGameManager.OnBeforeStateChanged -= OnStateChanged;

    private void OnStateChanged(GameState newState) {
        if (newState == GameState.PlayerTurn) _canMove = true;
    }

    protected virtual void Awake()
    {
        // Get the Animator attached to the GameObject you are intending to animate.
        m_Animator = GetComponentInChildren<Animator>();

        if (m_Animator == null)
        {
            Debug.LogError("Animator component not found on BaseUnit or its children.");
        }

        ExampleGameManager.OnBeforeStateChanged += OnStateChanged;
    }

    private void OnMouseDown() {
        // Only allow interaction when it's the hero turn
        if (ExampleGameManager.Instance.State != GameState.PlayerTurn) return;

        // Don't move if we've already moved
        if (!_canMove) return;

        // Show movement/attack options

        // Eventually either deselect or ExecuteMove(). You could split ExecuteMove into multiple functions
        // like Move() / Attack() / Dance()

        Debug.Log("Unit clicked");
    }

    public virtual void ExecuteMove() {
        // Override this to do some hero-specific logic, then call this base method to clean up the turn

        _canMove = false;
    }

    public void MoveUnit(Tile targetTile)
    {
        if (targetTile == null)
        {
            Debug.LogError("Target tile is null");
            return;
        }

        //transform.Translate(targetTile.transform.position - transform.position);

        targetTile.SetUnit(this);
    }

    public void ToogleMoved()
    {
        if (_moved)
        {
            _moved = false;
            m_Animator.SetBool("Moved", false);
        }
        else
        {
            _moved = true;
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
}