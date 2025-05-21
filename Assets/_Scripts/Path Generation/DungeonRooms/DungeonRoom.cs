using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PathGeneration
{
    public enum DungeonRoomType { DedlyWall, Doorswitch, FlipPuzzle }

    public class DungeonRoom
    {
        private PseudoRandom.SystemRandomManager _random;

        public DungeonRoomType Type { get; private set; }

        public DungeonRoom(DungeonRoomType type, (Vector2Int, Vector2Int) bounds, List<Vector2Int> exitPositionPairs)
        {
            Type = type;
            Bounds = bounds;
            ExitPositions = exitPositionPairs;

            _random = PseudoRandom.SystemRandomHolder.UseSystem(PseudoRandom.SystemRandomType.Other);
        }

        public TilesMatrix Tiles { get; private set; }

        public (Vector2Int, Vector2Int) Bounds { get; private set; }

        public List<Vector2Int> ExitPositions { get; private set; }
        public List<(Vector2Int, Vector2Int)> EnterExitPositionPairs { get; private set; } = new();

        public void SetTiles(TilesMatrix tiles)
        {
            Tiles = tiles;

            Tiles.LoopThroughTiles(SetTileToDungeonRoomTile, TilesMatrix.LoopType.All);

            Tiles.InvalidateBorders();
        }

        private void SetTileToDungeonRoomTile (int x, int y, Tile tile) => tile.SetAsDungeonRoomTile();

        public Vector3Int GetRandomPathTilePosition()
        {
            var pathTiles = Tiles.GetOccupiedPositions();

            return (Bounds.Item1 + pathTiles.ElementAt(_random.GetRandomInt(0, pathTiles.Count))).ToVector3WithZ(z: 0);
        }

        public void FindEnterExitPositionPairs() 
        {
            List<Vector2Int> exitPositionsbuffer = new(ExitPositions.Count);

            foreach (var exitPosition in ExitPositions)
            {
                exitPositionsbuffer.Add(new Vector2Int(exitPosition.x - Bounds.Item1.x, exitPosition.y - Bounds.Item1.y));
            }

            for (int i = 0; i < exitPositionsbuffer.Count; i++)
            {
                Tiles.FollowPath(exitPositionsbuffer[i], Tiles.GetOccupiedPositions(), (int x, int y, Tile tile) => 
                { 
                    if (exitPositionsbuffer.Any(pos => pos.x == x && pos.y == y) && exitPositionsbuffer[i].x != x && exitPositionsbuffer[i].y != y)
                    {   
                        EnterExitPositionPairs.Add((exitPositionsbuffer[i], new Vector2Int(x, y)));
                        exitPositionsbuffer.Remove(exitPositionsbuffer[i]);
                        exitPositionsbuffer.Remove(new Vector2Int(x, y));
                    } 
                });
            }

            Debug.Log("Strarasdtsdagfewsgf");

            foreach (var exit in ExitPositions)
            {
                Debug.Log($"Exit: {new Vector2Int(exit.x - Bounds.Item1.x, exit.y - Bounds.Item1.y)}");
            }

            foreach (var pair in EnterExitPositionPairs)
            {
                Debug.Log($"Enter: {pair.Item1}, Exit: {pair.Item2}");
            }

            Debug.Log("Endnndnasegfnesrg");
        }
    }
}
