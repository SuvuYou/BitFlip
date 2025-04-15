using UnityEngine;

namespace PathGeneration
{
    public class Map
    {
        public readonly Path MapPath;
        public readonly PseudoRandom.SystemRandomManager _systemRandom;

        private IDungeonRoomFinder _dungeonRoomFinder = new DungeonRoomFinder();
        private IDungeonRoomPathConstructor _dungeonRoomPathConstructor = new DungeonRoomPathConstructor();
        private IDungeonRoomTransformer _dungeonRoomTransformer = new DungeonRoomTransformer();

        public Vector2Int MaxRoomSize { get; private set; } 
        public Vector2Int MinRoomSize { get; private set; }
        public int MaxNumberOfDungeonRooms { get; private set; }
        public int NumberOfDungeonRooms { get; private set; }

        public Map(GameDataSO gameDataSO)
        {
            Vector2Int startPos = new (0 + gameDataSO.MapBorderSize.x, 0 + gameDataSO.MapBorderSize.y);
            Vector2Int endPos = new (gameDataSO.MapWidth - 1 - gameDataSO.MapBorderSize.x, gameDataSO.MapHeight - 1 - gameDataSO.MapBorderSize.y);

            MapPath = new Path(gameDataSO.MapWidth, gameDataSO.MapHeight, startPos, endPos, gameDataSO.MapBorderSize, gameDataSO.MapStemLength);

            _systemRandom = PseudoRandom.SystemRandomHolder.UseSystem(PseudoRandom.SystemRandomType.Other);

            MaxNumberOfDungeonRooms = gameDataSO.MaxNumberOfDungeonRooms;
            NumberOfDungeonRooms = 0;

            MaxRoomSize = gameDataSO.MaxDungeonRoomSize;
            MinRoomSize = gameDataSO.MinDungeonRoomSize;
        }

        public void Generate()
        {
            MapPath.RandomWalk();

            // ExpandCorners();
            GenerateDungeonRooms();
        }

        private void ExpandCorners()
        {
            foreach (var (pos, tile) in MapPath.Tiles.GetCornerTiles())
            {
                if (_systemRandom.GetRandomFloat() > 0.5f)
                {
                    var newPath = new Path(MapPath.Tiles.Width, MapPath.Tiles.Height, pos, pos, MapPath.Tiles.BorderSize, MapPath.StemLength, MapPath.Tiles.GetOccupiedPositions());
                    newPath.RandomWalk();
                    MapPath.Tiles.MergeWithPath(newPath);
                }
            }
        }

        private void GenerateDungeonRooms()
        {
            foreach (var (pos, tile) in MapPath.Tiles.GetCornerTiles())
            {
                if (_systemRandom.GetRandomFloat() > 0.5f && NumberOfDungeonRooms < MaxNumberOfDungeonRooms)
                {
                    if (!_dungeonRoomFinder.TryFindDungeonRoom(MapPath, pos, MinRoomSize, MaxRoomSize, out DungeonRoom dungeonRoom)) continue;

                    dungeonRoom = _dungeonRoomPathConstructor.ConstructPath(dungeonRoom);

                    // dungeonRoom = _dungeonRoomTransformer.TransformDungeonRoom(dungeonRoom);
                    
                    MapPath.Tiles.MergeWithDungeonRoom(dungeonRoom);

                    NumberOfDungeonRooms++;
                }
            }
        }
    }
}
