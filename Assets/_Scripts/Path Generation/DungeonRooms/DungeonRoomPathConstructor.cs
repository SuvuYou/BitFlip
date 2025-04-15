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

        public DungeonRoomPathConstructor() 
        {
            _random = PseudoRandom.SystemRandomHolder.UseSystem(PseudoRandom.SystemRandomType.PathGeneration);
        }

        public DungeonRoom ConstructPath(DungeonRoom dungeonRoom)
        {
            var expandCornerTileFunc = GetExpandCornerTileFunction(dungeonRoom);

            dungeonRoom.LoopThroughTiles(expandCornerTileFunc, DungeonRoom.LoopType.InnerRoom);

            return dungeonRoom;
        }

        private Action<int, int, Tile> GetExpandCornerTileFunction(DungeonRoom dungeonRoom)
        {
            return (int x, int y, Tile tile) => 
            {
                if (!tile.StateData.IsCorner) return;

                var pos = new Vector2Int(x, y);

                Debug.Log("sadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsggsadsdgsgg");

                var newPath = new Path(dungeonRoom.Width, dungeonRoom.Height, pos, pos, new Vector2Int(0, 0), 2, dungeonRoom.GetOccupiedPositions(), shouldLog: false);

                newPath.RandomWalk();

                dungeonRoom.Merge(newPath);
            };
  
        }
    }
}
