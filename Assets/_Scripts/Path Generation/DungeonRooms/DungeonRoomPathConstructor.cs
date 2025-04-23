using UnityEngine;

namespace PathGeneration
{
    public interface IDungeonRoomPathConstructor
    {
        public abstract DungeonRoom ConstructPath(DungeonRoom dungeonRoom);
    }

    public class DungeonRoomPathConstructor : IDungeonRoomPathConstructor
    {
        private const int MAX_PATH_ATTEMPTS = 6;
        private const float MIN_PATH_PERCENTAGE = 0.8f;

        private PseudoRandom.SystemRandomManager _random;

        private int _stemLength;

        public DungeonRoomPathConstructor(GameDataSO gameDataSO) 
        {
            _random = PseudoRandom.SystemRandomHolder.UseSystem(PseudoRandom.SystemRandomType.PathGeneration);

            _stemLength = gameDataSO.MapStemLength;
        }

        public DungeonRoom ConstructPath(DungeonRoom dungeonRoom)
        {
            int pathCounter = 0;

            while (dungeonRoom.Tiles.GetPathPercentage() < MIN_PATH_PERCENTAGE && pathCounter < MAX_PATH_ATTEMPTS)
            {
                pathCounter++;

                if (!dungeonRoom.Tiles.TryGetTwoConnectiveTiles(_random, out Vector2Int cornerTilePosition, out Vector2Int singleTilePosition, out Direction lockedDirection)) break;

                var newPath = new Path(dungeonRoom.Tiles, cornerTilePosition, singleTilePosition, lockedDirection, _stemLength, dungeonRoom.Tiles.CurrentLargestRouteIndex);

                newPath.RandomWalk();
            }

            return dungeonRoom;
        }
    }
}
