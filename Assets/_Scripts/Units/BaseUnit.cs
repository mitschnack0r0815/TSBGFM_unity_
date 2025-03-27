using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This will share logic for any unit on the field. Could be friend or foe, controlled or not.
/// Things like taking damage, dying, animation triggers etc
/// </summary>
public class BaseUnit : MonoBehaviour {
    public Tile OccupiedTile;
    public Character Character;
    Animator m_Animator;

    private bool _moved = false;

    protected virtual void Awake()
    {
        // Get the Animator attached to the GameObject you are intending to animate.
        m_Animator = GetComponentInChildren<Animator>();

        if (m_Animator == null)
        {
            Debug.LogError("Animator component not found on BaseUnit or its children.");
        }
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