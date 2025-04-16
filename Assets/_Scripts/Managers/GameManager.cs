using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    [SerializeField] private GameDataSO _gameData;

    [SerializeField] private MapRenderer _mapRenderer;

    [SerializeField] private PlayerSpawnManager _playerSpawnManager;

    [SerializeField] private SeedField _seedField;

    private void Awake()
    {
        SwapSystem.SwappableEntitiesManager.Instance.InitContainers(_gameData);
    }

    public void Spawn()
    {
        int seed = PseudoRandom.SystemRandomHolder.InitSystems(_seedField.Seed);

        _seedField.SetSeedText(seed);

        (Vector3Int startPosition, Vector3Int _) = _mapRenderer.Render();

        _playerSpawnManager.Spawn(startPosition);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwapSystem.SwappableEntitiesManager.Instance.SwapEntities(layerSwapInterval: 0.01f);
        }
    }
}