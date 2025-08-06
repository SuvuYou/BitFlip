using UnityEngine;

namespace PathGeneration
{
    public enum DungeonDoorType { Entrance, Exit }
    public enum DungeonDoorState { Open, Locked, Unexplored }

    public class DungeonDoor 
    {
        public DungeonDoorType Type { get; private set; }
        public DungeonDoorState State { get; private set; }

        public DungeonTilePosition Position { get; private set; }
        public Direction LeadingDirection { get; private set; }

        public DungeonDoor(DungeonDoorType type, DungeonTilePosition position, Direction leadingDirection)
        {
            Type = type;
            Position = position;
            LeadingDirection = leadingDirection;
            State = DungeonDoorState.Unexplored;
        }

        public void SetState(DungeonDoorState dungeonDoorState)
        {
            State = dungeonDoorState;
        }
    }
}
