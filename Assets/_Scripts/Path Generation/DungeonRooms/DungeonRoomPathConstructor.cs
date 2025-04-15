using System;
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
            var expandCornerTileFunc = GetExpandCornerTileFunction(dungeonRoom);

            dungeonRoom.Tiles.LoopThroughTiles(expandCornerTileFunc, TilesMatrix.LoopType.WithoutEdges);

            return dungeonRoom;
        }

        private Action<int, int, Tile> GetExpandCornerTileFunction(DungeonRoom dungeonRoom)
        {
            return (int x, int y, Tile tile) => 
            {
                if (!tile.StateData.IsCorner) return;

                var pos = new Vector2Int(x, y);

                var newPath = new Path(dungeonRoom.Tiles, pos, pos, _stemLength, shouldLog: false);

                newPath.RandomWalk();

                dungeonRoom.Tiles.MergeWithPath(newPath);
            };
  
        }
    }
}
