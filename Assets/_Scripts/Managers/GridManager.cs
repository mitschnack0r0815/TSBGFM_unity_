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
    [SerializeField] private Sprite[] landSprites;
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
                        yPos -= 0.2f;
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

                    // Assign a random sprite to land tiles
                    if ((TileType)board.map[x][y] == TileType.Land && landSprites.Length > 0)
                    {
                        var spriteRenderer = spawnedTile.GetComponentInChildren<SpriteRenderer>();
                        if (spriteRenderer != null)
                        {  
                            spriteRenderer.sprite = landSprites[UnityEngine.Random.Range(0, landSprites.Length)];
                        }
                    }

                    SetSortingLayer(spawnedTile, -y);

                    _tiles[new Vector2(x, y)] = spawnedTile;
                }
            }
        }
    }

    private void SetSortingLayer(Tile tile, int y) {
        var spriteRenderers = tile.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
        foreach (var spriteRenderer in spriteRenderers) {
            spriteRenderer.sortingOrder += y;
        }
    }
 
    public Tile GetTileAtPosition(Vector2 pos) {
        // Debug.Log($"GetTileAtPosition: {pos}");
        if (_tiles.TryGetValue(pos, out var tile)) return tile;
        return null;
    }    

    public List<Vector2> GetAttackableTiles(BaseUnit unit, bool highlight = false) {
        if (unit == null) return null;
        if (unit.ActionsLeft <= 0) return null;

        var pos = new Vector2(  unit.Unit.position.x, 
                                unit.Unit.position.y);

        var attackableTiles = new List<Vector2> { pos }; 
        GetAllAttackableTiles(attackableTiles, unit.Unit.weapons.first.range);

        var attackableUnits = new List<Vector2>(); // Temporary list to store attackable units
        foreach (var tile in attackableTiles) {
            var tileObj = GetTileAtPosition(tile);
            if (tileObj == null) continue;

            // Check if the tile is occupied by an enemy unit
            if (tileObj.IsOccupied) {
                if (tileObj.OccupiedUnit == null) continue; // Skip if no unit is present
                if (tileObj.OccupiedUnit.Unit.faction != unit.Unit.faction &&
                    unit.Unit.id != tileObj.OccupiedUnit.Unit.id) 
                {
                    attackableUnits.Add(tile); // Add the tile to the attackable tiles list
                } 
            } 
        }

        if (highlight) {
            if (unit is PlayerUnit playerUnit) {
                playerUnit.PossibleAttacks = attackableUnits;
            }
            HighlightTiles(attackableUnits, color: new Color(1f, 0f, 0f, 0.5f));
        }

        return attackableUnits;
    }

    private void GetAllAttackableTiles(List<Vector2> attackableTiles, int depth) {
        if (depth == 0) return;

        var newPositions = new List<Vector2>(); // Temporary list to store new positions

        foreach (var tilePos in attackableTiles) {
            var tile = GetTileAtPosition(tilePos);
            if (tile == null) continue;

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
                if (newTile != null && newTile.IsOccupied &&
                    !attackableTiles.Contains(newPos) && 
                    !newPositions.Contains(newPos)) 
                {
                    newPositions.Add(newPos); // Add to the temporary list
                }
            }
        }

        // Add all new positions to the original list after the loop
        attackableTiles.AddRange(newPositions);

        depth--;
        GetAllAttackableTiles(attackableTiles, depth);
    }


    public List<Vector2> GetMovableTiles(BaseUnit unit, bool highlight = false) {
        if (unit == null) return null;
        if (unit.ActionsLeft <= 0) return null;

        var pos = new Vector2(  unit.Unit.position.x, 
                                unit.Unit.position.y);

        var movableTiles = new List<Vector2> { pos }; 
        GetAllMovableTiles(movableTiles, unit.Unit.moveDistance);

        if (highlight) {
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
                if (newTile != null && !newTile.IsOccupied &&
                    !movableTiles.Contains(newPos) && 
                    !newPositions.Contains(newPos)) 
                {
                    newPositions.Add(newPos); // Add to the temporary list
                }
            }
        }

        // Add all new positions to the original list after the loop
        movableTiles.AddRange(newPositions);

        depth--;
        GetAllMovableTiles(movableTiles, depth);
    }

    public void HighlightTiles(List<Vector2> tilePositions, bool raiseY = false, Color color = default) {
        foreach (var tilePos in tilePositions) {
            var tile = GetTileAtPosition(tilePos);
            if (tile == null || !tile.isMovable) continue;

            var spriteRenderer = tile.GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                if (color != default) {
                    tile.MovableHighlight.GetComponent<SpriteRenderer>().color = color;
                } else {
                    // Set a default color if none is provided (greenish)
                    tile.MovableHighlight.GetComponent<SpriteRenderer>().color = new Color(0.03f, 0.25f, 0f, 0.75f);
                }
            }

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
    }
}

enum TileType {
    Land = 1,
    Water = 2,
    Mountain = 3
}