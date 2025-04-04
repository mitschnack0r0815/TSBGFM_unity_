using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Scriptable Unit")]
public class ScriptableUnit : ScriptableObject {
    public BaseUnit UnitPrefab;

    // [SerializeField] private bool _useSkinColor = false;
    [SerializeField] private Color _skinColor = new Color(1, 1, 1, 1);
    [SerializeField] private Color _teamColor = new Color(1, 1, 1, 1);

    [SerializeField] private bool _flipFace = false;

    // [SerializeField] private System.Collections.Generic.List<Color> colors;

    // [SerializeField] private Transform[] childGameObjects;

    // private void CacheChildGameObjects() {
    //     var children = new System.Collections.Generic.List<Transform>();
    //     foreach (Transform child in UnitPrefab.transform) {
    //         foreach (Transform grandChild in child.transform) {
    //                 children.Add(grandChild);
    //         }
    //     }
    //     childGameObjects = children.ToArray();
    // }

    // private void CacheSkinRenderers(string contains) {
    //     colors = new System.Collections.Generic.List<Color>();
    //     foreach (Transform child in UnitPrefab.transform) {
    //         foreach (Transform grandChild in child.transform) {
    //             if (grandChild.gameObject.name.Contains(contains)) {
    //                 var spriteRenderers = grandChild.GetComponentsInChildren<SpriteRenderer>();
    //                 foreach (var renderer in spriteRenderers) {
    //                     colors.Add(renderer.color);
    //                 }
    //             }
    //         }
    //     }
    //     // You can store the colors in a field if needed, e.g., _skinColors = colors.ToArray();
    // }

    private void OnEnable() {
        Debug.Log("ScriptableUnit OnEnable called " + UnitPrefab.name);
        ChangeSkinColor(_skinColor);
        ChangeSkinColor(_teamColor, "armor");
        if (_flipFace) {
            CacheSkinRenderersFace("eye");
            CacheSkinRenderersFace("mouth");
        }
        // CacheSkinRenderers();
        // CacheChildGameObjects();
    }

    public void ChangeSkinColor(Color newColor, string contains = "_skin") {
        foreach (Transform child in UnitPrefab.transform) {
            foreach (Transform grandChild in child.transform) {
                if (grandChild.gameObject.name.Contains(contains)) {
                    SpriteRenderer[] spriteRenderers = grandChild.GetComponentsInChildren<SpriteRenderer>();
                    foreach (SpriteRenderer spriteRenderer in spriteRenderers) {
                        if (spriteRenderer != null) {
                            spriteRenderer.color = newColor;
                        }
                    }
                }
            }
        }
    }

    private void CacheSkinRenderersFace(string contains) {
        foreach (Transform child in UnitPrefab.transform) {
            foreach (Transform grandChild in child.transform) {
                if (grandChild.gameObject.name.Contains(contains)) {
                    var spriteRenderers = grandChild.GetComponentsInChildren<SpriteRenderer>();
                    foreach (var renderer in spriteRenderers) {
                        renderer.flipY = true; // Flip the sprite vertically
                    }
                }
            }
        }
        // You can store the colors in a field if needed, e.g., _skinColors = colors.ToArray();
    }
}
