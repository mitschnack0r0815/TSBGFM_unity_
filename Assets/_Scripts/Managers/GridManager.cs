using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
 
public class GridManager : MonoBehaviour {
    public static GridManager Instance; 
    [SerializeField] private Tile _tilePrefab;
 
    [SerializeField] private Transform _cam;
 
    private Dictionary<Vector2, Tile> _tiles;
 
    void Awake() {
        Instance = this;
    }
 
    public void GenerateGrid(int width, int height) {
        _tiles = new Dictionary<Vector2, Tile>();

        /* TODO: This logic is for rectangles, we need to change it to hexagons */
        for (int x = 0; x < width; x++) {
            /* Start at 1 to avoid overlapping tiles */
            for (int y = 0; y < height; y++) {
                float xPos = x ;
                float yPos = y * 0.8f;

                yPos += 1.0f;

                if (y % 2 != 0) {
                    xPos += 0.5f;
                }

                var spawnedTile = Instantiate(_tilePrefab, new Vector3(xPos, yPos), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";

                var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                // spawnedTile.Init(isOffset);

                _tiles[new Vector2(x, y)] = spawnedTile;
            }
        }
 
        _cam.transform.position = new Vector3((float)width/2 -0.5f, (float)height / 2 - 0.5f,-10);
    }
 
    public Tile GetTileAtPosition(Vector2 pos) {
        if (_tiles.TryGetValue(pos, out var tile)) return tile;
        return null;
    }
}