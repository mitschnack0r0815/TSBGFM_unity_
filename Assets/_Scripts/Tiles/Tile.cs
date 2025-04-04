using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
 
public class Tile : MonoBehaviour {
    // [SerializeField] private Color _baseColor, _offsetColor;
    // [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;

    [SerializeField] public GameObject MovableHighlight;

    [SerializeField] public GameObject MouseHighlight;

    [SerializeField] public bool isMovable = false;

    [SerializeField] public bool IsOccupied = false;

    [SerializeField] public BaseUnit OccupiedUnit = null;

    public Vector2 pos;
 
    public void Init(bool isOffset) {
        // _renderer.color = isOffset ? _offsetColor : _baseColor;
    }
 
    void OnMouseEnter() {
        if (isMovable) {
            MouseHighlight.SetActive(true);
        }
    }
 
    void OnMouseExit()
    {
        if (isMovable) {
            MouseHighlight.SetActive(false);
        }
    }

    void OnMouseDown() {
        if (isMovable) {
            if (ExampleUnitManager.Instance.LogInPlayerUnit.PossibleMoves.Count > 0 &&
                ExampleUnitManager.Instance.LogInPlayerUnit.PossibleMoves.Contains(this.pos)) 
            {
                ExampleUnitManager.Instance.LogInPlayerUnit.MoveUnit(this);
                ExampleUnitManager.Instance.LogInPlayerUnit.Unit.position.x = (int)this.pos.x;
                ExampleUnitManager.Instance.LogInPlayerUnit.Unit.position.y = (int)this.pos.y;
            }
        }
    }

    // public void PlaceUnit(BaseUnit unit) {
    //     IsOccupied = true;
    //     OccupiedUnit = unit;
    //     unit.OccupiedTile = this;
    //     unit.transform.position = transform.position + unit.OffsetPosition;
    //     unit.OccupiedTile = this;
    // }

    public void SetUnit(BaseUnit unit) {
        int amount = Mathf.FloorToInt(unit.Unit.position.y - this.pos.y);
        unit.ChangeSortingOrder(amount);
        StartCoroutine(SmoothMove(unit, transform.position + unit.OffsetPosition, 0.5f));
    }

    private IEnumerator SmoothMove(BaseUnit unit, Vector3 targetPosition, float duration) {
        Vector3 startPosition = unit.transform.position;
        float elapsedTime = 0f;

        unit.ToogleMovedAnimation();
        while (elapsedTime < duration) {
            // Interpolate between the start and target positions
            unit.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            if (startPosition.x > targetPosition.x) {
                unit.FlipAllSprites(true);
            } else {
                unit.FlipAllSprites(false);
            }
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }
        // unit.transform.position = Mathf.MoveTowards(startPosition, targetPosition, duration);
        unit.ToogleMovedAnimation();
        unit.FlipAllSprites(false);

        // Ensure the unit ends exactly at the target position
        unit.transform.position = targetPosition;
    }
}