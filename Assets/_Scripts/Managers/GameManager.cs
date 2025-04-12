using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    [SerializeField] private GameDataSO _gameData;

    [SerializeField] private MapRenderer _mapRenderer;

    [SerializeField] private PlayerSpawnManager _playerSpawnManager;

    private Coroutine _swapCoroutine;

    private void Awake()
    {
        PseudoRandom.SystemRandomHolder.InitSystems();

        SwapSystem.SwappableEntitiesManager.Instance.InitContainers(_gameData);
    }

    private IEnumerator Start()
    {
        yield return null;
        
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