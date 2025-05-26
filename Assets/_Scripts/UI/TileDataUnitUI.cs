using PathGeneration;
using UnityEngine;
using UnityEngine.UI;

public class TileDataUnitUI : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _tileImage;

    [SerializeField] private Sprite _pathTileSprite;
    [SerializeField] private Sprite _wallTileSprite;
    [SerializeField] private Sprite _deadlyWallTileSprite;

    public void Setup(TileDataDisplayUI tileDataDisplayUI, Tile tile)
    {
        _button.onClick.AddListener(() => tileDataDisplayUI.DisplayData(tile));

        switch (tile.StateData.Type)
        {
            case TileType.Path:
                _tileImage.sprite = _pathTileSprite;
                break;
            case TileType.Wall:
                _tileImage.sprite = _wallTileSprite;
                break;
            case TileType.DeadlyWall:
                _tileImage.sprite = _deadlyWallTileSprite;
                break;
        }
    }
}