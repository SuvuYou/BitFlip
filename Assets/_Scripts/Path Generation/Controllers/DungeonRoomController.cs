using UnityEngine;
using UnityEngine.Tilemaps;

namespace PathGeneration
{
    public class DungeonRoomController : MonoBehaviour
    {
        [SerializeField] private SwappableTilemapRenderer _renderer;
        [SerializeField] private TileBase _dungeonRoomDoorTile;

        public void SetupRoomEntranceTriggers(DungeonRoom dungeonRoom)
        {
            foreach(((Vector2Int entrance, Vector2Int exit), DungeonRoomVariant roomVariant) in dungeonRoom.VariantsPerEntrance)
            {
                (Vector2Int lowerBounds, Vector2Int upperBounds) = dungeonRoom.Bounds;

                DungeonRoomMovementTrigger entranceTrigger = DungeonRoomMovementTrigger.CreateInstanceAt(lowerBounds.x + entrance.x, lowerBounds.y + entrance.y);
                DungeonRoomMovementTrigger exitTrigger = DungeonRoomMovementTrigger.CreateInstanceAt(lowerBounds.x + exit.x, lowerBounds.y + exit.y);

                entranceTrigger.OnMovementDetected += (position) => OnRoomEnter(dungeonRoom, roomVariant, position);
                exitTrigger.OnMovementDetected += (position) => OnRoomExit(dungeonRoom, position);
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
