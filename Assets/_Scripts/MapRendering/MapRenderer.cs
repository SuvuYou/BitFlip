using System.Collections.Generic;
using UnityEngine;

public class MapRenderer : MonoBehaviour
{
    [SerializeField] private int _width = 10, _height = 10;

    [SerializeField] private GameObject _wallTilePrefab;
    [SerializeField] private GameObject _pathTilePrefab;
    [SerializeField] private GameObject _invalidTilePrefab;

    [SerializeField] private TilemapRenderer _renderer;

    private PathGeneration.Map _map;

    private int index;

    private List<GameObject> garbage = new List<GameObject>();

    private void Awake()
    {
        _map = new PathGeneration.Map(_width, _height);

        _map.Generate();

        _map.MapPath.TilesSnapshotManager.Read();

        _renderer.RenderPath(_map.MapPath.Tiles);
    }

    public void Swap()
    {
        _renderer.SwapTiles();
    }


    // public void Update()
    // {
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         garbage.ForEach(x => Destroy(x));

    //         DrawAll(_map.MapPath.TilesSnapshotManager.Next());
    //     }

    //     if (Input.GetMouseButtonDown(1))
    //     {
    //         garbage.ForEach(x => Destroy(x));

    //         DrawAll(_map.MapPath.TilesSnapshotManager.Prev());
    //     }
    // }
}