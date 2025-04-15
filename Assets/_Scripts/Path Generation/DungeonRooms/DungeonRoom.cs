using System;
using System.Collections.Generic;
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

        public int Width => Tiles.GetLength(0);
        public int Height => Tiles.GetLength(1);

        public (Vector2Int, Vector2Int) Bounds { get; private set; }

        public Vector2Int EnterPosition { get; private set; }
        public Vector2Int ExitPosition { get; private set; }

        public void SetTiles(Tile[,] tiles) => Tiles = tiles;

        public void LoopThroughTiles(Action<int, int, Tile> action, LoopType loopType = LoopType.InnerRoom) 
        {
            switch (loopType)
            {
                case LoopType.InnerRoom:
                    for (int x = 1; x < Width - 1; x++)
                    {
                        for (int y = 1; y < Height - 1; y++)
                        {
                            action(x, y, Tiles[x, y]);
                        }
                    }
                    break;
                case LoopType.FullRoom:
                    for (int x = 0; x < Width; x++)
                    {
                        for (int y = 0; y < Height; y++)
                        {
                            action(x, y, Tiles[x, y]);
                        }
                    }
                    break;
                case LoopType.Borders:
                    for (int x = 0; x < Width; x++)
                    {
                        action(x, 0, Tiles[x, 0]); 
                        action(x, Height - 1, Tiles[x, Height - 1]); 
                    }

                    for (int y = 1; y < Height - 1; y++)
                    {
                        action(0, y, Tiles[0, y]);
                        action(Width - 1, y, Tiles[Width - 1, y]); 
                    } 
                    break;
            }
        }

        public void Merge(Path other)
        {
            var MergeTileFunc = GetMergeTileFunction(other);

            LoopThroughTiles(MergeTileFunc, LoopType.InnerRoom);
        }

        private Action<int, int, Tile> GetMergeTileFunction(Path otherPath)
        {
            return (int x, int y, Tile tile) => 
            {
                if (otherPath.GetTileByPosition(x, y).StateData.Type == TileType.Path)
                {
                    Tiles[x, y].CloneStateData(otherPath.GetTileByPosition(x, y));

                    SetupAdjacentConnections(x, y);
                }   
            };
        }   

        public HashSet<Vector2Int> GetOccupiedPositions()
        {
            var positions = new HashSet<Vector2Int>();

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (Tiles[x, y].StateData.Type == TileType.Path)
                        positions.Add(new Vector2Int(x, y));
                }
            }

            return positions;
        }

        private void SetupAdjacentConnections(int x, int y)
        {
            foreach (var direction in DirectionExtentions.AllDirections)
            {
                int newX = x + direction.ToVector().x;
                int newY = y + direction.ToVector().y;

                if (newX < 0 || newY < 0 || newX >= Width || newY >= Height) continue;

                if (Tiles[newX, newY].StateData.Type == TileType.Path)
                {
                    Tiles[x, y].AddConnection(direction);
                    Tiles[newX, newY].AddConnection(direction.Opposite());
                }

                if (Tiles[newX, newY].StateData.Type == TileType.Wall)
                {
                    Tiles[x, y].RemoveConnection(direction);
                    Tiles[newX, newY].RemoveConnection(direction.Opposite());
                }
            }
        }

    }
}
