using System;
using System.Collections.Generic;
using UnityEngine;

namespace PathGeneration
{
    public class DungeonRoomController : MonoBehaviour
    {
        [SerializeField] private SwappableTilemapRenderer _renderer;

        private List<DungeonRoom> _dungeonRooms = new(4);

        private void Update() 
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                _dungeonRooms[0].SetDungeonRoomVariant();
                _renderer.ReConstructTilemapRegion(_dungeonRooms[0].Bounds.Item1, _dungeonRooms[0].Bounds.Item2);
                _renderer.ReRenderTilemapRegion(_dungeonRooms[0].Bounds.Item1, _dungeonRooms[0].Bounds.Item2);
            }
        }

        public void SetupRoomEntranceTriggers(DungeonRoom dungeonRoom)
        {
            _dungeonRooms.Add(dungeonRoom);

            foreach(((Vector2Int entrance, Vector2Int exit), DungeonRoomVariant roomVariant) in dungeonRoom.VariantsPerEntrance)
            {
                (Vector2Int lowerBounds, Vector2Int upperBounds) = dungeonRoom.Bounds;

                DungeonRoomMovementTrigger entranceTrigger = DungeonRoomMovementTrigger.CreateInstanceAt(lowerBounds.x + entrance.x, lowerBounds.y + entrance.y);
                DungeonRoomMovementTrigger exitTrigger = DungeonRoomMovementTrigger.CreateInstanceAt(lowerBounds.x + exit.x, lowerBounds.y + exit.y);

                entranceTrigger.OnMovementDetected += () => RenderRoomVarient(dungeonRoom, roomVariant);
                exitTrigger.OnMovementDetected += () => RenderRoomVarient(dungeonRoom, dungeonRoom.OriginalVariant);
            }
        }

        private void RenderRoomVarient(DungeonRoom dungeonRoom, DungeonRoomVariant roomVariant)
        {
            dungeonRoom.Tiles.SetTilesDataFromMatrix(roomVariant.Tiles);
            _renderer.ReConstructTilemapRegion(_dungeonRooms[0].Bounds.Item1, _dungeonRooms[0].Bounds.Item2);
            _renderer.ReRenderTilemapRegion(dungeonRoom.Bounds.Item1, dungeonRoom.Bounds.Item2);
        }
    }
}
