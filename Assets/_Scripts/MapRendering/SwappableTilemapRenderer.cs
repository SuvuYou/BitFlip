using UnityEngine;
using UnityEngine.Tilemaps;

public class SwappableTilemapRenderer : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemap;

    [SerializeField] private SwapSystem.SwappableRuleTile _pathSwappableTilePrefab;
    [SerializeField] private SwapSystem.SwappableRuleTile _wallSwappableTilePrefab;

    private SwapSystem.SwappableRuleTile[,] _swappableTiles;

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
            }
        }
    }

    public void ConstructSwappableTiles(PathGeneration.Map map)
    { 
        _swappableTiles = new SwapSystem.SwappableRuleTile[map.MapPath.Width,  map.MapPath.Height];

        for (int x = 0; x < map.MapPath.Width; x++)
        {
            for (int y = 0; y <  map.MapPath.Height; y++)
            {
                Vector3Int tilePosition = new (x, y, 0);

                switch (map.MapPath.Tiles[x, y].Type)
                {
                    case PathGeneration.TileType.Path:
                        _swappableTiles[x, y] = Instantiate(_pathSwappableTilePrefab, tilePosition, Quaternion.identity);
                        _swappableTiles[x, y].Init();
                        break;
                    case PathGeneration.TileType.Wall:
                        _swappableTiles[x, y] = Instantiate(_wallSwappableTilePrefab, tilePosition, Quaternion.identity);
                        _swappableTiles[x, y].Init();
                        break;
                }
            }
        }
    }
}