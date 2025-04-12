namespace PathGeneration
{
    public interface IDungeonRoomTransformer
    {
        public abstract DungeonRoom TransformDungeonRoom(DungeonRoom dungeonRoom);
    }

    public class DungeonRoomTransformer : IDungeonRoomTransformer
    {

        public DungeonRoom TransformDungeonRoom(DungeonRoom dungeonRoom)
        {
            dungeonRoom.LoopThroughTiles(Transform, DungeonRoom.LoopType.InnerRoom);

            return dungeonRoom;
        }


        private void Transform(int x, int y, Tile tile)
        {
            
        }
    }
}
