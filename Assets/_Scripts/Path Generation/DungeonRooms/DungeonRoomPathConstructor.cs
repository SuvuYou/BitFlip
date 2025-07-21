using UnityEngine;

namespace PathGeneration
{
    public interface IDungeonRoomPathConstructor
    {
        public abstract IDungeonRoom ConstructPath(DungeonRoom dungeonRoom);
        public virtual IDungeonRoom ConstructPath(IDungeonRoom dungeonRoom) { return null; }
    }

    public class VarietyDungeonRoomPathConstructor : IDungeonRoomPathConstructor
    {
        private SimpleExtensionDungeonRoomPathConstructor _simpleExtensionDungeonRoomPathConstructor = new ();

        public IDungeonRoom ConstructPath(DungeonRoom dungeonRoom)
        {
            dungeonRoom.FindEnterExitPositionPairs();
            dungeonRoom.SetupDungeonRoomVariants();

            foreach (var variant in dungeonRoom.VariantsPerEntrance.Values)
            {
                _simpleExtensionDungeonRoomPathConstructor.ConstructPath(variant);
            }

            return dungeonRoom;
        }
    }

    public class SimpleExtensionDungeonRoomPathConstructor : IDungeonRoomPathConstructor
    {
        private const int MAX_PATH_ATTEMPTS = 6;
        private const float MIN_PATH_PERCENTAGE = 0.8f;

        private PseudoRandom.SystemRandomManager _random;

        public SimpleExtensionDungeonRoomPathConstructor() 
        {
            _random = PseudoRandom.SystemRandomHolder.UseSystem(PseudoRandom.SystemRandomType.PathGeneration);
        }

        public IDungeonRoom ConstructPath(IDungeonRoom dungeonRoom)
        {
            int pathCounter = 0;

            while (dungeonRoom.Tiles.GetPathPercentage() < MIN_PATH_PERCENTAGE && pathCounter < MAX_PATH_ATTEMPTS)
            {
                pathCounter++;

                if (!dungeonRoom.Tiles.TryGetTwoConnectiveTiles(_random, out Vector2Int cornerTilePosition, out Vector2Int singleTilePosition, out Direction lockedDirection)) break;

                var newPath = new Path(dungeonRoom.Tiles, cornerTilePosition, singleTilePosition, lockedDirection);

                newPath.RandomWalk();
            }

            return dungeonRoom;
        }

        public IDungeonRoom ConstructPath(DungeonRoom dungeonRoom)
        {
            return ConstructPath(dungeonRoom);
        }
    }
}
