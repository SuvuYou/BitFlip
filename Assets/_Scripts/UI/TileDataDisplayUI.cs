using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TileDataDisplayUI : MonoBehaviour
{
    // State data
    [SerializeField] private TextMeshProUGUI _tileTypeText;
    [SerializeField] private TextMeshProUGUI _tileConnectionTypeText;
    [SerializeField] private TextMeshProUGUI _isBorderText;
    [SerializeField] private TextMeshProUGUI _isIncludedInDungeonRoomText;
    [SerializeField] private TextMeshProUGUI _isValidText;

    // Previous facing direction
    [SerializeField] private GameObject _upImage;
    [SerializeField] private GameObject _rightImage;
    [SerializeField] private GameObject _downImage;
    [SerializeField] private GameObject _leftImage;

    // Connections
    [SerializeField] private Image _upConnectionImage;
    [SerializeField] private Image _rightConnectionImage;
    [SerializeField] private Image _downConnectionImage;
    [SerializeField] private Image _leftConnectionImage;

    public void DisplayData(PathGeneration.Tile tile)
    {
        // State data
        _tileTypeText.text = tile.StateData.Type.ToString();
        _tileConnectionTypeText.text = tile.StateData.ConnectionType.ToString();
        _isBorderText.text = tile.StateData.IsBorder.ToString();
        _isIncludedInDungeonRoomText.text = tile.StateData.IsIncludedInDungeonRoom.ToString();
        _isValidText.text = tile.StateData.IsValid.ToString();

        // Previous facing direction
        DesableAllDirectionObjects();

        switch (tile.StateData.PreviousFacingDirection)
        {
            case Direction.Up:
                _upImage.SetActive(true);
                break;
            case Direction.Right:
                _rightImage.SetActive(true);
                break;
            case Direction.Down:
                _downImage.SetActive(true);
                break;
            case Direction.Left:
                _leftImage.SetActive(true);
                break;
        }

        // Connections
        _upConnectionImage.gameObject.SetActive(tile.IsConnectedToDirection(Direction.Up));
        _rightConnectionImage.gameObject.SetActive(tile.IsConnectedToDirection(Direction.Right));
        _downConnectionImage.gameObject.SetActive(tile.IsConnectedToDirection(Direction.Down));
        _leftConnectionImage.gameObject.SetActive(tile.IsConnectedToDirection(Direction.Left));
    }

    private void DesableAllDirectionObjects() 
    {
        _upImage.SetActive(false);
        _rightImage.SetActive(false);
        _downImage.SetActive(false);
        _leftImage.SetActive(false);
    }
}