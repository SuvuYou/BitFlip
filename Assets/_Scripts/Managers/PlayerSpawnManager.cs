using UnityEngine;
using Cinemachine;

public class PlayerSpawnManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _playerCamera;
    [SerializeField] private GameObject _playerPrefab;

    public void Spawn(Vector3 spawnPoint)
    {
        var player = Instantiate(_playerPrefab, spawnPoint, Quaternion.identity);

        _playerCamera.Follow = player.transform;
        _playerCamera.LookAt = player.transform;
    }
}