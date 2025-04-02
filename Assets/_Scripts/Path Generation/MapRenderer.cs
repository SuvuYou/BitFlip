using System.Collections.Generic;
using UnityEngine;

public class MapRenderer : MonoBehaviour
{
    [SerializeField] private int _width = 10, _height = 10;

    [SerializeField] private GameObject _wallTilePrefab;
    [SerializeField] private GameObject _pathTilePrefab;
    [SerializeField] private GameObject _invalidTilePrefab;

    private PathGeneration.Map _map;

    private int index;

    private List<GameObject> garbage = new List<GameObject>();

    private void Awake()
    {
        _map = new PathGeneration.Map(_width, _height);

        _map.Generate();

        _map.MapPath.TilesSnapshotManager.Read();

        // DrawAll();
    }


    // private void DrawBackground()
    // {
    //     for (int x = 0; x < _map.MapPath.Width; x++)
    //     {
    //         for (int y = 0; y < _map.MapPath.Height; y++)
    //         {
    //             Instantiate(_wallTilePrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
    //         }
    //     }
    // }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            garbage.ForEach(x => Destroy(x));

            DrawAll(_map.MapPath.TilesSnapshotManager.Next());
        }

        if (Input.GetMouseButtonDown(1))
        {
            garbage.ForEach(x => Destroy(x));

            DrawAll(_map.MapPath.TilesSnapshotManager.Prev());
        }
    }

    // public void DrawNext()
    // {
    //     (Vector2Int pos, PathGeneration.Direction dir) state = _map.MapPath.pathStack.ToList()[index];

    //     Instantiate(_pathTilePrefab, new Vector3(state.pos.x, state.pos.y, 0), Quaternion.identity, transform);

    //     index++;
    // }

    // private void DrawMap()
    // {
    //     for (int x = 0; x < _map.MapPath.Width; x++)
    //     {
    //         for (int y = 0; y < _map.MapPath.Height; y++)
    //         {
    //             if (_map.MapPath.tiles[x, y]?.Type == PathGeneration.TileType.Wall)
    //             {
    //                 Instantiate(_wallTilePrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
    //             }
    //             else
    //             {
    //                 Instantiate(_pathTilePrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
    //             }
    //         }
    //     }
    // }

    // private void DrawInvalid()
    // {
    //     for (int x = 0; x < _map.MapPath.Width; x++)
    //     {
    //         for (int y = 0; y < _map.MapPath.Height; y++)
    //         {
    //             if (!_map.MapPath.tiles[x, y].IsValid)
    //                 Instantiate(_invalidTilePrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
    //         }
    //     }
    // }

    private void DrawAll(PathGeneration.Tile[,] tiles)
    {
        for (int x = 0; x < _map.MapPath.Width; x++)
        {
            for (int y = 0; y < _map.MapPath.Height; y++)
            {
                if (!tiles[x, y].IsValid)
                {
                    garbage.Add(Instantiate(_invalidTilePrefab, new Vector3(x, y, 0), Quaternion.identity, transform));

                    continue;
                }

                if (tiles[x, y]?.Type == PathGeneration.TileType.Wall)
                {
                    garbage.Add(Instantiate(_wallTilePrefab, new Vector3(x, y, 0), Quaternion.identity, transform));
                }
                else if (tiles[x, y]?.Type == PathGeneration.TileType.Path)
                {
                    garbage.Add(Instantiate(_pathTilePrefab, new Vector3(x, y, 0), Quaternion.identity, transform));
                }
            }
        }
    }
}