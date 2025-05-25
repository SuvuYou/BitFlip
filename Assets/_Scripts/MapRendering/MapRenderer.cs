using UnityEngine;

public class MapRenderer : MonoBehaviour
{
    [SerializeField] private SwappableTilemapRenderer _swappableTilemapRenderer;

    public void Render(PathGeneration.Map map)
    {
        _swappableTilemapRenderer.ConstructSwappableTiles(map);

        _swappableTilemapRenderer.RenderTilemap();
    }
}