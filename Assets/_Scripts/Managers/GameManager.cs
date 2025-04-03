using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{

    public MapRenderer _mapRenderer;

    private void Awake()
    {
        PseudoRandom.SystemRandomHolder.InitSystems();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _mapRenderer.Swap();
        }
    }
}