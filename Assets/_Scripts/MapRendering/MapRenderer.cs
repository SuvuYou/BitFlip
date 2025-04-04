using UnityEngine;

public class MapRenderer : MonoBehaviour
{
    [SerializeField] private GameDataSO _gameData;

    [SerializeField] private SwappableTilemapRenderer _swappableTilemapRenderer;

    private PathGeneration.Map _map;

    private void Awake()
    {
        _map = new PathGeneration.Map(_gameData.MapWidth, _gameData.MapHeight);

        _map.Generate();

        _swappableTilemapRenderer.ConstructSwappableTiles(_map);

        _swappableTilemapRenderer.RenderTilemap();

        // TODO: remove snapshots
        _map.MapPath.TilesSnapshotManager.Read();
    }
}