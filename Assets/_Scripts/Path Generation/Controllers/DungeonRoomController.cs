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

            foreach(((Vector2Int entrance, Vector2Int exit), DungeonRoomVariant roomVariant) in dungeonRoom.VariantsPerEntrance)
            {
                (Vector2Int lowerBounds, Vector2Int upperBounds) = dungeonRoom.Bounds;
            }
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
        

        private void OnRoomEnter(DungeonRoom dungeonRoom, DungeonRoomVariant roomVariant, Vector2Int position)
        {
            dungeonRoom.AddEploredEntrance(position);
            RenderRoomVarient(dungeonRoom, roomVariant);

            // RenderDoors(dungeonRoom, roomVariant);
        }

        private void OnRoomExit(DungeonRoom dungeonRoom, Vector2Int position) 
        {
            dungeonRoom.AddEploredEntrance(position);
            RenderRoomVarient(dungeonRoom, dungeonRoom.OriginalVariant);
            // RenderOriginDoors(dungeonRoom);
        }

        public void RenderOriginDoors(DungeonRoom dungeonRoom)
        {
            foreach (Vector2Int exit in dungeonRoom.GetUnexploredEntrances())
            {
                RenderDoorAt(exit);
            }
        }

        private void RenderRoomVarient(DungeonRoom dungeonRoom, DungeonRoomVariant roomVariant)
        {
            dungeonRoom.Tiles.SetTilesDataFromMatrix(roomVariant.Tiles);

            _renderer.ReConstructTilemapRegion(dungeonRoom.Bounds.Item1, dungeonRoom.Bounds.Item2);
            _renderer.ReRenderTilemapRegion(dungeonRoom.Bounds.Item1, dungeonRoom.Bounds.Item2);
        }

        private void RenderDoors(DungeonRoom dungeonRoom, DungeonRoomVariant roomVariant)
        {
            RenderDoorAt(dungeonRoom.Bounds.Item1 + roomVariant.EnterPosition);
            RenderDoorAt(dungeonRoom.Bounds.Item1 + roomVariant.ExitPosition);
        }

        private void RenderDoorAt(Vector2Int position)
        {
            _renderer.ForceRenderTileAt(position.x, position.y, _dungeonRoomDoorTile);
        }
    }
}
