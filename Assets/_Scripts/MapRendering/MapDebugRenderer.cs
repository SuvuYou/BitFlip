using System.Collections.Generic;
using UnityEngine;

public class MapDebugRenderer : MonoBehaviour
{
    private PathGeneration.Map _map;

    [SerializeField] GameObject _pathTilePrefab, _wallTilePrefab, _borderTilePrefab;

    private List<GameObject> garbage = new ();

    public void SetMap(PathGeneration.Map map)
    {
        _map = map;
        // TODO: remove snapshots
        _map.MapPath.TilesSnapshotManager.Read();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DrawAll(_map.MapPath.TilesSnapshotManager.Next());
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            DrawAll(_map.MapPath.TilesSnapshotManager.Prev());
        }
    }

    private void DrawAll(PathGeneration.Tile[,] tiles)
    {
        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                if (tiles[x, y].IsBorder)
                {
                    garbage.Add(Instantiate(_borderTilePrefab, new Vector3(x, y, 0), Quaternion.identity));

                    continue;
                }

                if (tiles[x, y].Type == PathGeneration.TileType.Path)
                {
                    garbage.Add(Instantiate(_pathTilePrefab, new Vector3(x, y, 0), Quaternion.identity));

                    continue;
                }
                
                if (tiles[x, y].Type == PathGeneration.TileType.Wall) {
                    garbage.Add(Instantiate(_wallTilePrefab, new Vector3(x, y, 0), Quaternion.identity));

                    continue;
                }
            }
        }
    }
}