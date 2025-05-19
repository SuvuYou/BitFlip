using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private SwappableEnemy _enemyPrefab;

    public void Spawn(Vector3 spawnPoint)
    {
        Instantiate(_enemyPrefab, new Vector3(24, 20, 0), Quaternion.identity);
    }
}

