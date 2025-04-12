using System;
using UnityEngine;

namespace PathGeneration
{
    public enum DungeonRoomType { DedlyWall, Doorswitch, FlipPuzzle }

    public class DungeonRoom
    {
        public enum LoopType { Borders, InnerRoom, FullRoom }

        public DungeonRoomType Type { get; private set; }

        public DungeonRoom(DungeonRoomType type, (Vector2Int, Vector2Int) bounds)
        {
            Type = type;
            Bounds = bounds;
        }

        public Tile[,] Tiles { get; private set; }

        public (Vector2Int, Vector2Int) Bounds { get; private set; }
        public Vector2Int EnterPosition { get; private set; }
        public Vector2Int ExitPosition { get; private set; }

        public void SetTiles(Tile[,] tiles) => Tiles = tiles;

        public void LoopThroughTiles(Action<int, int, Tile> action, LoopType loopType = LoopType.InnerRoom) 
        {
            int width = Tiles.GetLength(0);
            int height = Tiles.GetLength(1);

            switch (loopType)
            {
                case LoopType.InnerRoom:
                    for (int x = 1; x < width - 1; x++)
                    {
                        for (int y = 1; y < height - 1; y++)
                        {
                            action(x, y, Tiles[x, y]);
                        }
                    }
                    break;
                case LoopType.FullRoom:
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            action(x, y, Tiles[x, y]);
                        }
                    }
                    break;
                case LoopType.Borders:
                    for (int x = 0; x < width; x++)
                    {
                        action(x, 0, Tiles[x, 0]); 
                        action(x, height - 1, Tiles[x, height - 1]); 
                    }

                    for (int y = 1; y < height - 1; y++)
                    {
                        action(0, y, Tiles[0, y]);
                        action(width - 1, y, Tiles[width - 1, y]); 
                    } 
                    break;
            }
        }
    }
}
