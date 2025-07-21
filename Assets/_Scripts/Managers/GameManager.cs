using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    [SerializeField] private PathGeneration.MapController _mapController;
    [SerializeField] private PlayerSpawnManager _playerSpawnManager;
    [SerializeField] private EnemySpawner _enemySpawner;

    [SerializeField] private SeedField _seedField;

    private void Start()
    {
        SwapSystem.SwappableEntitiesManager.Instance.InitContainers();
    }

    public void Spawn()
    {
        int seed = PseudoRandom.SystemRandomHolder.InitSystems(_seedField.Seed);

        _seedField.SetSeedText(seed);

        PathGeneration.Map map = _mapController.GenerateMap();

        _playerSpawnManager.Spawn(map.MapPath.StartPosition.ToVector3WithZ(z: 0));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwapSystem.SwappableEntitiesManager.Instance.SwapEntities(layerSwapInterval: 0.01f);
        }
    }
}