using System;
using System.Collections.Generic;
using UnityEngine;

namespace PathGeneration
{
    public class DungeonRoomController : MonoBehaviour
    {
        [SerializeField] private SwappableTilemapRenderer _renderer;

        private List<DungeonRoom> dungeonRooms = new(4);

        private void Update() 
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                dungeonRooms[0].SetDungeonRoomVariant();
                _renderer.ReConstructTilemapRegion(dungeonRooms[0].Bounds.Item1, dungeonRooms[0].Bounds.Item2);
                _renderer.ReRenderTilemapRegion(dungeonRooms[0].Bounds.Item1, dungeonRooms[0].Bounds.Item2);
            }
        }

        public void SetupRoomEntranceTriggers(DungeonRoom dungeonRoom)
        {
            dungeonRooms.Add(dungeonRoom);
            foreach(((Vector2Int entrance, Vector2Int exit), DungeonRoomVariant roomVariant) in dungeonRoom.VariantsPerEntrance)
            {
                (Vector2Int lowerBounds, Vector2Int upperBounds) = dungeonRoom.Bounds;

                DungeonRoomEnterringTrigger trigger = DungeonRoomEnterringTrigger.CreateInstanceAt(lowerBounds.x + entrance.x, lowerBounds.y + entrance.y);

                trigger.OnRoomEntered += () => RenderRoomVarient(dungeonRoom, roomVariant);
            }
        }

        private void RenderRoomVarient(DungeonRoom dungeonRoom, DungeonRoomVariant roomVariant)
        {
            dungeonRoom.Tiles.SetTilesDataFromMatrix(roomVariant.Tiles);
            _renderer.ReConstructTilemapRegion(dungeonRooms[0].Bounds.Item1, dungeonRooms[0].Bounds.Item2);
            _renderer.ReRenderTilemapRegion(dungeonRoom.Bounds.Item1, dungeonRoom.Bounds.Item2);
        }
    }
}
