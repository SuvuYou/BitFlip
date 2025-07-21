using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SwappableTilemapRenderer : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private Tilemap _tilemapCollider;

    [SerializeField] private Transform _tilesParent;

    [SerializeField] private SwapSystem.SwappableRuleTile _pathSwappableTilePrefab;
    [SerializeField] private SwapSystem.SwappableRuleTile _wallSwappableTilePrefab;
    [SerializeField] private SwapSystem.SwappableRuleTile _deadlyWallSwappableTilePrefab;

    public event Action<int, int, PathGeneration.Tile> OnRenderTile;
    public int Width => _swappableTiles.GetLength(0);
    public int Height => _swappableTiles.GetLength(1);

    private SwapSystem.SwappableRuleTile[,] _swappableTiles;

    private PathGeneration.Map _map;

    public void ReRenderTilemapRegion(Vector2Int start, Vector2Int end)
    {
        for (int x = start.x; x <= end.x; x++)
        {
            for (int y = start.y; y <= end.y; y++)
            {
                RenderTile(new Vector3Int(x, y, 0));
            }
        }
    }

    public void RenderTilemap()
    {
        _tilemap.ClearAllTiles();

        Vector3Int tilePosition = new (0, 0, 0);

        for (int x = 0; x < Width; x++)
        {
            tilePosition.x = x;

            for (int y = 0; y < Height; y++)
            {
                tilePosition.y = y;

                RenderTile(tilePosition);
            }
        }
    }

    private void RenderTile(Vector3Int tilePosition)
    {
        var currentTile = _tilemap.GetTile(tilePosition);
        var targetTile = _swappableTiles[tilePosition.x, tilePosition.y].GetActiveVariant();

        if (currentTile != targetTile) _tilemap.SetTile(tilePosition, targetTile);

        if (_map.MapPath.Tiles.GetTileByPosition(tilePosition.x, tilePosition.y).StateData.IsIncludedInDungeonRoom)
        {
            _tilemap.SetColor(tilePosition, Color.red);
        }

        if (_swappableTiles[tilePosition.x, tilePosition.y].IsCollidable)
        {
            _tilemapCollider.SetTile(tilePosition, _swappableTiles[tilePosition.x, tilePosition.y].GetActiveVariant());
        } 
    }

    public void ConstructSwappableTiles(PathGeneration.Map map)
    { 
        _map = map;

        _swappableTiles = new SwapSystem.SwappableRuleTile[map.MapPath.Tiles.Width,  map.MapPath.Tiles.Height];

        for (int x = 0; x < map.MapPath.Tiles.Width; x++)
        {
            for (int y = 0; y <  map.MapPath.Tiles.Height; y++)
            {
                Vector3Int tilePosition = new (x, y, 0);
                OnRenderTile?.Invoke(x, y, map.MapPath.Tiles.GetTileByPosition(x, y));

                switch (map.MapPath.Tiles.GetTileByPosition(x, y).StateData.Type)
                {
                    case PathGeneration.TileType.Path:
                        _swappableTiles[x, y] = Instantiate(_pathSwappableTilePrefab, tilePosition, Quaternion.identity, _tilesParent);
                        _swappableTiles[x, y].Init();
                        break;
                    case PathGeneration.TileType.Wall:
                        _swappableTiles[x, y] = Instantiate(_wallSwappableTilePrefab, tilePosition, Quaternion.identity, _tilesParent);
                        _swappableTiles[x, y].Init(isCollidable: true);
                        break;
                    case PathGeneration.TileType.DeadlyWall:
                        _swappableTiles[x, y] = Instantiate(_deadlyWallSwappableTilePrefab, tilePosition, Quaternion.identity, _tilesParent);
                        _swappableTiles[x, y].Init(isCollidable: true);
                        break;
                }
            }
        }
    }
}