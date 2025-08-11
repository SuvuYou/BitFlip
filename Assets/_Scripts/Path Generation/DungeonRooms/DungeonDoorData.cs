using UnityEngine;

namespace PathGeneration
{
    public enum DungeonDoorType { Entrance, Exit }
    public enum DungeonDoorState { Open, Locked, Unexplored }

    public class DungeonDoorData 
    {
        public DungeonDoorType Type { get; private set; }
        public DungeonDoorState State { get; private set; }

        public DungeonTilePosition Position { get; private set; }
        public DungeonTilePosition FirstWalkableSpacePosition { get; private set; }

        public Direction LeadingDirection { get; private set; }
        public DungeonRoomVariant RelatedRoomVariant { get; private set; }

        public DungeonDoorData(DungeonDoorType type, DungeonTilePosition position, DungeonRoomVariant relatedRoomVariant, Direction leadingDirection)
        {
            Type = type;
            Position = position;
            LeadingDirection = leadingDirection;
            RelatedRoomVariant = relatedRoomVariant;

            State = DungeonDoorState.Unexplored;

            FirstWalkableSpacePosition = new DungeonTilePosition(
                position.Local + leadingDirection.ToVector(),
                position.Global + leadingDirection.ToVector()
            );
        }

        public void SetState(DungeonDoorState dungeonDoorState)
        {
            State = dungeonDoorState;
        }
    }
}
