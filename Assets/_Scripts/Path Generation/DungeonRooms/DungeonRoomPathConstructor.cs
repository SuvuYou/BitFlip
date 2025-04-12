namespace PathGeneration
{
    public interface IDungeonRoomPathConstructor
    {
        public abstract DungeonRoom ConstructPath(DungeonRoom dungeonRoom);
    }

    public class DungeonRoomPathConstructor : IDungeonRoomPathConstructor
    {
        public DungeonRoom ConstructPath(DungeonRoom dungeonRoom)
        {
            return dungeonRoom;
        }
    }
}
