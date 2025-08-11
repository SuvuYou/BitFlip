
using PathGeneration;
using UnityEngine;

namespace MapRendering
{
    public class MapController : MonoBehaviour
    {
        [SerializeField] private SwappableTilemapRenderer _renderer;
        [SerializeField] private DungeonRoomController _dungeonRoomController;

        private Map _map;

        private void Start()
        {
            SwapSystem.SwappableEntitiesManager.Instance.OnSwapAtYLevelComplete += (int yLevel) => _renderer.ReRenderTilemapRegion(new Vector2Int(0, 0), new Vector2Int(_renderer.Width - 1, yLevel)); 
        }

        public Map GenerateMap()
        {
            _map = new Map();

            _map.Generate();

            _renderer.ConstructTilemap(_map);

            _renderer.RenderTilemap();

            foreach (DungeonRoom dungeonRoom in _map.DungeonRooms)
            {
                _dungeonRoomController.SetupRoomEntranceTriggers(dungeonRoom);
            }

            return _map;
        }
    }
}
