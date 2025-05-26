using UnityEngine;

namespace PathGeneration
{
    public class DungeonRoomVariant : IDungeonRoom
    {
        public DungeonRoomVariant((Vector2Int, Vector2Int) bounds, Vector2Int enterPosition = default, Vector2Int exitPosition = default)
        {
            Bounds = bounds;
            EnterPosition = enterPosition;
            ExitPosition = exitPosition;
        }

        public TilesMatrix Tiles { get; private set; }

        public (Vector2Int, Vector2Int) Bounds { get; private set; }

        public Vector2Int EnterPosition { get; private set; }
        public Vector2Int ExitPosition { get; private set; }

        public void SetTiles(TilesMatrix tiles, bool shouldResetTiles = false)
        {
            Tiles = tiles;

            if (shouldResetTiles) Tiles.ResetTiles();

            Tiles.LoopThroughTiles(SetTileToDungeonRoomTile, TilesMatrix.LoopType.All);

            Tiles.InvalidateBorders();
        }

        private void SetTileToDungeonRoomTile (int x, int y, Tile tile) => tile.SetAsDungeonRoomTile();
    }
}
