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

    public void SetUnit(BaseUnit unit) {
        unit.transform.position = transform.position;
        unit.transform.position += new Vector3(0, 0.3f, 0);
    }
}