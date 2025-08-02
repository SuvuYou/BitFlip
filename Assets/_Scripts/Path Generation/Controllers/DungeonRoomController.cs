using CustomTiles;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace PathGeneration
{
    public class DungeonRoomController : MonoBehaviour
    {
        [SerializeField] private SwappableTilemapRenderer _renderer;
        [SerializeField] private TileBase _dungeonRoomDoorTile;
        [SerializeField] private CollisionContextDataSO _playerCollisionEvents;

        [SerializeField] private GameEvent<GameObject> _playerSpawnedEvent;

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
            _playerCollisionEvents.OnRaycastHitWall += HandleHitWall;
        }

        private void HandleHitWall(RaycastHit2D lastHit, Direction direction) 
        {
            if (lastHit.collider != null && lastHit.collider.TryGetComponent<Tilemap>(out var hitTilemap))
            {
                Vector3Int cellPos = hitTilemap.WorldToCell(lastHit.point +  0.5f * direction.ToVectorFloat());
                TileBase tile = hitTilemap.GetTile(cellPos);

                if (tile != null && tile is DoorRuleTile)
                {
                    Debug.Log("Ray lastHit tile: " + tile.name + " at " + cellPos);
                }
            }
        }
    }
}
