using System.Linq;
using CustomTiles;
using PathGeneration;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MapRendering
{
    public class DungeonRoomController : MonoBehaviour
    {
        [SerializeField] private DungeonDoorController _doorController;
        [SerializeField] private SwappableTilemapRenderer _renderer;
        [SerializeField] private TileBase _dungeonRoomDoorTile;
        [SerializeField] private CollisionContextDataSO _playerCollisionEvents;

        [SerializeField] private GameEvent<GameObject> _playerSpawnedEvent;

        [SerializeField] private DungeonEntrySequenceOrchestrator _dungeonEntrySequenceOrchestrator;

        private PlayerMovement _playerMovement;

        private void Start() 
        {
            _playerSpawnedEvent.Register((GameObject player) => 
            {
                if (player.transform.root.TryGetComponentInChildren(out PlayerMovement playerMovement))
                {
                    _playerMovement = playerMovement;
                }
                else
                {
                    Debug.LogError("Couldn't find player movement");
                }
            });
        }

        public void SetupRoomEntranceTriggers(DungeonRoom dungeonRoom)
        {
            _playerCollisionEvents.OnRaycastHitWall += (lastHit, direction) => HandleHitWall(lastHit, direction, dungeonRoom);
        }

        private void HandleHitWall(RaycastHit2D lastHit, Direction direction, DungeonRoom dungeonRoom) 
        {
            if (lastHit.collider != null && lastHit.collider.TryGetComponent<Tilemap>(out var hitTilemap))
            {
                Vector3Int cellPos = hitTilemap.WorldToCell(lastHit.point +  0.5f * direction.ToVectorFloat());
                TileBase tile = hitTilemap.GetTile(cellPos);

                if (tile == null || tile is not DoorRuleTile) return;

                foreach (DungeonDoorData door in dungeonRoom.Doors)
                {
                    var globalDoorPosition = door.Position.Global;

                    if (cellPos.x == globalDoorPosition.x && cellPos.y == globalDoorPosition.y)
                    {
                        switch (door.State)
                        {
                            case DungeonDoorState.Open:
                                Debug.Log("Door is already open.");
                                break;
                            case DungeonDoorState.Locked:
                                Debug.Log("Door is locked.");
                                break;
                            case DungeonDoorState.Unexplored:
                                Debug.Log("Door is unexplored.");
                                
                                _dungeonEntrySequenceOrchestrator.StartSequence(_playerMovement, _doorController, door);

                                if (door.Type == DungeonDoorType.Entrance)
                                {
                                    dungeonRoom.SetDungeonRoomVariant(door.RelatedRoomVariant);
                                    _renderer.ReConstructTilemapRegion(dungeonRoom.LowerBounds, dungeonRoom.UpperBounds);
                                    _renderer.ReRenderTilemapRegion(dungeonRoom.LowerBounds, dungeonRoom.UpperBounds);
                                }

                                if (door.Type == DungeonDoorType.Exit)
                                {
                                    dungeonRoom.SetDungeonRoomVariant(dungeonRoom.OriginalVariant);

                                    _renderer.ReConstructTilemapRegion(dungeonRoom.LowerBounds, dungeonRoom.UpperBounds);
                                    _renderer.ReRenderTilemapRegion(dungeonRoom.LowerBounds, dungeonRoom.UpperBounds);
                                }
           
                                break;
                        }
                    }
                }
            }
        }
    }
}
