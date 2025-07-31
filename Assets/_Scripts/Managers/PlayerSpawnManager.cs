using UnityEngine;
using Cinemachine;

public class PlayerSpawnManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _playerCamera;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameEvent<GameObject> _playerSpawnedEvent;

    private GameObject _player;

    public void Spawn(Vector3 spawnPoint)
    {
        if (_player != null) Destroy(_player);

        _player = Instantiate(_playerPrefab, spawnPoint, Quaternion.identity);

        _playerCamera.Follow = _player.transform;
        _playerCamera.LookAt = _player.transform;

        _playerSpawnedEvent.Raise(_player);
    }
}