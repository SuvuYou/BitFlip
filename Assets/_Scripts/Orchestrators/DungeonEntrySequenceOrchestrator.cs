using System.Collections;
using MapRendering;
using PathGeneration;
using UnityEngine;

public class DungeonEntrySequenceOrchestrator : MonoBehaviour
{
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private EnemySpawner _enemySpawner;

    public void StartSequence(PlayerMovement _playerMovement, DungeonDoorController _doorController, DungeonDoorData doorData)
    {
        StartCoroutine(RunSequence(_playerMovement, _doorController, doorData));
    }

    private IEnumerator RunSequence(PlayerMovement _playerMovement, DungeonDoorController _doorController, DungeonDoorData doorData)
    {
        _doorController.Init(doorData);

        var openDoorTask = _doorController.Open(doorData);

        yield return new WaitUntil(() => openDoorTask.IsCompleted);

        _playerMovement.Movement.SetPosition(doorData.FirstWalkableSpacePosition.Global.ToVector3WithZ(z: 0f));
        _playerMovement.Movement.SetDirection(doorData.LeadingDirection.Opposite());

        

        // yield return new WaitForSeconds(0.1f);

        // yield return _cameraController.ZoomToDungeon();
        // // yield return _enemySpawner.SpawnAll();
        // _doorController.Lock(doorData);

        // yield return _cameraController.ZoomToPlayer();

        // Debug.Log("Dungeon entry sequence complete.");
    }
}