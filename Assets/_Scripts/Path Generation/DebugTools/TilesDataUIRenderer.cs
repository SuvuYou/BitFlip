using UnityEngine;

public class TilesDataUIRenderer : MonoBehaviour
{
    [SerializeField] private TileDataDisplayUI _tileDataDisplayUI;
    [SerializeField] private RectTransform _parent;
    [SerializeField] private TileDataUnitUI _tileDataUnitUIPrefab;

    [SerializeField] private SwappableTilemapRenderer _swappableTilemapRenderer;

    private void Start()
    {
        _swappableTilemapRenderer.OnRenderTile += RenderTileDataUnitAt;
    }

    public void RenderTileDataUnitAt(int x, int y, PathGeneration.Tile tile)
    {
        var tileDataUnitUI = Instantiate(_tileDataUnitUIPrefab, _parent);
        tileDataUnitUI.transform.localPosition = new Vector3(x * 15, y * 15, 0);
        tileDataUnitUI.Setup(_tileDataDisplayUI, tile);
    }
}