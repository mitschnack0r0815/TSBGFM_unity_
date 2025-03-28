using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class Tile : MonoBehaviour {
    // [SerializeField] private Color _baseColor, _offsetColor;
    // [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;

    [SerializeField] public GameObject MovableHighlight;

    [SerializeField] public GameObject MouseHighlight;

    [SerializeField] public bool isMovable = false;

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

    public void SetUnit(BaseUnit unit) {
        StartCoroutine(SmoothMove(unit, transform.position + new Vector3(-0.3f, 1.2f, 0), 0.5f));
    }

    private IEnumerator SmoothMove(BaseUnit unit, Vector3 targetPosition, float duration) {
        Vector3 startPosition = unit.transform.position;
        float elapsedTime = 0f;

        unit.ToogleMoved();
        while (elapsedTime < duration) {
            // Interpolate between the start and target positions
            unit.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }
        unit.ToogleMoved();
        
        // Ensure the unit ends exactly at the target position
        unit.transform.position = targetPosition;
    }
}