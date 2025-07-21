using System;
using UnityEngine;

namespace PathGeneration
{
    public class DungeonRoomController : MonoBehaviour
    {
        [SerializeField] private SwappableTilemapRenderer _renderer;

        public void SetupRoomEntranceTriggers(DungeonRoom dungeonRoom)
        {
            foreach(((Vector2Int entrance, Vector2Int exit), DungeonRoomVariant roomVariant) in dungeonRoom.VariantsPerEntrance)
            {
                DungeonRoomEnterringTrigger trigger = DungeonRoomEnterringTrigger.CreateInstanceAt(entrance.x, entrance.y);

                trigger.OnRoomEntered += () => RenderRoomVarient(dungeonRoom, roomVariant);
            }
        }

        private void RenderRoomVarient(DungeonRoom dungeonRoom, DungeonRoomVariant roomVariant)
        {
            dungeonRoom.Tiles.SetTilesDataFromMatrix(roomVariant.Tiles);
            _renderer.ReRenderTilemapRegion(roomVariant.Bounds.Item1, roomVariant.Bounds.Item2);
        }
    }
}
