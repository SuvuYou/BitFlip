using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    [SerializeField] private GameDataSO _gameData;

    [SerializeField] private MapRenderer _mapRenderer;

    [SerializeField] private PlayerSpawnManager _playerSpawnManager;
    [SerializeField] private EnemySpawner _enemySpawner;

    [SerializeField] private SeedField _seedField;

    private PathGeneration.Map _map;

    private void Awake()
    {
        SwapSystem.SwappableEntitiesManager.Instance.InitContainers(_gameData);

        
    }

    public void Spawn()
    {
        int seed = PseudoRandom.SystemRandomHolder.InitSystems(_seedField.Seed);

        _seedField.SetSeedText(seed);

        _map = new PathGeneration.Map(_gameData);

        _map.Generate();

        _mapRenderer.Render(_map);

        _playerSpawnManager.Spawn(_map.MapPath.StartPosition.ToVector3WithZ(z: 0));
        _enemySpawner.Spawn(_map.GetRandomDungeonRoom().GetRandomPathTilePosition());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwapSystem.SwappableEntitiesManager.Instance.SwapEntities(layerSwapInterval: 0.01f);
        }
    }
}