using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
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
        Transform parentTransform = new GameObject("Tiles").transform;

        for (int x = 0; x < board.x; x++) {
            /* Start at 1 to avoid overlapping tiles */
            for (int y = 0; y < board.y; y++) {
                float xPos = x ;
                float yPos = y * 0.74f;

                yPos += 1.0f;

                if (y % 2 != 0) {
                    xPos += 0.5f;
                }

                Tile _tilePrefab;
                switch ((TileType)board.map[x][y]) {
                    case TileType.Land:
                        _tilePrefab = _tileLandPrefab;
                        // yPos += UnityEngine.Random.Range(0.0f, 0.1f);
                        break;
                    case TileType.Water:
                        _tilePrefab = _tileWaterPrefab;
                        yPos -= 0.25f;
                        break;
                    case TileType.Mountain:
                        _tilePrefab = _tileMountainPrefab;
                        yPos += UnityEngine.Random.Range(0.1f, 0.2f);
                        break;
                    default:
                        _tilePrefab = _tileEmptyPrefab;
                        break;
                }
                if (_tilePrefab) {
                    var spawnedTile = Instantiate(_tilePrefab, new Vector3(xPos, yPos), Quaternion.identity, parentTransform);
                    spawnedTile.name = $"Tile {x} {y}";
                    spawnedTile.pos = new Vector2(x, y);
                    _tiles[new Vector2(x, y)] = spawnedTile;
                }
            }
        }
    }
 
    public Tile GetTileAtPosition(Vector2 pos) {
        Debug.Log($"GetTileAtPosition: {pos}");
        if (_tiles.TryGetValue(pos, out var tile)) return tile;
        return null;
    }    

    public List<Vector2> GetMovableTiles(BaseUnit unit, bool highlight = false) {

        var pos = new Vector2(  unit.Unit.position.x, 
                                unit.Unit.position.y);

        var movableTiles = new List<Vector2> { pos }; 
        GetAllMovableTiles(movableTiles, unit.Unit.moveDistance - 1);
    
        // var tile = GetTileAtPosition(pos);
        // if (tile == null) return null;
        // var movableTiles = new List<Vector2>();
        // // Add the current tile to the list of movable tiles
        // movableTiles.Add(pos);
        // var x = (int)pos.x;
        // var y = (int)pos.y;

        // var directions = y % 2 != 0 ? 
        //     new[] { 
        //         new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), 
        //         new Vector2(0, -1), new Vector2(1, 1), new Vector2(1, -1) } : 
        //     new[] { 
        //         new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), 
        //         new Vector2(0, -1), new Vector2(-1, 1), new Vector2(-1, -1) };

        // foreach (var dir in directions) {
        //     var newPos = pos + dir;
        //     var newTile = GetTileAtPosition(newPos);
        //     if (newTile != null && newTile.isMovable) {
        //         movableTiles.Add(newPos);
        //     }
        // }

        if (highlight) {
            UnhighlightTiles();
            if (unit is PlayerUnit playerUnit) {
                playerUnit.PossibleMoves = movableTiles;
            }
            HighlightTiles(movableTiles);
        }

        return movableTiles;
    }

    private void GetAllMovableTiles(List<Vector2> movableTiles, int depth) {
        if (depth == 0) return;

        var newPositions = new List<Vector2>(); // Temporary list to store new positions

        foreach (var tilePos in movableTiles) {
            var tile = GetTileAtPosition(tilePos);
            if (tile == null || !tile.isMovable) continue;

            var x = (int)tilePos.x;
            var y = (int)tilePos.y;

            var directions = y % 2 != 0 ? 
                new[] { 
                    new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), 
                    new Vector2(0, -1), new Vector2(1, 1), new Vector2(1, -1) } : 
                new[] { 
                    new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), 
                    new Vector2(0, -1), new Vector2(-1, 1), new Vector2(-1, -1) };

            foreach (var dir in directions) {
                var newPos = tilePos + dir;
                var newTile = GetTileAtPosition(newPos);
                if (newTile != null && !movableTiles.Contains(newPos) && !newPositions.Contains(newPos)) {
                    newPositions.Add(newPos); // Add to the temporary list
                }
            }
        }

        // Add all new positions to the original list after the loop
        movableTiles.AddRange(newPositions);

        depth--;
        GetAllMovableTiles(movableTiles, depth);
    }

    public void HighlightTiles(List<Vector2> tilePositions, bool raiseY = false) {
        foreach (var tilePos in tilePositions) {
            var tile = GetTileAtPosition(tilePos);
            if (tile == null || !tile.isMovable) continue;
            tile.MovableHighlight.SetActive(true);
            if (raiseY) {
                tile.transform.position += new Vector3(0, 0.05f, 0);
            }
        }
    }

    public void UnhighlightTiles(bool raiseY = false) {
        foreach (var tilePair in _tiles) {
            var tile = tilePair.Value;
            if (tile.MovableHighlight != null) {
                    if (tile.MovableHighlight.activeSelf == true) {
                    if (raiseY) {
                        tile.transform.position -= new Vector3(0, 0.05f, 0);
                    }
                    tile.MovableHighlight.SetActive(false);
                }
            }
        }

        // foreach (var tilePos in tilePositions) {
        //     var tile = GetTileAtPosition(tilePos);
        //     if (tile == null) continue;
        //     tile.MovableHighlight.SetActive(false);
        //     tile.transform.position -= new Vector3(0, 0.05f, 0);
        // }
    }
}

enum TileType {
    Land = 1,
    Water = 2,
    Mountain = 3
}