using UnityEngine;

namespace PathGeneration
{
    public enum DungeonRoomType { DedlyWall, Doorswitch, FlipPuzzle }

    public class DungeonRoom
    {
        public DungeonRoomType Type { get; private set; }

        public DungeonRoom(DungeonRoomType type, (Vector2Int, Vector2Int) bounds)
        {
            Type = type;
            Bounds = bounds;
        }

        public TilesMatrix Tiles { get; private set; }

        public (Vector2Int, Vector2Int) Bounds { get; private set; }

        public Vector2Int EnterPosition { get; private set; }
        public Vector2Int ExitPosition { get; private set; }

        public void SetTiles(TilesMatrix tiles)
        {
            Tiles = tiles;

            Tiles.LoopThroughTiles(SetTileToDungeonRoomTile, TilesMatrix.LoopType.All);
        }

        private void SetTileToDungeonRoomTile (int x, int y, Tile tile) => tile.SetAsDungeonRoomTile();
    }
}
