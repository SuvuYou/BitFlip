using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapRenderer : MonoBehaviour
{
    public Tilemap tilemap;
    public SwapSystem.SwappableRuleTile pathTile;
    public SwapSystem.SwappableRuleTile wallTile;  

    private PathGeneration.Tile[,] _tiles;

    public void RenderPath(PathGeneration.Tile[,] tiles)
    {
        _tiles = tiles;

        tilemap.ClearAllTiles();

        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);

                switch (tiles[x, y].Type)
                {
                    case PathGeneration.TileType.Path:
                        tilemap.SetTile(tilePosition, pathTile.GetActiveVariant());
                        break;
                    case PathGeneration.TileType.Wall:
                        tilemap.SetTile(tilePosition, wallTile.GetActiveVariant());
                        break;
                }
            }
        }
    }

    public void SwapTiles()
    {
        SwapSystem.SwappableEntitiesManager.Instance.SwapVariant();

        RenderPath(_tiles);
    }
}