using UnityEngine;
using UnityEngine.Tilemaps;

public class SwappableTilemapRenderer : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private Tilemap _tilemapCollider;

    [SerializeField] private SwapSystem.SwappableRuleTile _pathSwappableTilePrefab;
    [SerializeField] private SwapSystem.SwappableRuleTile _wallSwappableTilePrefab;
    [SerializeField] private SwapSystem.SwappableRuleTile _deadlyWallSwappableTilePrefab;

    [SerializeField] private RouteIndexUIRenderer _routeIndexUIRenderer;

    private SwapSystem.SwappableRuleTile[,] _swappableTiles;

    private PathGeneration.Map _map;

    private void Start()
    {
        SwapSystem.SwappableEntitiesManager.Instance.OnSwapAtYLevelComplete += (int yLevel) => RenderTilemap(); 
    }

    public void RenderTilemap()
    {
        _tilemap.ClearAllTiles();

        Vector3Int tilePosition = new (0, 0, 0);

        for (int x = 0; x < _swappableTiles.GetLength(0); x++)
        {
            for (int y = 0; y < _swappableTiles.GetLength(1); y++)
            {
                tilePosition.x = x;
                tilePosition.y = y;

                _tilemap.SetTile(tilePosition, _swappableTiles[x, y].GetActiveVariant());

                if (_map.MapPath.Tiles.GetTileByPosition(x, y).StateData.IsIncludedInDungeonRoom)
                {
                    _tilemap.SetColor(tilePosition, Color.red);
                }

                if (_swappableTiles[x, y].IsCollidable)
                    _tilemapCollider.SetTile(tilePosition, _swappableTiles[x, y].GetActiveVariant());
            }
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
                _routeIndexUIRenderer.RenderRouteAt(map.MapPath.Tiles.GetTileByPosition(x, y).StateData.RouteIndices, x, y);

                switch (map.MapPath.Tiles.GetTileByPosition(x, y).StateData.Type)
                {
                    case PathGeneration.TileType.Path:
                        _swappableTiles[x, y] = Instantiate(_pathSwappableTilePrefab, tilePosition, Quaternion.identity);
                        _swappableTiles[x, y].Init();
                        break;
                    case PathGeneration.TileType.Wall:
                        _swappableTiles[x, y] = Instantiate(_wallSwappableTilePrefab, tilePosition, Quaternion.identity);
                        _swappableTiles[x, y].Init(isCollidable: true);
                        break;
                    case PathGeneration.TileType.DeadlyWall:
                        _swappableTiles[x, y] = Instantiate(_deadlyWallSwappableTilePrefab, tilePosition, Quaternion.identity);
                        _swappableTiles[x, y].Init(isCollidable: true);
                        break;
                }
            }
        }
    }
}