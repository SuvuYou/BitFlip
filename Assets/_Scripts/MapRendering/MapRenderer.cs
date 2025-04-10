using UnityEngine;

public class MapRenderer : MonoBehaviour
{
    [SerializeField] private GameDataSO _gameData;

    [SerializeField] private SwappableTilemapRenderer _swappableTilemapRenderer;

    private PathGeneration.Map _map;

    [SerializeField] MapDebugRenderer _mapDebugRenderer;

    public (Vector3Int startPosition, Vector3Int endPosition) Render()
    {
        _map = new PathGeneration.Map(_gameData.MapWidth, _gameData.MapHeight, _gameData.MapBorderSize, _gameData.MapStemLength);

        _map.Generate();

        _swappableTilemapRenderer.ConstructSwappableTiles(_map);

        _swappableTilemapRenderer.RenderTilemap();

        // _mapDebugRenderer.SetMap(_map);

        return (_map.MapPath.StartPosition.ToVector3WithZ(0), _map.MapPath.EndPosition.ToVector3WithZ(0));
    }
}