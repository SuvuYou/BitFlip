using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SwappableTilemapRenderer : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private Tilemap _tilemapCollider;

    [SerializeField] private Transform _tilesParent;

    [SerializeField] private SwapSystem.SwappableRuleTile _swappableTilePrefab;
    private ObjectPool<SwapSystem.SwappableRuleTile> _swappableTilePool;

    public event Action<int, int, PathGeneration.Tile> OnRenderTile;
    public int Width => _swappableTiles.GetLength(0);
    public int Height => _swappableTiles.GetLength(1);

    private SwapSystem.SwappableRuleTile[,] _swappableTiles;

    private PathGeneration.Map _map;

    public void InitObjecPool()
    {
        _swappableTilePool = new ObjectPool<SwapSystem.SwappableRuleTile>(_swappableTilePrefab, _tilesParent, initialPoolSize: 1000);
    }

    public void ReRenderTilemapRegion(Vector2Int start, Vector2Int end)
    {
        for (int x = start.x; x <= end.x; x++)
        {
            for (int y = start.y; y <= end.y; y++)
            {
                RenderTile(x, y);
            }
        }
    }

    public void ReConstructTilemapRegion(Vector2Int start, Vector2Int end)
    {
        for (int x = start.x; x <= end.x; x++)
        {
            for (int y = start.y; y <= end.y; y++)
            {
                RemapConstructSwappableTile(x, y);
            }
        }
    }

    public void RenderTilemap()
    {
        _tilemap.ClearAllTiles();

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                RenderTile(x, y);
            }
        }
    }

    public void ConstructTilemap(PathGeneration.Map map)
    { 
        if (map != null)
        {
            _map = map;
        }

        _swappableTiles = new SwapSystem.SwappableRuleTile[_map.MapTiles.Width,  _map.MapTiles.Height];

        for (int x = 0; x < _map.MapTiles.Width; x++)
        {
            for (int y = 0; y < _map.MapTiles.Height; y++)
            {
                InstantiateConstructSwappableTile(x, y);
            }
        }
    }

    private void RenderTile(int x, int y)
    {
        Vector3Int tilePosition = new (x, y, 0);

        var currentTile = _tilemap.GetTile(tilePosition);
        var targetTile = _swappableTiles[x, y].GetActiveVariant();

        if (currentTile == targetTile) return;

        _tilemap.SetTile(tilePosition, targetTile);

        if (_map.MapTiles.GetTileByPosition(x, y).StateData.IsIncludedInDungeonRoom)
        {
            _tilemap.SetColor(tilePosition, Color.red);
        }

        if (_swappableTiles[x, y].IsCollidable)
        {
            _tilemapCollider.SetTile(tilePosition, _swappableTiles[x, y].GetActiveVariant());
        } 
        else
        {
            _tilemapCollider.SetTile(tilePosition, null);
        }
    }

    private void RemapConstructSwappableTile(int x, int y)
    { 
        OnRenderTile?.Invoke(x, y, _map.MapTiles.GetTileByPosition(x, y));

        switch (_map.MapTiles.GetTileByPosition(x, y).StateData.Type)
        {
            case PathGeneration.TileType.Path:
                _swappableTiles[x, y].SetTileType(PathGeneration.TileType.Path);
                break;
            case PathGeneration.TileType.Wall:
                _swappableTiles[x, y].SetTileType(PathGeneration.TileType.Wall);
                break;
            case PathGeneration.TileType.DeadlyWall:
                _swappableTiles[x, y].SetTileType(PathGeneration.TileType.DeadlyWall);
                break;
        }
    }

    private void InstantiateConstructSwappableTile(int x, int y)
    { 
        Vector3Int tilePosition = new (x, y, 0);
        OnRenderTile?.Invoke(x, y, _map.MapTiles.GetTileByPosition(x, y));

        switch (_map.MapTiles.GetTileByPosition(x, y).StateData.Type)
        {
            case PathGeneration.TileType.Path:
                _swappableTiles[x, y] = PoolTileOfType(PathGeneration.TileType.Path);
                _swappableTiles[x, y].transform.localPosition = tilePosition;
                break;
            case PathGeneration.TileType.Wall:
                _swappableTiles[x, y] = PoolTileOfType(PathGeneration.TileType.Wall);
                _swappableTiles[x, y].transform.localPosition = tilePosition;
                break;
            case PathGeneration.TileType.DeadlyWall:
                _swappableTiles[x, y] = PoolTileOfType(PathGeneration.TileType.DeadlyWall);
                _swappableTiles[x, y].transform.localPosition = tilePosition;
                break;
        }
    }

    private SwapSystem.SwappableRuleTile PoolTileOfType(PathGeneration.TileType type) 
    {
        SwapSystem.SwappableRuleTile tile = _swappableTilePool.GetObject();
        tile.Init();
        tile.SetTileType(type);

        return tile;
    }
}