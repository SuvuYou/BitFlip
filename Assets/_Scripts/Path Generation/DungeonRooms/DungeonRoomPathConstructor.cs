using System;
using System.Linq;
using UnityEngine;

namespace PathGeneration
{
    public interface IDungeonRoomPathConstructor
    {
        public abstract DungeonRoom ConstructPath(DungeonRoom dungeonRoom);
    }

    public class DungeonRoomPathConstructor : IDungeonRoomPathConstructor
    {
        private PseudoRandom.SystemRandomManager _random;

        private int _stemLength;

        public DungeonRoomPathConstructor(GameDataSO gameDataSO) 
        {
            _random = PseudoRandom.SystemRandomHolder.UseSystem(PseudoRandom.SystemRandomType.PathGeneration);

            _stemLength = gameDataSO.MapStemLength;
        }

        public DungeonRoom ConstructPath(DungeonRoom dungeonRoom)
        {
            int i = 0;

            var cornerTiles = dungeonRoom.Tiles.GetCornerTiles();
            var pathfinder = new Pathfinder(dungeonRoom.Tiles);

            var exampleCornerTile = cornerTiles.ElementAt(0);
            
            // TODO:
            // Set minimum distance for random unoccupied position
            while (i < 1)
            {
                i++;

                if (!dungeonRoom.Tiles.TryGetRandomUnoccupiedPosition(_random, exampleCornerTile.x % 2 == 0, exampleCornerTile.y % 2 == 0, out Vector2Int randomUnoccupiedPosition)) continue;

                var randomCornerTile = cornerTiles.ElementAt(_random.GetRandomInt(0, cornerTiles.Count));

                // if (!pathfinder.IsPathFindable(randomUnoccupiedPosition, randomCornerTile)) continue;

                dungeonRoom.Tiles.TraversePathRoute(randomCornerTile);

                var pathTiles = dungeonRoom.Tiles.GetSingleOccupiedPositionsByRoute(1);

                pathTiles = pathTiles.Where(tilePosition => tilePosition.x % 2 == exampleCornerTile.x % 2 && tilePosition.y % 2 == exampleCornerTile.y % 2).ToHashSet();

                if (pathTiles.Count == 0) continue;

                dungeonRoom.Tiles.IncreaseCurrentLargestRouteIndex();

                // var newPath1 = new Path(dungeonRoom.Tiles, randomCornerTile, randomUnoccupiedPosition, _stemLength, dungeonRoom.Tiles.CurrentLargestRouteIndex);
                var newPath2 = new Path(dungeonRoom.Tiles, randomUnoccupiedPosition, pathTiles.ElementAt(0), _stemLength, dungeonRoom.Tiles.CurrentLargestRouteIndex);

                Debug.Log(i);
                Debug.Log("randomCornerTile " + randomCornerTile);
                Debug.Log("randomUnoccupiedPosition " + randomUnoccupiedPosition);
                Debug.Log("randomNonCornerPosition " + pathTiles.ElementAt(0));

                // newPath1.RandomWalk();
                newPath2.RandomWalk();
            }

            // var expandCornerTileFunc = GetExpandCornerTileFunction(dungeonRoom);

            // dungeonRoom.Tiles.LoopThroughTiles(expandCornerTileFunc, TilesMatrix.LoopType.WithoutEdges);

            return dungeonRoom;
        }

        private Action<int, int, Tile> GetExpandCornerTileFunction(DungeonRoom dungeonRoom)
        {
            return (int x, int y, Tile tile) => 
            {
                if (tile.StateData.ConnectionType != TileConnectionType.Corner) return;

                var pos = new Vector2Int(x, y);

                var newPath = new Path(dungeonRoom.Tiles, pos, pos, _stemLength, shouldLog: false);

                newPath.RandomWalk();

                dungeonRoom.Tiles.MergeWithPath(newPath);
            };
  
        }
    }
}
