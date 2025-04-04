using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// This will share logic for any unit on the field. Could be friend or foe, controlled or not.
/// Things like taking damage, dying, animation triggers etc
/// </summary>
public class BaseUnit : MonoBehaviour {

    public static event Action<UnitState> OnBeforeUnitStateChanged;
    public static event Action<UnitState> OnAfterUnitStateChanged;
    public bool ActiveRoutine = false;
    [SerializeField] public Vector3 OffsetPosition = new(0, 0);

    public List<Vector2> PossibleMoves;
    public List<Vector2> FirstWeaponAttacks;
    public List<Vector2> SeccondWeaponAttacks;
    public Tile OccupiedTile;
    public Unit Unit;
    public Animator m_Animator;
    private bool _movedAnimation = false;
    private BaseUnit _targetUnit = null;
    public int ActionsLeft = 2; // This will be set to the number of actions a unit can take in a turn.

    [SerializeField] private bool _startWithSideView = false; // This will be set to true if the unit should start with a side view.
    public Vector2 actionStartPosition = new(0, 0); // This will be set to the position of the unit at the start of the turn.
    public class WantsToAttack
    {
        public bool IsRanged = false;
        public Vector2 TargetPosition = new Vector2(0, 0);
    }

    public WantsToAttack AttackIntent = new WantsToAttack();

    private void OnValidate()
    {
        GetSpecificSprites("side");
    }

    protected virtual void Awake()
    {
        // Get the Animator attached to the GameObject you are intending to animate.
        m_Animator = GetComponentInChildren<Animator>();
        // Debug.Log($"Animator assigned to {gameObject.name}: {m_Animator}");

        if (_startWithSideView) {
            GetSpecificSprites("side");
        } else {
            GetSpecificSprites("front");
        }

        if (m_Animator == null)
        {
            Debug.LogError("Animator component not found on BaseUnit or its children.");
        }
    }

    public UnitState State { get; private set; } // Add this property to store the current state

    public void ChangeState(UnitState newState) {
        OnBeforeUnitStateChanged?.Invoke(newState);

        State = newState;
        switch (newState) {
            case UnitState.Ready:
                break;
            case UnitState.Attacking:
                break;
            case UnitState.Done:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnAfterUnitStateChanged?.Invoke(newState);
    }

    public virtual void OnMouseDown() {
        if (ActiveRoutine) return; // Don't allow interaction if the unit is already moving
        // Only allow interaction when it's the hero turn

        // Show movement/attack options

        // Eventually either deselect or ExecuteMove(). You could split ExecuteMove into multiple functions
        // like Move() / Attack() / Dance()

        // Debug.Log("Unit " + Unit.name + " clicked");
    }

    public virtual void ExecuteMove() 
    {
        if (ActiveRoutine) return; // Don't allow interaction if the unit is already moving
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

    public void AttackUnit(BaseUnit targetUnit, bool isRanged)
    {
        if (targetUnit == null)
        {
            Debug.LogError("Target unit is null");
            return;
        }

        if (isRanged)
        {
            MainMenuScreen.Instance.UpdateGeneralInfo("Attacked " + targetUnit.name + " with ranged attack...", true);
            StartCoroutine(UnitCombat(this, targetUnit, isRanged));
        }
        else
        {
            MainMenuScreen.Instance.UpdateGeneralInfo("Attacked " + targetUnit.name + " with meele attack...", true);
            StartCoroutine(UnitCombat(this, targetUnit, isRanged));
        }
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

    public void GetSpecificSprites(string childFolder)
    {
        if (childFolder == "side") {
            m_Animator.SetBool("SideIdle", true);
        } else { 
            m_Animator.SetBool("SideIdle", false);
        }

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
                }
            } else {
                // If the child is named "side", we want to keep it visible
                foreach (var spriteRenderer in spriteRenderers)
                {
                    spriteRenderer.enabled = true; // Make all sprites visible
                }
            }
        }
    }

    public void ChangeSortingOrder(int amount, bool staticValue = false) {
        foreach (Transform child in transform)
        {
            var spriteRenderers = child.GetComponentsInChildren<SpriteRenderer>();
            foreach (var spriteRenderer in spriteRenderers)
            {
                // Increase the sorting order by y position
                if (staticValue) {
                    spriteRenderer.sortingOrder += amount; 
                } else {
                    spriteRenderer.sortingOrder += 100 * amount; 
                }
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

    public IEnumerator UnitCombat(BaseUnit attacker, BaseUnit target, bool isRanged)
    {
        if (attacker == null || target == null)
        {
            throw new ArgumentNullException("Attacker or target unit cannot be null.");
        }

        _targetUnit = target;
        ActiveRoutine = true;

        Weapon attackerWeapon = isRanged ? attacker.Unit.weapons.second : attacker.Unit.weapons.first;
        int attackStrength = attackerWeapon.strength;
        int targetArmor = target.Unit.armor;

        for (int i = 0; i < attackerWeapon.attacks; i++)
        {


            int roll = CombatLib.RollDice();
            int hitPoints = 0;
            if (roll == 6) {
                hitPoints = attackerWeapon.critPower;
            }  else if (attackStrength > targetArmor && roll >= 3) {
                hitPoints = attackerWeapon.attackPower;
            } else if (attackStrength == targetArmor && roll >= 4) {
                hitPoints = attackerWeapon.attackPower; 
            } else if (roll >= 5) {
                hitPoints = attackerWeapon.attackPower; 
            } else {
                hitPoints = 0;
            }

            if (hitPoints > 0) {
                // Trigger the attack animation
                m_Animator.SetTrigger("AttackMelee");
                // Wait for the animation to finish using a coroutine
                yield return new WaitUntil(() => m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f );                
                yield return new WaitWhile(() => m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f );
            }

            target.Unit.life -= hitPoints;
            Debug.Log(target.Unit.name + " rolled " + roll + " hit by " + hitPoints + "... attack " + i);
        }

        ActiveRoutine = false;
    }

    public IEnumerator PlayAndWaitForAnimation(string animationName)
    {
        if (m_Animator == null)
        {
            Debug.LogError("Animator is not assigned!");
            yield break;
        }

        // Play the animation
        m_Animator.Play(animationName);

        // Wait for the animation to finish
        yield return new WaitUntil(() => m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
    }

    // Used by the AnimationEvent to trigger the hit animation
    private void TriggetTargetHitAnimation()
    {
        // Trigger the hit animation
        _targetUnit.m_Animator.SetTrigger("GotHit");
        // Wait for the animation to finish using a coroutine
    }
    
}

[Serializable]
public enum UnitState {
    Ready = 0,
    Attacking = 1,
    Done = 2,
}
