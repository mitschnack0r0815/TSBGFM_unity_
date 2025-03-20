using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
 
public class GridManager : MonoBehaviour {
    public static GridManager Instance; 
    [SerializeField] private Tile _tileLandPrefab;
    [SerializeField] private Tile _tileWaterPrefab;
    [SerializeField] private Tile _tileMountainPrefab;
    [SerializeField] private Tile _tileEmptyPrefab;
    private Dictionary<Vector2, Tile> _tiles;
 
    void Awake() {
        Instance = this;
    }
 
    public void GenerateGrid(Board board) {
        _tiles = new Dictionary<Vector2, Tile>();

        for (int x = 0; x < board.x; x++) {
            /* Start at 1 to avoid overlapping tiles */
            for (int y = 0; y < board.y; y++) {
                float xPos = x ;
                float yPos = y * 0.8f;

                yPos += 1.0f;

                if (y % 2 != 0) {
                    xPos += 0.5f;
                }

                Tile _tilePrefab;
                switch ((TileType)board.map[x][y]) {
                    case TileType.Land:
                        _tilePrefab = _tileLandPrefab;
                        break;
                    case TileType.Water:
                        _tilePrefab = _tileWaterPrefab;
                        break;
                    case TileType.Mountain:
                        _tilePrefab = _tileMountainPrefab;
                        break;
                    default:
                        _tilePrefab = _tileEmptyPrefab;
                        break;
                }
                if (_tilePrefab) {
                    var spawnedTile = Instantiate(_tilePrefab, new Vector3(xPos, yPos), Quaternion.identity);
                    spawnedTile.name = $"Tile {x} {y}";
                    _tiles[new Vector2(x, y)] = spawnedTile;
                }
            }
        }
    }
 
    public Tile GetTileAtPosition(Vector2 pos) {
        if (_tiles.TryGetValue(pos, out var tile)) return tile;
        return null;
    }    

    public List<Vector2> GetMovableTiles(Vector2 pos) {
        var tile = GetTileAtPosition(pos);
        if (tile == null) return null;
        var movableTiles = new List<Vector2>();
        var x = (int)pos.x;
        var y = (int)pos.y;
        var directions = new Vector2[] { 
                new Vector2(1, 0),
                new Vector2(-1, 0),
                new Vector2(0, 1),
                new Vector2(0, -1),
                new Vector2(-1, 1),
                new Vector2(-1, -1)
            };
        foreach (var dir in directions) {
            var newPos = pos + dir;
            var newTile = GetTileAtPosition(newPos);
            if (newTile.isMovable) {
                movableTiles.Add(newPos);
            }
        }
        return movableTiles;
    }

    public void HighlightMovableTiles(List<Vector2> tilePositions) {
        foreach (var tilePos in tilePositions) {
            var tile = GetTileAtPosition(tilePos);
            if (tile == null) continue;
            tile.MovableHighlight.SetActive(true);
            tile.transform.position += new Vector3(0, 0.1f, 0);
        }
    }
}

enum TileType {
    Land = 1,
    Water = 2,
    Mountain = 3
}