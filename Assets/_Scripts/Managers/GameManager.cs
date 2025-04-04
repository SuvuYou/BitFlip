using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    [SerializeField] private GameDataSO _gameData;

    [SerializeField] private MapRenderer _mapRenderer;

    private Coroutine _swapCoroutine;

    private void Awake()
    {
        PseudoRandom.SystemRandomHolder.InitSystems();

        SwapSystem.SwappableEntitiesManager.Instance.InitContainers(_gameData);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwapSystem.SwappableEntitiesManager.Instance.SwapEntities(layerSwapInterval: 0.01f);
        }
    }
}