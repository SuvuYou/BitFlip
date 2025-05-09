using UnityEngine;

public class MapRenderer : MonoBehaviour
{
    [SerializeField] private SwappableTilemapRenderer _swappableTilemapRenderer;

    [SerializeField] MapDebugRenderer _mapDebugRenderer;

    public void Render(PathGeneration.Map map)
    {
        _swappableTilemapRenderer.ConstructSwappableTiles(map);

        _swappableTilemapRenderer.RenderTilemap();

        // _mapDebugRenderer.SetMap(_map);
    }
}