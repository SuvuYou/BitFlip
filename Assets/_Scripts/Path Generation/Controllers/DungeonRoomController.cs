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

                entranceTrigger.OnMovementDetected += () => OnRoomEnter(dungeonRoom, roomVariant);
                exitTrigger.OnMovementDetected += () => OnRoomExit(dungeonRoom);
            }
        }

        private void OnRoomEnter(DungeonRoom dungeonRoom, DungeonRoomVariant roomVariant)
        {
            RenderRoomVarient(dungeonRoom, roomVariant);

            RenderDoors(dungeonRoom, roomVariant);
        }

        private void OnRoomExit(DungeonRoom dungeonRoom) 
        {
            RenderRoomVarient(dungeonRoom, dungeonRoom.OriginalVariant);
        }

        private void RenderRoomVarient(DungeonRoom dungeonRoom, DungeonRoomVariant roomVariant)
        {
            dungeonRoom.Tiles.SetTilesDataFromMatrix(roomVariant.Tiles);

            _renderer.ReConstructTilemapRegion(dungeonRoom.Bounds.Item1, dungeonRoom.Bounds.Item2);
            _renderer.ReRenderTilemapRegion(dungeonRoom.Bounds.Item1, dungeonRoom.Bounds.Item2);
        }

        private void RenderDoors(DungeonRoom dungeonRoom, DungeonRoomVariant roomVariant)
        {
            _renderer.ForceRenderTileAt(dungeonRoom.Bounds.Item1.x + roomVariant.EnterPosition.x, dungeonRoom.Bounds.Item1.y + roomVariant.EnterPosition.y, _dungeonRoomDoorTile);
            _renderer.ForceRenderTileAt(dungeonRoom.Bounds.Item1.x + roomVariant.ExitPosition.x, dungeonRoom.Bounds.Item1.y + roomVariant.ExitPosition.y, _dungeonRoomDoorTile);
        }
    }
}
