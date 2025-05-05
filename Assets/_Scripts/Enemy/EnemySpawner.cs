using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private SwappableEnemy _enemyPrefab;

    public void Spawn(Vector3 spawnPoint)
    {
        Instantiate(_enemyPrefab, spawnPoint, Quaternion.identity);
    }
}

