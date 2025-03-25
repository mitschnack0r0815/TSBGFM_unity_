using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This will share logic for any unit on the field. Could be friend or foe, controlled or not.
/// Things like taking damage, dying, animation triggers etc
/// </summary>
public class BaseUnit : MonoBehaviour {
    public string UnitName;
    public Tile OccupiedTile;
    public Faction Faction;

    public Character Character;

    public Vector2 Position;

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
}