using UnityEngine;

namespace PathGeneration
{
    public class DungeonRoomVariant : IDungeonRoom
    {
        public DungeonRoomVariant((Vector2Int, Vector2Int) bounds, DungeonTilePosition enterPosition = default, DungeonTilePosition exitPosition = default)
        {
            Bounds = bounds;
            EnterPosition = enterPosition;
            ExitPosition = exitPosition;
        }

        public TilesMatrix Tiles { get; private set; }

        public (Vector2Int, Vector2Int) Bounds { get; private set; }

        public DungeonTilePosition EnterPosition { get; private set; }
        public DungeonTilePosition ExitPosition { get; private set; }

        public void SetTiles(TilesMatrix tiles)
        {
            Tiles = tiles;

            Tiles.LoopThroughTiles(SetTileToDungeonRoomTile, TilesMatrix.LoopType.All);

            Tiles.InvalidateBorders();
        }

        private void SetTileToDungeonRoomTile (int x, int y, Tile tile) => tile.SetAsDungeonRoomTile();
    }
}
